using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{

    public enum CipherType
    {
        Base64
    }

    public class FileInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Size { get; set; }
    }

    public class ChunkInfo
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int Pos { get; set; }
        public string Data { get; set; } = "";

        [ForeignKey("FileId")]
        public FileInfo? FileInfo { get; set; }
    }

    public class KeyInfo
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string? Data0 { get; set; }
        public string? Data1 { get; set; }
        public string? Data2 { get; set; }
        public string? Data3 { get; set; }
        public string? Data4 { get; set; }
        public CipherType CipherType { get; set; }

        [ForeignKey("FileId")]
        public FileInfo? FileInfo { get; set; }
    }

    public class UserKey
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string? UserAccessKey { get; set; }

        [ForeignKey("FileId")]
        public FileInfo? FileInfo { get; set; }
    }

}
