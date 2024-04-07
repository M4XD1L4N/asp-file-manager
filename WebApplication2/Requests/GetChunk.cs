using WebApplication2.Models;

namespace WebApplication2.Requests
{
    public class GetChunkReq
    {
        public int FileId { set; get; }
        public int Pos { set; get; }
    }
    public class GetChunkRes
    {
        public ChunkInfo? Data { set; get; }
        public int Status { set; get; }
    }
}