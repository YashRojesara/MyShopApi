using Microsoft.AspNetCore.Http;

namespace MyShopApi.Models
{
    public class FileModel
    {
        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
