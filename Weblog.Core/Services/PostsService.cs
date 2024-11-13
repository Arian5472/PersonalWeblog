using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Core.DTO;
using Weblog.Core.Domain.RepositoryContracts;
using Weblog.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace Weblog.Core.Services
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<PostsService> _logger;

        public PostsService(IPostsRepository postsRepository, ILogger<PostsService> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<int> AddPost(PostAddDTO post)
        {
            Post? _post = await _postsRepository.GetPostBySlug(post.Slug!);
            if (_post != null) { return -1; }
            
            return await _postsRepository.AddPost(post.ToPost());
        }

        public async Task<int> DeletePost(string slug)
        {
            Post? post = await _postsRepository.GetPostBySlug(slug);
            if (post == null)
            {
                _logger.LogWarning("Received post slug for deletion not found: " + slug);
                return -1;
            }
            return await _postsRepository.DeletePost(slug);
        }

        public async Task<PostResponseDTO?> GetPostBySlug(string slug)
        {
            Post? post = await _postsRepository.GetPostBySlug(slug);
            if (post == null) { return null; }
            return PostResponseDTO.ToPostResponse(post);
        }

        public async Task<List<PostResponseDTO>?> GetPosts()
        {
            List<Post>? posts = await _postsRepository.GetPosts();
            if (posts == null) { return null; }
            return posts.Select(temp => PostResponseDTO.ToPostResponse(temp)).ToList();
        }

        public async Task<List<PostResponseDTO>?> SearchPosts(string? name)
        {
            if (name == null) { return await GetPosts(); }
            List<Post>? posts = await _postsRepository.SearchPosts(name);
            if (posts == null) { return null; };
            return posts.Select(temp => PostResponseDTO.ToPostResponse(temp)).ToList();
        }

        public List<PostResponseDTO>? SortPosts(List<PostResponseDTO>? posts, string? sortBy = "PubDate", int order = 1)
        {
            if (posts == null) { return null; }
            List<PostResponseDTO> sortedPost;

            if (sortBy == "Author")
            {
                if (order == -1) { sortedPost = posts.OrderByDescending(temp => temp.Author).ToList(); }
                else { sortedPost = posts.OrderBy(temp => temp.Author).ToList(); }
            }

            else
            {
                if (order == -1) { sortedPost = posts.OrderByDescending(temp => temp.PubDate).ToList(); }
                else { sortedPost = posts.OrderBy(temp => temp.PubDate).ToList(); };
            }

            return sortedPost;
        }

        public async Task<int> UpdatePost(PostUpdateDTO post)
        {
            Post? _post = await _postsRepository.GetPostBySlug(post.Slug!);
            if (_post == null)
            {
                _logger.LogWarning("Received post slug for Updation not found: " + post.Slug);
                return -1;
            }
            return await _postsRepository.UpdatePost(post.ToPost());
        }

        public async Task Viewed(string slug)
        {
            await _postsRepository.Viewed(slug);
        }
    }
}
