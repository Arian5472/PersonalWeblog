using Microsoft.AspNetCore.Identity;

namespace Weblog.Core.Domain.IdentityEntities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Password {  get; set; }
    }
}
