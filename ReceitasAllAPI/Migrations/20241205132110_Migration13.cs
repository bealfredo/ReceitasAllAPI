// Decompiled with JetBrains decompiler
// Type: ReceitasAllAPI.Migrations.Migration13
// Assembly: ReceitasAllAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9B75B2E0-CFCF-4AB8-8B7D-738B515A4450
// Assembly location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.dll
// XML documentation location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.xml

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using ReceitasAllAPI.Persistence;
using System;

#nullable enable
namespace ReceitasAllAPI.Migrations
{
  /// <inheritdoc />
  [DbContext(typeof (ApplicationDbContext))]
  [Migration("20241205132110_Migration13")]
  public class Migration13 : Migration
  {
    /// <inheritdoc />
    protected override void Up(
    #nullable disable
    MigrationBuilder migrationBuilder)
    {
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }

    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
      modelBuilder.HasAnnotation("ProductVersion", (object) "9.0.0").HasAnnotation("Relational:MaxIdentifierLength", (object) 128);
      modelBuilder.UseIdentityColumns();
      modelBuilder.Entity("ReceitasAllAPI.Entities.Author", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("ID").ValueGeneratedOnAdd().HasColumnType<int>("int");
        b.Property<int>("ID").UseIdentityColumn<int>();
        b.Property<bool>("Admin").HasColumnType<bool>("bit");
        b.Property<string>("Bibliography").IsRequired(true).HasMaxLength(500).HasColumnType<string>("nvarchar(500)");
        b.Property<string>("EmailContact").IsRequired(true).HasColumnType<string>("nvarchar(max)");
        b.Property<string>("FirstName").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.Property<string>("Image").IsRequired(true).HasColumnType<string>("nvarchar(max)");
        b.Property<string>("LastName").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.Property<string>("Nacionality").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.Property<string>("PasswordHash").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.Property<string>("Pseudonym").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.Property<string>("UserName").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.HasKey("ID");
        b.ToTable("Authors");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.FavoriteRecipe", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("ID").ValueGeneratedOnAdd().HasColumnType<int>("int");
        b.Property<int>("ID").UseIdentityColumn<int>();
        b.Property<int>("AuthorId").HasColumnType<int>("int");
        b.Property<DateTime>("DateAdded").HasColumnType<DateTime>("datetime2");
        b.Property<int>("RecipeId").HasColumnType<int>("int");
        b.HasKey("ID");
        b.HasIndex("AuthorId");
        b.HasIndex("RecipeId");
        b.ToTable("FavoriteRecipes");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Ingredient", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("ID").ValueGeneratedOnAdd().HasColumnType<int>("int");
        b.Property<int>("ID").UseIdentityColumn<int>();
        b.Property<int>("Order").HasColumnType<int>("int");
        b.Property<int>("RecipeId").HasColumnType<int>("int");
        b.Property<string>("Value").IsRequired(true).HasMaxLength(500).HasColumnType<string>("nvarchar(500)");
        b.HasKey("ID");
        b.HasIndex("RecipeId");
        b.ToTable("Ingredients");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Recipe", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("ID").ValueGeneratedOnAdd().HasColumnType<int>("int");
        b.Property<int>("ID").UseIdentityColumn<int>();
        b.Property<string>("AccentColor").IsRequired(true).HasColumnType<string>("nvarchar(max)");
        b.Property<int>("AuthorId").HasColumnType<int>("int");
        b.Property<DateTime>("DateAdded").HasColumnType<DateTime>("datetime2");
        b.Property<DateTime>("DateUpdated").HasColumnType<DateTime>("datetime2");
        b.Property<string>("Description").IsRequired(true).HasMaxLength(500).HasColumnType<string>("nvarchar(500)");
        b.Property<int>("Difficulty").HasColumnType<int>("int");
        b.Property<string>("Image").IsRequired(true).HasColumnType<string>("nvarchar(max)");
        b.Property<bool>("IsPrivate").HasColumnType<bool>("bit");
        b.Property<int>("PreparationTimeInMinutes").HasColumnType<int>("int");
        b.Property<string>("Rendimento").IsRequired(true).HasColumnType<string>("nvarchar(max)");
        b.Property<string>("Title").IsRequired(true).HasMaxLength(50).HasColumnType<string>("nvarchar(50)");
        b.HasKey("ID");
        b.HasIndex("AuthorId");
        b.ToTable("Recipes");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Step", (Action<EntityTypeBuilder>) (b =>
      {
        b.Property<int>("ID").ValueGeneratedOnAdd().HasColumnType<int>("int");
        b.Property<int>("ID").UseIdentityColumn<int>();
        b.Property<int>("Order").HasColumnType<int>("int");
        b.Property<int>("RecipeId").HasColumnType<int>("int");
        b.Property<string>("Value").IsRequired(true).HasMaxLength(500).HasColumnType<string>("nvarchar(500)");
        b.HasKey("ID");
        b.HasIndex("RecipeId");
        b.ToTable("Steps");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.FavoriteRecipe", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("ReceitasAllAPI.Entities.Author", "Author").WithMany("FavoriteRecipes").HasForeignKey("AuthorId").OnDelete(DeleteBehavior.NoAction).IsRequired();
        b.HasOne("ReceitasAllAPI.Entities.Recipe", "Recipe").WithMany().HasForeignKey("RecipeId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Author");
        b.Navigation("Recipe");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Ingredient", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("ReceitasAllAPI.Entities.Recipe", "Recipe").WithMany("Ingredients").HasForeignKey("RecipeId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Recipe");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Recipe", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("ReceitasAllAPI.Entities.Author", "Author").WithMany("Recipes").HasForeignKey("AuthorId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Author");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Step", (Action<EntityTypeBuilder>) (b =>
      {
        b.HasOne("ReceitasAllAPI.Entities.Recipe", "Recipe").WithMany("Steps").HasForeignKey("RecipeId").OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.Navigation("Recipe");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Author", (Action<EntityTypeBuilder>) (b =>
      {
        b.Navigation("FavoriteRecipes");
        b.Navigation("Recipes");
      }));
      modelBuilder.Entity("ReceitasAllAPI.Entities.Recipe", (Action<EntityTypeBuilder>) (b =>
      {
        b.Navigation("Ingredients");
        b.Navigation("Steps");
      }));
    }
  }
}
