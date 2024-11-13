using Microsoft.EntityFrameworkCore;
using Weblog.Core.Domain.Entities;
using Weblog.Core.Domain.RepositoryContracts;
using Weblog.Infrastructure.DbContext;

namespace Weblog.Infrastructure.Repositories
{
    public class SubscribersRepository : ISubscribersRepository
    {
        private readonly ApplicationDbContext _db;

        public SubscribersRepository(ApplicationDbContext db) { _db = db; }

        public async Task<int> AddSubscriber(Subscriber subscriber)
        {
            _db.Subscribers.Add(subscriber);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> DeleteSubscriber(string email)
        {
            Subscriber? subscriber = await _db.Subscribers.FirstOrDefaultAsync(x => x.Email == email);
            if (subscriber != null)
            {
                _db.Subscribers.Remove(subscriber);
                return await _db.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<int> UpdateSubscriber(Subscriber subscriber)
        {
            Subscriber? _subscriber = await _db.Subscribers.FirstOrDefaultAsync(s => s.Email == subscriber.Email);
            if (_subscriber != null)
            {
                _subscriber.Verified = subscriber.Verified;
                return await _db.SaveChangesAsync();
            }
            else return 0;
        }

        public async Task<List<Subscriber>?> GetSubscribers()
        {
            return await _db.Subscribers.Where(s => s.Verified == true).ToListAsync();
        }

        public async Task<Subscriber?> GetSubscriber(string email)
        {
            return await _db.Subscribers.FirstOrDefaultAsync(subscriber => subscriber.Email == email);
        }
    }
}
