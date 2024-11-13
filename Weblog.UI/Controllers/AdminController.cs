using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Core.ServiceContracts;

namespace Weblog.UI.Controllers
{
	[Route("[controller]")]
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		
		public AdminController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index([FromServices] IPostsService _postsService)
		{
			List<ApplicationRole> roles = _roleManager.Roles.ToList();
			ViewBag.Roles = roles;
			List<ApplicationUser> users = _userManager.Users.ToList();
			Dictionary<ApplicationUser, List<string>> data = new Dictionary<ApplicationUser, List<string>>();
			foreach (ApplicationUser user in users)
			{
				data.Add(user, (await _userManager.GetRolesAsync(user)).ToList());
			}
			ViewBag.Data = data;
			ViewBag.Posts = (await _postsService.GetPosts())?.OrderByDescending(p => p.PubDate);
			return View();
		}

		[Route("[action]/{id}")]
		[HttpGet]
		public async Task<IActionResult> EditRole(Guid id, [FromServices] ILogger<AdminController> _logger)
		{
			ApplicationRole? role = await _roleManager.FindByIdAsync(id.ToString());
			if (role == null)
			{
				_logger.LogWarning("Role not found: " + id.ToString());
				return RedirectToAction("NotFoundError", "Home");
			}
			return View(role);
		}

		[Route("[action]/{id}")]
		[HttpPost]
		public async Task<IActionResult> EditRole(Guid id, string password, [FromServices] ILogger<AdminController> _logger)
		{
			ApplicationRole? role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                _logger.LogWarning("Role not found: " + id.ToString());
                return RedirectToAction("NotFoundError", "Home");
            }
			if (password == null || password.Length < 8) { return BadRequest(); }
			role.Password = password;
			IdentityResult result = await _roleManager.UpdateAsync(role);
			if (result.Succeeded) { return RedirectToAction("Index", "Admin"); }
			else { throw new Exception(); }
        }

		[Route("[action]/{id}")]
		public async Task<IActionResult> EditUserRole(Guid id, [FromQuery] int? action, [FromServices] ILogger<AdminController> _logger)
		{
			ApplicationUser? user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User not found: " + id.ToString());
                return RedirectToAction("NotFoundError", "Home");
            }
			if (action == 1)
			{
				IdentityResult result = await _userManager.AddToRoleAsync(user, "Author");
                if (result.Succeeded) { return RedirectToAction("Index", "Admin"); }
                else { throw new Exception(); }
            }
            else if (action == 2)
            {
                IdentityResult result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded) { return RedirectToAction("Index", "Admin"); }
                else { throw new Exception(); }
            }
            else if (action == -1)
            {
				IdentityResult result = await _userManager.RemoveFromRoleAsync(user, "Author");
                if (result.Succeeded) { return RedirectToAction("Index", "Admin"); }
                else { throw new Exception(); }
            }
			else if (action == -2)
			{
				IdentityResult result = await _userManager.AddToRoleAsync(user, "Author");
                if (result.Succeeded) { return RedirectToAction("Index", "Admin"); }
                else { throw new Exception(); }
            }
            else
            {
				return BadRequest();
            }
        }
	}
}
