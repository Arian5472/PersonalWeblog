using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Weblog.Core.Domain.IdentityEntities;

namespace Weblog.Core.Domain.Entities
{
    public class Post
    {
        [Key]
        [StringLength(100)]
        public string? Slug { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public string? Article { get; set; }

        public bool Published { get; set; }

        public DateTime? PubDate { get; set; }

        public DateTime? UpDate { get; set; }

        public int? View { get; set; } = 0;

        public bool Admin { get; set; } = false;

        public Guid AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public virtual ApplicationUser? Author { get; set; }
    }
}
