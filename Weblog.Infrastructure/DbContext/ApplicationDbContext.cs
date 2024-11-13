using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.IdentityEntities;

namespace Weblog.Infrastructure.DbContext
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public DbSet<Post> Posts { get; set; }

		public DbSet<Subscriber> Subscribers { get; set; }

		public ApplicationDbContext(DbContextOptions options) : base(options) { }
		
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Post>().ToTable("Posts");

			builder.Entity<Subscriber>().ToTable("Subscribers");

			builder.Entity<ApplicationRole>().HasData(new ApplicationRole[] { new ApplicationRole { Id = Guid.Parse("14c64666-5d8f-40fe-a0a2-2f22f0acc6b3"), Name = "Author", NormalizedName = "AUTHOR", Password = "@uthoruser9029487233" }, new ApplicationRole { Id = Guid.Parse("b7fd6484-6984-4ba8-9210-86c89c868cfb"), Name = "Admin", NormalizedName = "ADMIN", Password = "@dminuser9029487233" } });
		}
	}
}
