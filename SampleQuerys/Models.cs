using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleQuerys
{
    public class MoviesContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder buider)
            => buider.UseSqlite(@"Data source=C:\DB\movies.sqlite");
    }

    public class Category
    {
        public int Id { get; set; }

        [StringLength(35)]
        public string Name { get; set; }
    }

    public class Movie
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        public Category Category { get; set; }

        public virtual ICollection<UserReview> UserReviews { get; set; }
    }

    public class User
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
        public virtual ICollection<UserReview> UserReviews { get; set; }
    }

    public class UserReview
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public DateTime ReviewDate { get; set; }
        [Range(0,5)]
        public int Rating { get; set; }
        [StringLength(1000)]
        public string Review { get; set; }

        public Movie Movie { get; set; }
        public User User { get; set; }
    }
}
