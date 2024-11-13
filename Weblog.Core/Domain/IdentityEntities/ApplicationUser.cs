using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Weblog.Core.Domain.Entities;

namespace Weblog.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [StringLength(30)]
        public string? Name { get; set; }

        [StringLength(360)]
        public string? Bio {  get; set; }

        public virtual ICollection<Post>? Posts { get; set; }
    }
}
