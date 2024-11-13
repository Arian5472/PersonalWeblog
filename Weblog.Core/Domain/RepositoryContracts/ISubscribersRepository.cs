using Weblog.Core.Domain.Entities;

namespace Weblog.Core.Domain.RepositoryContracts
{
    public interface ISubscribersRepository
    {
        Task<int> AddSubscriber(Subscriber subscriber);

        Task<Subscriber?> GetSubscriber(string email);

        Task<List<Subscriber>?> GetSubscribers();

        Task<int> UpdateSubscriber(Subscriber subscriber);

        Task<int> DeleteSubscriber(string email);
    }
}
