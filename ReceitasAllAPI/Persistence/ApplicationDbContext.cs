using Microsoft.EntityFrameworkCore;
using ReceitasAllAPI.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ReceitasAllAPI.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuração de relacionamentos ou restrições adicionais
        }
    }
}


//public class ApplicationDbContext : DbContext
//{
//    public DbSet<Author> Authors { get; set; }
//    //public DbSet<Recipe> Recipes { get; set; }

//    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//    : base(options)
//    {
//    }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);
//        // Configuração de relacionamentos ou restrições adicionais
//    }
//}
