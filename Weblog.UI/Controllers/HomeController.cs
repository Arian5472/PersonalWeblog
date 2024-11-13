using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weblog.Core.DTO;
using Weblog.Core.ServiceContracts;

namespace Weblog.UI.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("/")]
        public async Task<IActionResult> Index([FromServices] IPostsService _postsService)
        {
            List<PostResponseDTO>? posts = await _postsService.GetPosts();
            posts = posts?.Where(p => p.Published == true).ToList();
            if (posts == null || posts.Count < 5) { ViewBag.Bannar = posts; }
            else
            {
                Random random = new Random();
                int[] a = new int[5];
                for (int i = 0; i < posts.Count; i++) a[i] = random.Next(0, posts.Count);
                List<PostResponseDTO> bannar = new List<PostResponseDTO>();
                foreach (int x in a) { bannar.Add(posts[x]); }
                ViewBag.Bannar = bannar;
            }
            ViewBag.Newest = posts?.OrderByDescending(p => p.PubDate).Take(7).ToList();
            ViewBag.MostViewed = posts?.OrderByDescending(p => p.View).Take(7).ToList();
            return View();
        }

        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("contactus")]
        public IActionResult ContactUs()
        {
            return View();
        }

        [Route("newsletter")]
        public IActionResult NewsLetter()
        {
            return View();
        }

        [Route("Error")]
        public IActionResult Error()
        {
            return View();
        }

        [Route("NotFound")]
        [Route("{a}/{b?}/{c?}/{d?}/{e?}/{f?}/{g?}")]
        public IActionResult NotFoundError()
        {
            return View();
        }
    }
}
