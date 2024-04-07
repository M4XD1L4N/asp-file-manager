using WebApplication2.Models;

namespace WebApplication2.Requests
{
    public class SendAccessKeyReq
    {
       public string AccessKey { set; get; }
    }
    public class SendAccessKeyRes
    {
        public string privateKey { set; get; }
        public Models.FileInfo? Data { set; get; }
        public int numberOfChunks { set; get; }
        public int Status { set; get; }
    }
}