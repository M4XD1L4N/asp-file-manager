using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Models;
using WebApplication2.Requests;
using WebApplication2.services;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoadController : ControllerBase
    {
        static AppDbContext dbContext = new AppDbContext();

        static Dictionary<int, string> AESCache = new Dictionary<int, string>();

        [HttpPost("start-send-file")]
        public StartSendFileRes StartSendFile([FromBody] StartSendFileReq value)
        {
            var fileInfo = new Models.FileInfo() { Name = value.FileName, Size = value.Size };
            dbContext.Files.Add(fileInfo);
            dbContext.SaveChanges();

            var data = new
            {
                AES = Convert.ToHexString(CryptoChannel.AES.GetAesKey()),
                IV = Convert.ToHexString(CryptoChannel.AES.GetIVKey())
            };
            string jsonStr = JsonConvert.SerializeObject(data);
            AESCache.Add(fileInfo.Id, jsonStr);
            storage.MakeTemporaryTable(fileInfo.Id);
            //RSA rsa = RSA.Create(4096);
            //var pubKey = rsa.ExportRSAPublicKey();
            //var pubKeyHex = Convert.ToHexString(pubKey);
            //
            //RSACache.Add(fileInfo.Id, rsa);
            return new StartSendFileRes() {FileId = fileInfo.Id,
                                           PubKey = jsonStr,
                                           Status = 200};
        }

        static ChunkStorage storage = new ChunkStorage();

        [HttpPost("send-chunk")]
        public async Task<SendChunkRes> SendChunkAsync([FromBody] SendChunkReq value)
        {
            var chunkInfo = new ChunkInfo() { Pos = value.ChunkPos, Data = value.Data, FileId = value.FileId };
            storage.AddChunkAsync(chunkInfo);
            return new SendChunkRes() { Data = chunkInfo,  Status = 200 };
        }

        static string QuickHash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }

        [HttpPost("end-send-file")]
        public CompleteSendFileRes CompleteSendFile([FromBody] CompleteSendFileReq value)
        {
            if(!AESCache.ContainsKey(value.FileId))
                return new CompleteSendFileRes() { Status = 301 };

            var privateKey = AESCache[value.FileId];

            var keyInfo = new KeyInfo() { FileId = value.FileId, Data1 = privateKey };

            var files = dbContext.Files.Where(x => x.Id == value.FileId).ToList();
            if(files ==null || files.Count == 0)
                return new CompleteSendFileRes() { Status = 300 }; // doesn't have fileInfo by id

            AESCache.Remove(value.FileId);

            dbContext.Keys.Add(keyInfo);
            //var chunks = storage.GetChunks(value.FileId);
            //dbContext.Chunks.AddRange(chunks);
            //storage.RemoveChunkInfo(value.FileId);
            storage.MoveFromTemporary(value.FileId);
            storage.DropTemporaryTable(value.FileId);
            //var chunksCount = chunks.Count;
            var chunksCount = dbContext.Chunks.Count(e => e.FileId == value.FileId);
            var file = files[0];

            var accessKey = QuickHash($"{file.Id} + {file.Name} + {file.Size} + {chunksCount}");
            keyInfo.Data0 = accessKey;
        
            dbContext.SaveChanges();


            return new CompleteSendFileRes() { Status = 200, AccessKey = accessKey };
        }

    }
}