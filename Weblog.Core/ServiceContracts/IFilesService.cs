using Microsoft.AspNetCore.Http;

namespace Weblog.Core.ServiceContracts
{
    public interface IFilesService
    {
        Task<string?> SavePic(IFormFile file, string filePath, string fileName);
    }
}
