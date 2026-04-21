using TheatreMs.Api.Models;

namespace TheatreMs.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedUsersAsync(db);
        await SeedMoviesAsync(db);
        await SeedTheatresAsync(db);
        await SeedScreeningsAsync(db);
    }

    private static async Task SeedUsersAsync(AppDbContext db)
    {
        if (db.Users.Any()) return;

        db.Users.AddRange(
            new User
            {
                Username = "admin",
                Email = "ingabireo64@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Admin", LastName = "User",
                Role = UserRole.ROLE_ADMIN
            },
            new User
            {
                Username = "manager",
                Email = "manager@theatrems.com",
                Password = BCrypt.Net.BCrypt.HashPassword("manager123"),
                FirstName = "Manager", LastName = "User",
                Role = UserRole.ROLE_MANAGER
            },
            new User
            {
                Username = "john_doe",
                Email = "john@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                FirstName = "John", LastName = "Doe",
                Role = UserRole.ROLE_USER
            }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedMoviesAsync(AppDbContext db)
    {
        if (db.Movies.Any()) return;

        db.Movies.AddRange(
            new Movie
            {
                Title = "Galactic Odyssey",
                Description = "A crew of astronauts ventures beyond the known universe and discovers a civilisation that challenges everything humanity believes about existence.",
                DurationMinutes = 148,
                Genre = MovieGenre.SCI_FI,
                Director = "James Cameron",
                Cast = "Chris Evans, Zoe Saldana, Oscar Isaac",
                ReleaseDate = new DateOnly(2024, 3, 15),
                Rating = MovieRating.PG13,
                PosterImageUrl = "https://images.unsplash.com/photo-1446776811953-b23d57bd21aa?w=400"
            },
            new Movie
            {
                Title = "The Last Detective",
                Description = "A retired detective is pulled back into action when a series of impossible crimes resurface, each bearing his signature.",
                DurationMinutes = 122,
                Genre = MovieGenre.THRILLER,
                Director = "David Fincher",
                Cast = "Idris Elba, Emily Blunt, Tom Hardy",
                ReleaseDate = new DateOnly(2024, 5, 22),
                Rating = MovieRating.R,
                PosterImageUrl = "https://images.unsplash.com/photo-1485846234645-a62644f84728?w=400"
            },
            new Movie
            {
                Title = "Hearts in Paris",
                Description = "Two strangers meet on a rainy evening in Paris and spend one unforgettable night rediscovering what it means to truly connect.",
                DurationMinutes = 108,
                Genre = MovieGenre.ROMANCE,
                Director = "Sofia Coppola",
                Cast = "Timothée Chalamet, Florence Pugh",
                ReleaseDate = new DateOnly(2024, 2, 14),
                Rating = MovieRating.PG,
                PosterImageUrl = "https://images.unsplash.com/photo-1499856871958-5b9627545d1a?w=400"
            },
            new Movie
            {
                Title = "Dragon's Roar",
                Description = "An elite soldier teams up with an ancient dragon spirit to stop a shadowy organisation from plunging the world into chaos.",
                DurationMinutes = 135,
                Genre = MovieGenre.ACTION,
                Director = "Chad Stahelski",
                Cast = "Dwayne Johnson, Michelle Yeoh, Jason Statham",
                ReleaseDate = new DateOnly(2024, 7, 4),
                Rating = MovieRating.PG13,
                PosterImageUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af9f23?w=400"
            },
            new Movie
            {
                Title = "Laughing Out Loud",
                Description = "A stand-up comedian accidentally becomes the mayor of a small town and must govern while keeping his identity a secret.",
                DurationMinutes = 98,
                Genre = MovieGenre.COMEDY,
                Director = "Judd Apatow",
                Cast = "Kevin Hart, Awkwafina, John Mulaney",
                ReleaseDate = new DateOnly(2024, 6, 10),
                Rating = MovieRating.PG13,
                PosterImageUrl = "https://images.unsplash.com/photo-1495615080073-6b89c9839ce0?w=400"
            },
            new Movie
            {
                Title = "The Haunting of Blackwood",
                Description = "A family moves into a Victorian mansion and slowly uncovers the horrifying truth behind the previous owners' disappearance.",
                DurationMinutes = 114,
                Genre = MovieGenre.HORROR,
                Director = "James Wan",
                Cast = "Lupita Nyong'o, Benedict Cumberbatch",
                ReleaseDate = new DateOnly(2024, 10, 31),
                Rating = MovieRating.R,
                PosterImageUrl = "https://images.unsplash.com/photo-1478760329108-5c3ed9d495a0?w=400"
            },
            new Movie
            {
                Title = "The Wanderer",
                Description = "An ageing traveller crosses three continents on foot, forging unexpected friendships and confronting long-buried regrets.",
                DurationMinutes = 130,
                Genre = MovieGenre.DRAMA,
                Director = "Alfonso Cuarón",
                Cast = "Viola Davis, Anthony Hopkins, Cate Blanchett",
                ReleaseDate = new DateOnly(2024, 9, 5),
                Rating = MovieRating.PG,
                PosterImageUrl = "https://images.unsplash.com/photo-1528360983277-13d401cdc186?w=400"
            },
            new Movie
            {
                Title = "Tiny Worlds",
                Description = "When a group of insects discover their garden is about to be demolished, they band together in a hilarious bid to save their home.",
                DurationMinutes = 94,
                Genre = MovieGenre.ANIMATION,
                Director = "Pete Docter",
                Cast = "Animated Feature",
                ReleaseDate = new DateOnly(2024, 11, 20),
                Rating = MovieRating.G,
                PosterImageUrl = "https://images.unsplash.com/photo-1620336655052-b57986f5a26a?w=400"
            }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedTheatresAsync(AppDbContext db)
    {
        if (db.Theatres.Any()) return;

        var grandCinema = new Theatre
        {
            Name = "Grand Cinema",
            Address = "123 Main Street, City Center",
            PhoneNumber = "+1-555-0100",
            Email = "info@grandcinema.com",
            Description = "Premium cinema experience with state-of-the-art screens and Dolby Atmos sound.",
            TotalScreens = 3,
            ImageUrl = "https://images.unsplash.com/photo-1507676184212-d03ab07a01bf?w=600"
        };

        var starlightMultiplex = new Theatre
        {
            Name = "Starlight Multiplex",
            Address = "456 Sunset Boulevard, Westside",
            PhoneNumber = "+1-555-0200",
            Email = "info@starlightmultiplex.com",
            Description = "The largest multiplex in the region featuring IMAX and 4DX screens.",
            TotalScreens = 4,
            ImageUrl = "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=600"
        };

        db.Theatres.AddRange(grandCinema, starlightMultiplex);
        await db.SaveChangesAsync();

        // Seats for Grand Cinema (3 screens)
        AddSeats(db, grandCinema.Id, screenNumber: 1, rows: 8, seatsPerRow: 10);
        AddSeats(db, grandCinema.Id, screenNumber: 2, rows: 6, seatsPerRow: 8);
        AddSeats(db, grandCinema.Id, screenNumber: 3, rows: 10, seatsPerRow: 12);

        // Seats for Starlight Multiplex (4 screens)
        AddSeats(db, starlightMultiplex.Id, screenNumber: 1, rows: 12, seatsPerRow: 14); // IMAX
        AddSeats(db, starlightMultiplex.Id, screenNumber: 2, rows: 8, seatsPerRow: 10);
        AddSeats(db, starlightMultiplex.Id, screenNumber: 3, rows: 8, seatsPerRow: 10);
        AddSeats(db, starlightMultiplex.Id, screenNumber: 4, rows: 6, seatsPerRow: 8);

        await db.SaveChangesAsync();
    }

    private static void AddSeats(AppDbContext db, long theatreId, int screenNumber, int rows, int seatsPerRow)
    {
        for (int r = 0; r < rows; r++)
        {
            var rowName = ((char)('A' + r)).ToString();
            // Last two rows are PREMIUM, rest are STANDARD
            var seatType = r >= rows - 2 ? SeatType.PREMIUM : SeatType.STANDARD;
            var multiplier = seatType == SeatType.PREMIUM ? 1.5 : 1.0;

            for (int n = 1; n <= seatsPerRow; n++)
            {
                db.Seats.Add(new Seat
                {
                    TheatreId = theatreId,
                    ScreenNumber = screenNumber,
                    RowName = rowName,
                    SeatNumber = n,
                    SeatType = seatType,
                    PriceMultiplier = multiplier
                });
            }
        }
    }

    private static async Task SeedScreeningsAsync(AppDbContext db)
    {
        if (db.Screenings.Any()) return;

        var movies = db.Movies.ToList();
        var theatres = db.Theatres.ToList();

        if (!movies.Any() || !theatres.Any()) return;

        Movie Get(string title) => movies.First(m => m.Title == title);
        Theatre Theatre(string name) => theatres.First(t => t.Name == name);

        var grand = Theatre("Grand Cinema");
        var starlight = Theatre("Starlight Multiplex");

        var today = DateTime.UtcNow.Date;

        var screenings = new List<Screening>
        {
            // ── Grand Cinema, Screen 1 ──────────────────────────────────────
            Screening(Get("Galactic Odyssey"),    grand,     1, today.AddDays(1).AddHours(10), ScreeningFormat.STANDARD,    14.00),
            Screening(Get("Galactic Odyssey"),    grand,     1, today.AddDays(1).AddHours(14), ScreeningFormat.STANDARD,    14.00),
            Screening(Get("Dragon's Roar"),       grand,     1, today.AddDays(1).AddHours(18), ScreeningFormat.DOLBY_ATMOS, 16.00),
            Screening(Get("Dragon's Roar"),       grand,     1, today.AddDays(2).AddHours(10), ScreeningFormat.DOLBY_ATMOS, 16.00),
            Screening(Get("The Last Detective"),  grand,     1, today.AddDays(2).AddHours(15), ScreeningFormat.STANDARD,    13.00),
            Screening(Get("The Last Detective"),  grand,     1, today.AddDays(3).AddHours(19), ScreeningFormat.STANDARD,    13.00),

            // ── Grand Cinema, Screen 2 ──────────────────────────────────────
            Screening(Get("Hearts in Paris"),     grand,     2, today.AddDays(1).AddHours(11), ScreeningFormat.STANDARD,    12.00),
            Screening(Get("Hearts in Paris"),     grand,     2, today.AddDays(2).AddHours(17), ScreeningFormat.STANDARD,    12.00),
            Screening(Get("Laughing Out Loud"),   grand,     2, today.AddDays(1).AddHours(20), ScreeningFormat.STANDARD,    12.00),
            Screening(Get("Laughing Out Loud"),   grand,     2, today.AddDays(3).AddHours(14), ScreeningFormat.STANDARD,    12.00),

            // ── Grand Cinema, Screen 3 ──────────────────────────────────────
            Screening(Get("Tiny Worlds"),         grand,     3, today.AddDays(1).AddHours(10), ScreeningFormat.STANDARD,    10.00),
            Screening(Get("Tiny Worlds"),         grand,     3, today.AddDays(1).AddHours(13), ScreeningFormat.STANDARD,    10.00),
            Screening(Get("Tiny Worlds"),         grand,     3, today.AddDays(2).AddHours(10), ScreeningFormat.STANDARD,    10.00),
            Screening(Get("The Wanderer"),        grand,     3, today.AddDays(2).AddHours(19), ScreeningFormat.STANDARD,    13.00),

            // ── Starlight Multiplex, Screen 1 (IMAX) ───────────────────────
            Screening(Get("Galactic Odyssey"),    starlight, 1, today.AddDays(1).AddHours(12), ScreeningFormat.IMAX,        20.00),
            Screening(Get("Galactic Odyssey"),    starlight, 1, today.AddDays(1).AddHours(16), ScreeningFormat.IMAX,        20.00),
            Screening(Get("Galactic Odyssey"),    starlight, 1, today.AddDays(2).AddHours(12), ScreeningFormat.IMAX,        20.00),
            Screening(Get("Dragon's Roar"),       starlight, 1, today.AddDays(2).AddHours(17), ScreeningFormat.IMAX,        20.00),
            Screening(Get("Dragon's Roar"),       starlight, 1, today.AddDays(3).AddHours(13), ScreeningFormat.IMAX,        20.00),

            // ── Starlight Multiplex, Screen 2 ──────────────────────────────
            Screening(Get("The Last Detective"),  starlight, 2, today.AddDays(1).AddHours(11), ScreeningFormat.STANDARD,    13.00),
            Screening(Get("The Haunting of Blackwood"), starlight, 2, today.AddDays(1).AddHours(20), ScreeningFormat.STANDARD, 13.00),
            Screening(Get("The Haunting of Blackwood"), starlight, 2, today.AddDays(2).AddHours(20), ScreeningFormat.STANDARD, 13.00),
            Screening(Get("The Haunting of Blackwood"), starlight, 2, today.AddDays(3).AddHours(20), ScreeningFormat.STANDARD, 13.00),

            // ── Starlight Multiplex, Screen 3 (3D) ─────────────────────────
            Screening(Get("Dragon's Roar"),       starlight, 3, today.AddDays(1).AddHours(14), ScreeningFormat.THREE_D,     17.00),
            Screening(Get("Tiny Worlds"),         starlight, 3, today.AddDays(1).AddHours(10), ScreeningFormat.THREE_D,     15.00),
            Screening(Get("Tiny Worlds"),         starlight, 3, today.AddDays(2).AddHours(10), ScreeningFormat.THREE_D,     15.00),
            Screening(Get("Galactic Odyssey"),    starlight, 3, today.AddDays(3).AddHours(17), ScreeningFormat.THREE_D,     17.00),

            // ── Starlight Multiplex, Screen 4 ──────────────────────────────
            Screening(Get("Hearts in Paris"),     starlight, 4, today.AddDays(1).AddHours(19), ScreeningFormat.STANDARD,    12.00),
            Screening(Get("The Wanderer"),        starlight, 4, today.AddDays(2).AddHours(15), ScreeningFormat.STANDARD,    12.00),
            Screening(Get("Laughing Out Loud"),   starlight, 4, today.AddDays(3).AddHours(18), ScreeningFormat.STANDARD,    12.00),
        };

        db.Screenings.AddRange(screenings);
        await db.SaveChangesAsync();
    }

    private static Screening Screening(Movie movie, Theatre theatre, int screenNumber, DateTime startTime, ScreeningFormat format, double basePrice) =>
        new()
        {
            MovieId = movie.Id,
            TheatreId = theatre.Id,
            ScreenNumber = screenNumber,
            StartTime = startTime,
            EndTime = startTime.AddMinutes(movie.DurationMinutes),
            Format = format,
            BasePrice = basePrice
        };
}
