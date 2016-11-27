using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApiTokenAuth.Models;

namespace WebApiTokenAuth.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public ApplicationDbContext() : base("name=ApplicationDbContext")
        {
        }
    }
}