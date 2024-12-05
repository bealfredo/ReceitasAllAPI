using Microsoft.EntityFrameworkCore;
using ReceitasAllAPI.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace ReceitasAllAPI.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
        public DbSet<Cookbook> Cookbooks { get; set; }
        public DbSet<RecipeCookbook> RecipeCookbooks { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuração de relacionamentos ou restrições adicionais

            // não apagar o author ou a receita quando um favorito é apagado, no action
            modelBuilder.Entity<FavoriteRecipe>()
                .HasOne(fr => fr.Author)
                .WithMany(a => a.FavoriteRecipes)
                .OnDelete(DeleteBehavior.NoAction);

            // ao apagar recipecoookbook não apagar a receita ou o cookbook
            modelBuilder.Entity<RecipeCookbook>()
                .HasOne(rc => rc.Recipe)
                .WithMany(r => r.RecipeCookbooks)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RecipeCookbook>()
                .HasOne(rc => rc.Cookbook)
                .WithMany(c => c.RecipeCookbooks)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}