using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.RepositoryContracts;
using Weblog.Infrastructure.DbContext;

namespace Weblog.Infrastructure.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly ApplicationDbContext _db;

        public PostsRepository(ApplicationDbContext db, ILogger<PostsRepository> logger)
        {
            _db = db;
        }

        public async Task<int> AddPost(Post post)
        {
            _db.Posts.Add(post);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> DeletePost(string slug)
        {
            Post _post = await GetPostBySlug(slug);
            _db.Posts.Remove(_post!);
            return await _db.SaveChangesAsync();
        }

        public async Task<Post?> GetPostBySlug(string slug)
        {
            return await _db.Posts.Include("Author").FirstOrDefaultAsync(post => post.Slug == slug);
        }

        public async Task<List<Post>?> GetPosts()
        {
            return await _db.Posts.Include("Author").ToListAsync();
        }

        public async Task<List<Post>?> SearchPosts(string name)
        {
            return await _db.Posts.Include("Author").Where(post => post.Title!.Contains(name)).ToListAsync();
        }

        public async Task<int> UpdatePost(Post post)
        {
            Post _post = await GetPostBySlug(post.Slug);
            _post!.Title = post.Title;
            _post.Description = post.Description;
            _post.Article = post.Article;
            _post.Published = post.Published;
            _post.PubDate = post.PubDate;
            _post.UpDate = post.UpDate;
            return await _db.SaveChangesAsync();
        }

        public async Task Viewed(string slug)
        {
            Post _post = await GetPostBySlug(slug);
            _post!.View += 1;
            await _db.SaveChangesAsync();
        }
    }
}
