using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTokenAuth.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum ApplicationType
    {
        JavascriptNonConfidential = 0,
        NativeConfidential = 1
    }
    public class Client
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string ApplicationName { get; set; }
        public ApplicationType ApplicationType { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }

    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiryUtc { get; set; }
        public string ProtectedTicket { get; set; }
    }
}