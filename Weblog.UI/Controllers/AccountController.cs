using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Weblog.Core.Domain.IdentityEntities;
using Weblog.Core.DTO;
using Weblog.Core.ServiceContracts;
using System.Diagnostics.Eventing.Reader;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace Weblog.UI.Controllers
{
	[Route("[controller]")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}
		
		[HttpGet]
		[AllowAnonymous]
		[Route("[action]")]
		public IActionResult Register()
		{
			if (User.Identity?.IsAuthenticated == true) { return RedirectToAction("Panel", "Account"); }
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("registeras")]
		public IActionResult RegisterAs()
		{
            if (User.Identity?.IsAuthenticated == true) { return RedirectToAction("Panel", "Account"); }
            return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("[action]")]
		public async Task<IActionResult> Register(UserRegisterDTO user, [FromServices] IFilesService _filesService, [FromQuery] bool special = false)
		{
			if (!ModelState.IsValid) { return BadRequest(); }

			ApplicationUser? tuser = await _userManager.FindByEmailAsync(user.Email!);
			if (tuser != null)
			{
				ViewBag.Duplicated = "ایمیل وارده تکراری است";
				if (special) { return View("RegisterAs", user); }
				else return View(user);
			}

			if (!user.Profile!.ContentType.Contains("jpeg"))
			{
				ViewBag.FormatError = "تصویر باید در فرمت jpg باشد";
				if (special) { return View("RegisterAs", user); }
				else { return View(user); }
			}

			if (special)
			{
				if (user.Level == 1)
				{
					var role = await _roleManager.FindByNameAsync("Author");
					if (role!.Password != user.LevelPass)
					{
						ViewBag.WrongLevelPass = "کد دسترسی اشتباه است";
						return View("RegisterAs", user);
					}
				} else if (user.Level == 2)
				{
					var role = await _roleManager.FindByNameAsync("Admin");
					if (role!.Password != user.LevelPass)
					{
						ViewBag.WrongLevelPass = "کد دسترسی اشتباه است";
						return View("RegisterAs", user);
					}
				}
			}

			Guid id = Guid.NewGuid();
			ApplicationUser _user = new ApplicationUser() { Id = id, UserName = user.Email, Email = user.Email, Name = user.Name, Bio = user.Bio };
			IdentityResult result = await _userManager.CreateAsync(_user, user.Password!);
			if (!result.Succeeded) { throw new Exception(); }
			await _filesService.SavePic(user.Profile, "users", id.ToString() + ".jpg");

			var suser = await _userManager.FindByEmailAsync(user.Email!);
			if (special)
			{
				if (user.Level == 1) await _userManager.AddToRoleAsync(suser!, "Author");
				if (user.Level == 2) await _userManager.AddToRoleAsync(suser!, "Admin");
			}
			await _signInManager.SignInAsync(_user, true);

			return RedirectToAction("Panel", "Account");
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("[action]")]
		public IActionResult SignIn()
		{
			if (User.Identity?.IsAuthenticated == true) { return RedirectToAction("Panel", "Account"); }
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("[action]")]
		public async Task<IActionResult> SignIn(UserSigninDTO user)
		{
			if (!ModelState.IsValid) { return BadRequest(); }
			var _user = await _userManager.FindByEmailAsync(user.Email!);
			if (_user == null)
			{
				ViewBag.NotFound = "ایمیل وارده یافت نشد";
				return View(user);
			}
			Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(_user, user.Password!, user.RememberMe, false);
			if (result.Succeeded) { return RedirectToAction("Panel", "Account"); } else
			{
				ViewBag.WrongPassword = "گذرواژه اشتباه است";
				return View(user);
			}
		}

		[Route("[action]")]
		public async Task<IActionResult> Signout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[Route("[action]")]
		public async Task<IActionResult> Panel([FromServices] IPostsService _postsService)
		{
			if (User.Identity?.IsAuthenticated != true) { return RedirectToAction("SignIn", "Account"); }
			var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
			var _user = user!.ToUserResponse();
			if (User.IsInRole("Author"))
			{
				var posts = await _postsService.GetPosts();
				if (posts != null && posts.Count != 0)
				{
					ViewBag.Posts = posts.Where(p => p.AuthorId == user!.Id).OrderByDescending(p => p.PubDate).ToList();
                }
				else { ViewBag.Posts = null; }
				return View("PanelAuthor", _user);
			}
			return View(_user);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> Edit()
		{
            if (User.Identity?.IsAuthenticated != true) { return RedirectToAction("SignIn", "Account"); }
            var user = await _userManager.FindByEmailAsync(User.Identity!.Name!);
			UserUpdateDTO _user = new UserUpdateDTO() { Name = user!.Name, Email = user.Email, Bio = user.Bio };
			return View(_user);
        }

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> Edit(UserUpdateDTO user, [FromServices] IFilesService _filesService)
		{
			if (!ModelState.IsValid) { return BadRequest(); }
			
			if (user.Profile != null && !user.Profile!.ContentType.Contains("jpeg"))
			{
				ViewBag.FormatError = "تصویر باید در فرمت jpg باشد";
				return View(user);
			}

			bool so = false;
			var _user = await _userManager.FindByEmailAsync(User.Identity?.Name!);
			if (_user == null) { return RedirectToAction("SignIn", "Account"); }
			if (user.Email != User.Identity?.Name)
			{
                ApplicationUser? tuser = await _userManager.FindByEmailAsync(user.Email!);
				if (tuser != null) { ViewBag.Duplicated = "ایمیل وارده تکراری است"; return View(user); }
                _user!.Email = user.Email;
				_user!.UserName = user.Email;
				so = true;
			}
			_user!.Name = user.Name;
			_user.Bio = user.Bio;
			IdentityResult result = await _userManager.UpdateAsync(_user);
			if (!result.Succeeded) { throw new Exception(); }
			if (user.Profile != null)
			{
				await _filesService.SavePic(user.Profile, "users", _user.Id.ToString() + ".jpg");
			}
			if (so) { await _signInManager.SignOutAsync(); return RedirectToAction("SignIn", "Account"); }
			return RedirectToAction("Panel", "Account");
		}

		[HttpGet]
		[Route("[action]")]
		public IActionResult EditPassword()
		{
			if (User.Identity?.IsAuthenticated != true) { return RedirectToAction("SignIn", "Account"); }
			return View();
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> EditPassword(UserUpPassDTO user)
		{
			if (!ModelState.IsValid) { return BadRequest(); }
			var _user = await _userManager.FindByEmailAsync(User!.Identity!.Name!);
			if (_user == null) { return BadRequest(); }
			IdentityResult result = await _userManager.ChangePasswordAsync(_user, user.CurrentPass!, user.Password!);
			if (!result.Succeeded)
			{
				ViewBag.WrongPass = "گذرواژه اشتباه است";
				return View();
			}
			return RedirectToAction("Panel", "Account");
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("[action]")]
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ForgetPassword(string email, [FromServices] IEmailService _emailService)
		{
			var user = await _userManager.FindByNameAsync(email);
			if (user == null)
			{
                ViewBag.NotFound = "ایمیل وارده یافت نشد";
				return View();
            }
			string Token = await _userManager.GeneratePasswordResetTokenAsync(user);
			string message = $"<div dir='rtl' style='border: 2px solid; border-radius: 10px; padding: 10px; margin: 10px; font-family: Arial; font-size: 16px; background-color: #45cabf;'>با سلام<br />کد بازنشانی گذرواژه:<br /><div dir='ltr'>{Token}</div><br /><br />اگر شما جهت بازنشانی اقدام نکرده اید این پیام را نادیده بگیرید.";
			await _emailService.SendEmailAsync(email, "بازنشانی گذرواژه", message);
			UserResetPassDTO _user = new UserResetPassDTO() { Email = email };
			return View("ResetPassword", _user);
        }

		[AllowAnonymous]
		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ResetPassword(UserResetPassDTO user)
		{
			if (!ModelState.IsValid) { return BadRequest(); }

			var _user = await _userManager.FindByEmailAsync(user.Email!);
			if (_user == null) { return BadRequest(); }
			IdentityResult result = await _userManager.ResetPasswordAsync(_user!, user.Token!, user.Password!);
			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(_user, true);
				return RedirectToAction("Panel", "Account");
			}
			ViewBag.WrongCode = "کد بازنشانی اشتباه است";
			return View(user);
        }
	}
}
