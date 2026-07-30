using FileOrganization_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace FileOrganization_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<OrganizeLog> OrganizeLogs { get; set; }
    }
}
