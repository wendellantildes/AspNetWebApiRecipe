using Microsoft.EntityFrameworkCore;
using AspNetWebApiRecipe.Models;


namespace AspNetWebApiRecipe.Models
{
    public partial class AspNetWebApiRecipeContext : DbContext
    {
        public virtual DbSet<Person> Pessoas { get; set; }

        public AspNetWebApiRecipeContext(DbContextOptions options) : base(options){
            
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}