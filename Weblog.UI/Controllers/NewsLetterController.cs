using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Weblog.Core.Domain.Entities;
using Weblog.Core.ServiceContracts;
using Weblog.Core.Services;

namespace Weblog.UI.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class NewsLetterController : Controller
    {
        private readonly ISubscribersService _subscribersService;

        public NewsLetterController (ISubscribersService subscribersService) { _subscribersService = subscribersService; }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Register(Subscriber subscriber, [FromServices] IEmailService _emailService)
        {
            if (subscriber.Email == null) { return BadRequest(); }

            Subscriber? _subscriber = await _subscribersService.GetSubscriber(subscriber.Email);
            if (_subscriber != null && _subscriber.Verified == true)
            {
                ViewBag.Duplicated = "شما قبلا در خبرنامه عضو شده اید";
                return View("NewsLetter");
            }
            else if (_subscriber != null && _subscriber.Verified == false)
            {
                await _subscribersService.SendVerificationCode(_subscriber.Email!, _subscriber.VerificationCode!.Value, _emailService);
            }
            else
            {
                int y = await _subscribersService.AddSubscriber(subscriber, _emailService);
                if (y == 0) { throw new Exception("AddSubscriber failed."); }
                if (y == -1) { throw new Exception("Send verification code failed."); }
            }

            return View(subscriber);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Verify(Subscriber subscriber)
        {
            if (subscriber.Email == null || subscriber.VerificationCode == null) { return BadRequest(); }
            int x = await _subscribersService.VerifySubscriber(subscriber);
            if (x == -2) { throw new Exception("Received email for verifing not found: " + subscriber.Email); }
            if (x == -1)
            {
                ViewBag.WrongCode = "کد وارده صحیح نیست";
                return View("Register");
            }
            if (x == 0) { throw new Exception("Verifing email failed."); }
            return View();
        }

        [Route("[action]")]
        public async Task<IActionResult> Cancel([FromQuery] string email)
        {
            if (email == null) { return BadRequest(); }

            int x = await _subscribersService.DeleteSubscriber(email);
            if (x == -1) { return RedirectToAction("NotFoundError", "Home"); }

            if (x == 0)
            {
                throw new Exception("Canceling Subscription failed: " + email);
            }

            return View();
        }
    }
}
