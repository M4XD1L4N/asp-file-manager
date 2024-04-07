using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.services
{
    public class ChunkStorage
    {
        static AppDbContext dbContext = new AppDbContext();
        Mutex mutexObj = new Mutex(false);
        public void MakeTemporaryTable(int fileId)
        {
            dbContext.Database.ExecuteSqlRaw($"CREATE TEMP TABLE tmp_load_chunk_info_{fileId}" +
                $" (file_id INT, pos INT, data nvarchar(50000))");
        }

        public void MoveFromTemporary(int fileId)
        {
            dbContext.Database.ExecuteSqlRaw($"INSERT INTO Chunks (FileId, Pos, Data)"
                + $" SELECT file_id, pos, data FROM tmp_load_chunk_info_{fileId}");
        }

        public async Task AddChunkAsync(ChunkInfo chunk)
        {
            mutexObj.WaitOne();
            dbContext.Database.ExecuteSqlRaw($"INSERT INTO tmp_load_chunk_info_{chunk.FileId}(file_id, pos, data)" +
                $" VALUES ({chunk.FileId}, {chunk.Pos}, '{chunk.Data}')");
            mutexObj.ReleaseMutex();
        }

        public void DropTemporaryTable(int fileId)
        {
            dbContext.Database.ExecuteSqlRaw($"DROP TABLE IF EXISTS tmp_load_chunk_info_{fileId}");
        }
        /*
        public void AddChunk(ChunkInfo chunk)
        {
            if(!chunksByFileId.ContainsKey(chunk.FileId)) 
            {
                chunksByFileId.Add(chunk.FileId, new List<ChunkInfo>());
            }
            List<ChunkInfo> chunks = null;
            mutexObj.WaitOne();
            _ = chunksByFileId.TryGetValue(chunk.FileId, out chunks);
            chunks.Add(chunk);
            mutexObj.ReleaseMutex();
        }

        public List<ChunkInfo> GetChunks(int fileId)
        {
            List<ChunkInfo> chunks = null;
            mutexObj.WaitOne();
            _ = chunksByFileId.TryGetValue(fileId, out chunks);
            mutexObj.ReleaseMutex();
            return chunks;
        }

        public void RemoveChunkInfo(int fileId)
        {
            mutexObj.WaitOne();
            chunksByFileId.Remove(fileId);
            mutexObj.ReleaseMutex();
        }
        */

    }
}
