using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleQuerys
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create categories
            AddCategories();
            AddMovies();
            AddUsersAndReviews();
            AddAutoReview();
            PrintMovies();
        }

        static void AddCategories()
        {
            using (var context = new MoviesContext())
            {
                Console.WriteLine("Registrando información de categorias");
                // Check if any category exist
                if (context.Categories.Any())
                {
                    Console.WriteLine("Carga completa");
                    return;
                }

                // Create
                context.Categories.Add(new Category
                {
                    Name = "Terror"
                });

                context.Add(new Category
                {
                    Name = "Comedia"
                });

                context.Categories.AddRange(
                    new Category { Name = "Acción" },
                    new Category { Name = "Animada" }
                );

                context.AddRange(
                    new Category { Name = "Clásicos" },
                    new Category { Name = "Aventura" }
                );
                context.AddRange(new List<Category>
                {
                    new Category { Name = "Ciencia ficción" },
                    new Category { Name = "Suspenso" }
                });

                // Send data to DB
                context.SaveChanges();
                Console.WriteLine("Carga completa");
            }
        }

        static void AddMovies()
        {
            using (var context = new MoviesContext())
            {
                Console.WriteLine("Registrando información de películas");
                // Check if any movie exist
                if (context.Movies.Any())
                {
                    Console.WriteLine("Carga completa");
                    return;
                }

                var categories = context.Categories.ToList();

                context.AddRange(new List<Movie>
                {
                    new Movie{ Name = "Película 1", CategoryId = 1 },
                    new Movie{ Name = "Película 2", CategoryId = 2 },
                    new Movie{ Name = "Película 3", Category = categories.First() },
                    new Movie{ Name = "Película 4", Category = categories.Last() },
                    new Movie{ Name = "Película 5", Category = categories.First(x => x.Name.Contains("Comedia")) },
                    new Movie{ Name = "Película 6", Category = categories.Last(x => x.Name.Contains("ión")) },
                    new Movie{ Name = "Película 7", Category = categories.Where(x => x.Id > 3).First() },
                });

                context.SaveChanges();
                Console.WriteLine("Carga completa");
            }
        }

        static void AddUsersAndReviews()
        {
            using (var context = new MoviesContext())
            {
                Console.WriteLine("Registrando información de los Usuarios");
                // Check if any user exist
                if (context.Users.Any())
                {
                    Console.WriteLine("Carga completa");
                    return;
                }

                context.AddRange(new List<User>
                {
                    new User{ Name = "Usuario 1", UserReviews = new List<UserReview>{ new UserReview { MovieId = 1, Rating = 1, Review = "Revisión 1", ReviewDate = DateTime.Now }}},
                    new User{ Name = "Usuario 2", UserReviews = new List<UserReview>{ new UserReview { MovieId = 2, Rating = 2, Review = "Revisión 2", ReviewDate = new DateTime(2020, 11, 22) }}},
                    new User{ Name = "Usuario 3", UserReviews = new List<UserReview>{ new UserReview { MovieId = 3, Rating = 3, Review = "Revisión 3", ReviewDate = DateTime.Now.Date }}},
                    new User{ Name = "Usuario 4", UserReviews = new List<UserReview>{ new UserReview { MovieId = 4, Rating = 4, Review = "Revisión 4", ReviewDate = DateTime.Now.AddDays(-5) }}},
                    new User{ Name = "Usuario 5", UserReviews = new List<UserReview>{ new UserReview { MovieId = 5, Rating = 5, Review = "Revisión 5", ReviewDate = DateTime.Now.AddMonths(-5) }}},
                    new User{ Name = "Usuario 6", UserReviews = new List<UserReview>{ new UserReview { MovieId = 6, Rating = 1, Review = "Revisión 6", ReviewDate = DateTime.Now.AddYears(-5) }}},
                    new User{ Name = "Usuario 7", UserReviews = new List<UserReview>{ new UserReview { MovieId = 7, Rating = 3, Review = "Revisión 7", ReviewDate = DateTime.Now.AddYears(1) }}}
                });

                context.SaveChanges();
                Console.WriteLine("Carga completa");
            }
        }

        static void AddAutoReview()
        {
            using (var context = new MoviesContext())
            {
                Console.WriteLine("Generando comentario de prueba");
                // Check if any movie exist
                if (!context.Users.Any(x => x.Id == 1))
                {
                    Console.WriteLine("No se puede completar la acción");
                    return;
                }

                var random = new Random(); // Aleatory number object
                var movieId = random.Next(1, 7); // Aleatory number between 1-7
                var rating = random.Next(1, 5);  // Aleatory number between 1-5

                context.Add(new UserReview
                {
                    UserId = 1,
                    MovieId = movieId,
                    Rating = rating,
                    ReviewDate = DateTime.Now,
                    // Interpolación de cadenas -> $""
                    Review = $"Aleatorio de la película {movieId} - rating {rating}"
                });

                context.SaveChanges();
                Console.WriteLine("Registro completo");
            }
        }

        static void PrintMovies()
        {
            using (var context = new MoviesContext())
            {
                Console.WriteLine("Registrando información de películas");
                // Check if any movie exist
                if (!context.Movies.Any())
                {
                    Console.WriteLine("Sin películas aún");
                    return;
                }

                var movies = context.Movies
                    .Include(x => x.UserReviews)
                    .ThenInclude(x => x.User)
                    .Include(x => x.Category)
                    .OrderBy(x => x.Name)
                    .ToList();

                Console.WriteLine("");
                Console.WriteLine(" == Listado de películas ==");
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var movie in movies)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine($"Nombre: {movie.Name}");
                    Console.WriteLine($"Comentarios ======");

                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    foreach (var comment in movie.UserReviews)
                    {
                        Console.WriteLine($"Fecha: {comment.ReviewDate} - Usuario: {comment.User.Name} - Rating: {comment.Rating}");
                        Console.WriteLine(comment.Review + "\n");
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("== FIN ==");
            }
        }
    }
}
