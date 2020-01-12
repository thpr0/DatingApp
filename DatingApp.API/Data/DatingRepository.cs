using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _db;

        public DatingRepository(DataContext db)
        {
            _db = db;

        }
        public void add<T>(T entity) where T : class
        {
            _db.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _db.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientID)
        {
            return await _db.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientID);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _db.Photos.Where(u => u.UserID == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _db.Photos.FirstOrDefaultAsync(p => p.ID == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _db.users.Include(u => u.Photos).OrderBy(u => u.LastActive).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _db.users.Include(u => u.Photos).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserID);
            users = users.Where(u => u.gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfbirth >= minDob && u.DateOfbirth <= maxDob);
            }
            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _db.users.Include(u => u.Likers).Include(u => u.Likees).FirstOrDefaultAsync(u => u.Id == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
        }

        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _db.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessageForUser(MessageParams messageParams)
        {

            var messages = _db.Messages.Include(m => m.Sender).ThenInclude(p => p.Photos)
                        .Include(m => m.Recipient).ThenInclude(p => p.Photos).AsQueryable();
            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientID == messageParams.UserID);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserID);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientID == messageParams.UserID && u.IsRead == false);
                    break;

            }

            messages=messages.OrderByDescending(d=>d.DateSend);
            return await PagedList<Message>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);           
        }

        public Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientID)
        {
            throw new NotImplementedException();
        }
    }
}