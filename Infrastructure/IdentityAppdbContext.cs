using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Core.Domain;

namespace Infrastructure
{
   public class IdentityAppdbContext : IdentityDbContext<User>
    {

        public IdentityAppdbContext(DbContextOptions<IdentityAppdbContext> options) : base(options){}
        public override DbSet<User> Users { get; set; }

    }
}
