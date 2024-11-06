using BlogService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }

        public DbSet<WebsiteData> WebsiteDatas { get; set; }
        public DbSet<BlogInfo> BlogInfos { get; set; }
        public DbSet<BlogLog> BlogLogs { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogInfo>()
                .HasOne(b => b.WebsiteData)
                .WithMany(w => w.BlogInfos)
                .HasForeignKey(b => b.WebsiteId);

            modelBuilder.Entity<BlogLog>()
                .HasOne(b => b.WebsiteData)
                .WithMany()
                .HasForeignKey(b => b.WebsiteId);
        }
    }
}
