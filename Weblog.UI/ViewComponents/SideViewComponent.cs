using Microsoft.AspNetCore.Mvc;
using Weblog.Core.DTO;
using Weblog.Core.ServiceContracts;

namespace Weblog.UI.ViewComponents
{
	public class SideViewComponent : ViewComponent
	{
		private readonly IPostsService _postsService;
		public SideViewComponent(IPostsService postsService) { _postsService = postsService; }
		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<PostResponseDTO>? posts = await _postsService.GetPosts();
			posts = posts?.Where(p => p.Published == true).ToList();
			var p1 = posts?.OrderByDescending(p => p.PubDate).ToList();
			ViewBag.Newest = p1?.Take(3).ToList();
			var p2 = posts?.OrderByDescending(p => p.View).ToList();
			ViewBag.MostViewed = p2?.Take(3).ToList();
			return View();
		}
	}
}
