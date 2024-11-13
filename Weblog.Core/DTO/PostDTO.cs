using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Weblog.Core.Domain.Entities;

namespace Weblog.Core.DTO
{
    public class PostAddDTO
    {
        [StringLength(100)]
        [Required(ErrorMessage = "لینک را وارد نمایید")]
        public string? Slug { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "عنوان را وارد نمایید")]
        public string? Title { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "توضیحات را وارد نمایید")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "مطالب را وارد نمایید")]
        public string? Article { get; set; }

        public bool Published { get; set; }

        public DateTime? PubDate { get; set; }

        public Guid AuthorId { get; set; }

        public bool Admin { get; set; }

        [Required(ErrorMessage = "تصویر هدر را بارگذاری نمایید")]
        public IFormFile? Header { get; set; }

        public Post ToPost()
        {
            return new Post { Slug = Slug, Title = Title, Description = Description, Article = Article, Published = Published, PubDate = PubDate, AuthorId = AuthorId };
        }
    }

    public class PostUpdateDTO
    {
        [StringLength(100)]
        [Required]
        public string? Slug { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "عنوان را وارد نمایید")]
        public string? Title { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "توضیحات را وارد نمایید")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "مطالب را وارد نمایید")]
        public string? Article { get; set; }

        public bool Published { get; set; }

        public DateTime? PubDate { get; set; }

        public DateTime? UpDate { get; set; }

        public IFormFile? Header { get; set; }

        public Post ToPost()
        {
            return new Post { Slug = Slug, Title = Title, Description = Description, Article = Article, Published = Published, PubDate = PubDate, UpDate = UpDate };
        }
    }

    public class PostResponseDTO
    {
        public string? Slug { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Article { get; set; }

        public string? Summary { get; set; }

        public bool Published { get; set; }

        public DateTime? PubDate { get; set; }

        public DateTime? UpDate { get; set; }

        public int? View { get; set; }

        public bool Admin { get; set; }

        public Guid AuthorId { get; set; }

        public string? Author { get; set; }

        public string? AuthorBio { get; set; }

        public PostUpdateDTO ToPostUpdate()
        {
            return new PostUpdateDTO { Slug = Slug, Title = Title, Description = Description, Article = Article, Published = Published, PubDate = PubDate, UpDate = UpDate };
        }

        public static PostResponseDTO ToPostResponse(Post post)
        {
            string summary;
            if (post.Article!.Length < 186) summary = post.Article; else summary = post.Article!.Substring(0, 186) + "...";
            return new PostResponseDTO { Slug = post.Slug, Title = post.Title, Description = post.Description, Article = post.Article, Summary = summary, Published = post.Published, PubDate = post.PubDate, UpDate = post.UpDate, View = post.View, Admin = post.Admin, AuthorId = post.AuthorId, Author = post.Author?.Name, AuthorBio = post.Author?.Bio };
        }
    }
}
