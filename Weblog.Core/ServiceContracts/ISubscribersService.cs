using Microsoft.AspNetCore.Mvc;
using Weblog.Core.Domain.Entities;

namespace Weblog.Core.ServiceContracts
{
    public interface ISubscribersService
    {
        Task<int> AddSubscriber(Subscriber subscriber, [FromServices] IEmailService _emailService);

        Task<Subscriber?> GetSubscriber(string email);

        Task<List<Subscriber>?> GetSubscribers();

        Task<int> DeleteSubscriber(string email);

        Task SendVerificationCode(string email, int code, [FromServices] IEmailService _emailsService);

        Task<int> VerifySubscriber(Subscriber subscriber);
    }
}
