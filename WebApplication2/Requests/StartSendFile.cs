using WebApplication2.Models;

namespace WebApplication2.Requests
{
    public class StartSendFileReq
    {
        public string FileName { set; get; }
        public CipherType CipherType { set; get; }
        public int Size { set; get; }
    }

    public class StartSendFileRes
    {
        public int FileId { set; get; }
        public int Status { set; get; }
        public string PubKey { set; get; }
    }
}
