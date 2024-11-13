using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Core.DTO;
using Weblog.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Weblog.Core.Services;
using Newtonsoft.Json.Linq;
using Weblog.Core.Domain.Entities;

namespace Weblog.UI.Controllers
{
    [Route("[controller]")]
    public class PostsController : Controller
    {
        private readonly IPostsService _postsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PostsController> _logger;

        public PostsController(IPostsService postsService, UserManager<ApplicationUser> userManager, ILogger<PostsController> logger)
        {
            _postsService = postsService;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] Guid authorId, [FromQuery] int sortOrder = 1)
        {
            List<PostResponseDTO>? posts = await _postsService.SearchPosts(search);
            if (authorId != Guid.Empty) { posts = posts?.Where(temp => temp.AuthorId == authorId).ToList(); }
            if (sortOrder != -1) posts = posts?.OrderByDescending(temp => temp.PubDate).ToList();
            else posts = posts?.OrderBy(temp => temp.PubDate).ToList();
            ViewBag.CSearch = search;
            ViewBag.CAuthorId = authorId;
            ViewBag.CSortOrder = sortOrder;
            List<ApplicationUser>? authors = (await _userManager.GetUsersInRoleAsync("Author")).ToList();
            Dictionary<Guid, string> authorDict = new Dictionary<Guid, string>() { { Guid.Empty, "همه" } };
            if (authors != null)
            {
                foreach (var author in authors)
                {
                    authorDict.Add(author.Id, author.Name!);
                }

            }
            ViewBag.Authors = authorDict;
            return View(posts?.Where(p => p.Published == true).ToList());
        }

        [Route("{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(string slug)
        {
            PostResponseDTO? post = await _postsService.GetPostBySlug(slug);
            if (post == null || post.Published == false) { return RedirectToAction("NotFoundError", "Home"); }
            if (!User.IsInRole("Admin") && !User.IsInRole("Author")) { await _postsService.Viewed(slug); }
            return View(post);
        }

        [Route("[action]")]
        [HttpGet]
        [Authorize(Roles = "Author, Admin")]
        public IActionResult Add([FromQuery] int? ap = 0)
        {
            ViewBag.ap = ap;
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> Add(PostAddDTO post, [FromServices] IFilesService _filesService, [FromServices] ISubscribersService _subscribersService, [FromServices] IEmailService _emailService, [FromQuery] int? ap = 0)
        {
            if (!ModelState.IsValid) { return BadRequest(); }

            PostResponseDTO? _post = await _postsService.GetPostBySlug(post.Slug!);
            if (_post != null)
            {
                ViewBag.Duplicated = "لینک تکراری است";
                return View(post);
            }

            if (post.Header!.ContentType != "image/jpeg")
            {
                ViewBag.FormatError = "تصویر باید در فرمت jpg باشد";
                return View(post);
            }

            if (post.Published == true) post.PubDate = DateTime.Now;

            ApplicationUser? author = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            if (author == null) { return RedirectToAction("SignIn", "Account"); }
            if (!(await _userManager.IsInRoleAsync(author, "Admin"))) { post.Admin = false; }
            if (!(await _userManager.IsInRoleAsync(author, "Author"))) { post.Admin = true; }
            post.AuthorId = author.Id;

            int x = await _postsService.AddPost(post);

            if (post.Header != null)
            {
                if (post.Header.ContentType == "image/jpeg") await _filesService.SavePic(post.Header, "articles", post.Slug! + ".jpg");
            }

            if (x == 0) { throw new Exception("Add post failed."); }

            List<Subscriber>? subscribers = await _subscribersService.GetSubscribers();
            if (subscribers?.Count != 0) foreach (var s in subscribers)
                {
                    string message = $"<div dir='rtl' style='border: 2px solid; border-radius: 10px; padding: 10px; margin: 10px; font-family: Arial; font-size: 16px; background-color: #45cabf;'><img src='https://royayeravi.ir/pics/articles/{post.Slug}.jpg' alt='header' style='border-radius: 16px' /><br />با سلام<br />مطلب جدیدی در وبلاگ رویای راوی انتشار یافته است: {post.Title}<br />جهت مشاهده <a href='https://royayeravi.ir/posts/{post.Slug}'>کلیک</a> کنید.<br /><br /><a href='https://royayeravi.ir/newsletter/cancel?email={s.Email}'>لغو خبرنامه</a>";
                    await _emailService.SendEmailAsync(s.Email!, "انتشار مطلب جدید", message);
                }

            if (ap == 1) { return RedirectToAction("Index", "Admin"); }

            return RedirectToAction("Panel", "Account");
        }

        [Route("[action]/{slug?}")]
        [HttpGet]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> Edit(string? slug, [FromQuery] int? ap = 0)
        {
            if (slug == null) { return RedirectToAction("NotFoundError", "Home"); }

            PostResponseDTO? post = await _postsService.GetPostBySlug(slug);
            if (post == null) { return RedirectToAction("NotFoundError", "Home"); }

            ApplicationUser? user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            if (user == null) { return RedirectToAction("SignIn", "Account"); }
            if (user.Id != post.AuthorId)
            {
                if (!(await _userManager.IsInRoleAsync(user, "Admin"))) { return StatusCode(401); }
            }

            ViewBag.ap = ap;

            PostUpdateDTO _post = post.ToPostUpdate();
            return View(_post);
        }

        [Route("[action]/{slug?}")]
        [HttpPost]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> Edit([FromRoute] string? slug, PostUpdateDTO post, [FromServices] IFilesService _filesService, [FromQuery] int? ap)
        {
            if (!ModelState.IsValid || slug == null) { return BadRequest(); }

            if (slug != post.Slug) { return BadRequest(); }

            PostResponseDTO? _post = await _postsService.GetPostBySlug(slug);
            if (_post == null) { return RedirectToAction("NotFoundError", "Home"); }

            bool w = false;
            ApplicationUser? user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            if (user == null) { return RedirectToAction("SignIn", "Account"); }
            if (user.Id != _post.AuthorId)
            {
                if (!(await _userManager.IsInRoleAsync(user, "Admin"))) { return StatusCode(401); }
                else { w = true; }
            }

            if (_post.Published == true)
            {
                if (_post.Article != post.Article) post.UpDate = DateTime.Now; else post.UpDate = _post.UpDate;
                post.PubDate = _post.PubDate;
                post.Published = _post.Published;
            }
            int x = await _postsService.UpdatePost(post);
            if (x == 0) { throw new Exception("Update post failed: " + post.Slug); }

            if (post.Header != null)
            {
                if (post.Header.ContentType == "image/jpeg") await _filesService.SavePic(post.Header, "articles", post.Slug! + ".jpg");
            }

            if (w)
            {
                _logger.LogWarning($"Admin: {user.Id} edited post: {post.Slug}");
                return RedirectToAction("Index", "Admin");
            }

            if (ap == 1)
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Panel", "Account");
        }

        [Route("[action]/{slug?}")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> Delete(string? slug, [FromQuery] int? ap = 0)
        {
            if (slug == null) { return RedirectToAction("NotFoundError", "Home"); }

            PostResponseDTO? post = await _postsService.GetPostBySlug(slug);
            if (post == null) { return RedirectToAction("NotFoundError", "Home"); }

            ApplicationUser? user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            if (user == null) { return RedirectToAction("SignIn", "Account"); }
            bool w = false;
            if (user.Id != post.AuthorId)
            {
                if (!(await _userManager.IsInRoleAsync(user, "Admin")))
                {
                    return StatusCode(401);
                }
                else
                {
                    w = true;
                }
            }

            int x = await _postsService.DeletePost(slug);

            if (x == 0) { throw new Exception("Delete post failed"); }

            if (w)
            {
                _logger.LogWarning($"Admin: {user.Id} deleted post: {post.Slug}");
                return RedirectToAction("Index", "Admin");
            }

            if (ap == 1)
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Panel", "Account");
        }

        [Route("[action]/{slug}")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> Publish(string slug, [FromQuery] int? ap)
        {
            PostResponseDTO? post = await _postsService.GetPostBySlug(slug);
            if (post == null || post.Published == true)
            {
                return BadRequest();
            }
            post.Published = true;
            post.PubDate = DateTime.Now;
            int x = await _postsService.UpdatePost(post.ToPostUpdate());
            if (x == 0) { throw new Exception(); }
            if (ap == 1) return RedirectToAction("Index", "Admin");
            return RedirectToAction("Panel", "Account");
        }

        [Route("[action]")]
        [HttpPost]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult?> UploadImage(IFormFile upload, [FromServices] IFilesService _filesService)
        {
            if (upload == null) { return Json(false); }

            if (!upload.ContentType.Contains("image")) { return Json(false); }

            if (upload.Length > 5243000) { return Json(false); }

            string? r = await _filesService.SavePic(upload, "articles", (new Random()).Next(6400).ToString() + upload.FileName);
            
            if (r != null)
            {
                return new JsonResult(new { url = "https://" + Request.Host.ToString() + "/" + r });
            }
            
            else { return Json(false); }
        }
    }
}
