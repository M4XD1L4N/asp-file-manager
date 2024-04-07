using WebApplication2.Models;

namespace WebApplication2.Requests
{
    public class CompleteSendFileReq
    {
        public int FileId { set; get; }
    }

    public class CompleteSendFileRes
    {
        public int Status { set; get; }
        public string? AccessKey { set; get; }
    }

}
