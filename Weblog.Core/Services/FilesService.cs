using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Weblog.Core.ServiceContracts;

namespace Weblog.Core.Services
{
    public class FilesService : IFilesService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string?> SavePic(IFormFile file, string filePath, string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, "pics", filePath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return "pics" + "/" + filePath + "/" + fileName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
