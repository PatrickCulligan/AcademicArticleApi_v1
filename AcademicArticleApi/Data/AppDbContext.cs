using Microsoft.EntityFrameworkCore;
using AcademicArticleApi.Models;

namespace AcademicArticleApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AcademicArticle> AcademicArticles { get; set; }
    }
}
