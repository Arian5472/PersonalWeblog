using Weblog.Core.Domain.Entities;

namespace Weblog.Core.Domain.RepositoryContracts
{
    public interface IPostsRepository
    {
        Task<int> AddPost(Post post);

        Task<List<Post>?> GetPosts();

        Task<Post?> GetPostBySlug(string slug);

        Task<int> UpdatePost(Post post);

        Task<int> DeletePost(string slug);

        Task<List<Post>?> SearchPosts(string name);

        Task Viewed(string slug);
    }
}
