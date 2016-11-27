using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApiTokenAuth.Models;

namespace WebApiTokenAuth.DAL
{
    public class AuthRepository : IDisposable
    {
        private ApplicationDbContext _context;


        public AuthRepository()
        {
            _context = new ApplicationDbContext();
        }

        public bool RegisterUser(User user)
        {
            user = _context.Users.Add(user);
            _context.SaveChanges();
            return (user != null) && user.Id != 0;
        }

        public User FindUser(User user)
        {
            return _context.Users.FirstOrDefault(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password));
        }

        public Client FindClient(string clientId)
        {
            var client = _context.Clients.Find(clientId);
            return client;
        }

        public bool AddRefreshToken(RefreshToken refreshToken)
        {
            var existingToken = _context.RefreshTokens.FirstOrDefault(t => t.ClientId == refreshToken.ClientId);

            if (existingToken != null)
            {
                var result = RemoveRefreshToken(existingToken);
            }
            _context.RefreshTokens.Add(refreshToken);
            return _context.SaveChanges() > 0;
        }

        public bool RemoveRefreshToken(int refreshTokenId)
        {
            var refreshToken = _context.RefreshTokens.Find(refreshTokenId);

            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                return _context.SaveChanges() > 0;
            }
            return false;
        }

        public bool RemoveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            return _context.SaveChanges() > 0;
        }

        public bool RemoveRefreshToken(string refreshToken)
        {
            _context.RefreshTokens.Remove(_context.RefreshTokens.FirstOrDefault(x => x.Token.Equals(refreshToken)));
            return _context.SaveChanges() > 0;
        }

        public RefreshToken FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = _context.RefreshTokens.FirstOrDefault(x => x.Token.Equals(refreshTokenId));

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _context.RefreshTokens.ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}