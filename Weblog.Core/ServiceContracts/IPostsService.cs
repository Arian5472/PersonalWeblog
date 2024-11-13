using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Core.DTO;

namespace Weblog.Core.ServiceContracts
{
    public interface IPostsService
    {
        Task<int> AddPost(PostAddDTO post);

        Task<PostResponseDTO?> GetPostBySlug(string slug);

        Task<List<PostResponseDTO>?> GetPosts();

        List<PostResponseDTO>? SortPosts(List<PostResponseDTO>? posts, string? sortBy = "PubDate", int order = 1);

        Task<List<PostResponseDTO>?> SearchPosts(string? name);

        Task<int> UpdatePost(PostUpdateDTO post);

        Task<int> DeletePost(string slug);

        Task Viewed(string slug);
    }
}
