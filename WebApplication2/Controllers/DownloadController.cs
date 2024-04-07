using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using WebApplication2.Models;
using WebApplication2.Requests;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        static AppDbContext dbContext = new AppDbContext();

        [HttpPost("send-access-key")]
        public SendAccessKeyRes SendAccessKey([FromBody] SendAccessKeyReq value)
        {
            var keyInfos = dbContext.Keys.Where(x => x.Data0 == value.AccessKey).ToList();
            if(keyInfos ==null || keyInfos.Count == 0)
            {
                return new SendAccessKeyRes() { Status = 300 };
            }
            var keyInfo = keyInfos.First();
            var fileInfo = dbContext.Files.Where(x => x.Id == keyInfo.FileId).ToList().First();
            var chunksCount = dbContext.Chunks.Where(x => x.FileId == keyInfo.FileId).Count();
            return new SendAccessKeyRes() { Status = 200,
                Data = fileInfo,
                numberOfChunks = chunksCount,
                privateKey = keyInfo.Data1
            };
        }
        
        [HttpPost("get-chunk")]
        public GetChunkRes GetChunk([FromBody] GetChunkReq value)
        {
            var chunkInfo = dbContext.Chunks.Where(x => x.FileId == value.FileId && x.Pos == value.Pos).First();
            if(chunkInfo == null)
                return new GetChunkRes() { Status = 300};

            return new GetChunkRes() { Status = 200, Data = chunkInfo };
        }
    }
}