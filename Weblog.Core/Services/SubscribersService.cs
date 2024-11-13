using Microsoft.AspNetCore.Mvc;
using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.RepositoryContracts;
using Weblog.Core.ServiceContracts;

namespace Weblog.Core.Services
{
    public class SubscribersService : ISubscribersService
    {
        private readonly ISubscribersRepository _subscribersRepistory;

        public SubscribersService(ISubscribersRepository subscribersRepistory) { _subscribersRepistory = subscribersRepistory; }

        public async Task<int> AddSubscriber(Subscriber subscriber, IEmailService _emailService)
        {
            Random random = new Random();
            subscriber.VerificationCode = random.Next(10000, 99999);
            subscriber.Verified = false;
            int x = await _subscribersRepistory.AddSubscriber(subscriber);
            if (x > 0)
            {
                await SendVerificationCode(subscriber.Email!, subscriber.VerificationCode.Value, _emailService);
            }
            return x;
        }

        public async Task<int> DeleteSubscriber(string email)
        {
            return await _subscribersRepistory.DeleteSubscriber(email);
        }

        public async Task<Subscriber?> GetSubscriber(string email)
        {
            return await _subscribersRepistory.GetSubscriber(email);
        }

        public async Task<List<Subscriber>?> GetSubscribers()
        {
            return await _subscribersRepistory.GetSubscribers();
        }

        public async Task SendVerificationCode(string email, int code, [FromServices] IEmailService _emailService)
        {
            string body = $"<div dir='rtl' style='border: 2px solid; border-radius: 10px; padding: 10px; margin: 10px; font-family: Arial; font-size: 16px; background-color: #45cabf;'>با سلام<br />کد تایید عضویت شما در خبرنامه‌ی <a href='https://royayeravi.ir/'>رویای راوی</a>: {code}<br /><br />اگر شما جهت عضویت اقدام نکرده اید این پیام را نادیده بگیرید.";
            await _emailService.SendEmailAsync(email, "تایید عضویت در خبرنامه", body);
        }

        public async Task<int> VerifySubscriber(Subscriber subscriber)
        {
            Subscriber? _subscriber = await _subscribersRepistory.GetSubscriber(subscriber.Email!);
            if (_subscriber != null)
            {
                if (_subscriber.VerificationCode == subscriber.VerificationCode)
                {
                    _subscriber.Verified = true;
                    int x = await _subscribersRepistory.UpdateSubscriber(_subscriber);
                    return x;
                }
                return -1;
            }
            return -2;
        }
    }
}
