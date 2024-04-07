using WebApplication2.Models;

namespace WebApplication2.Requests
{
    public class SendChunkReq
    {
        public int ChunkPos { set; get; }
        public int FileId { set; get; }
        public string Data { set; get; }
    }

    public class SendChunkRes
    {
        public ChunkInfo Data {set;get;}
        public int Status { set; get; }
    }
}
