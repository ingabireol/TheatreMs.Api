using System.ComponentModel.DataAnnotations;

namespace TheatreMs.Api.Models;

public class Movie
{
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int DurationMinutes { get; set; }

    public MovieGenre? Genre { get; set; }

    [MaxLength(255)]
    public string? Director { get; set; }

    [MaxLength(255)]
    public string? Cast { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? PosterImageUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public MovieRating? Rating { get; set; }

    public ICollection<Screening> Screenings { get; set; } = [];
}

public enum MovieGenre
{
    ACTION, ADVENTURE, ANIMATION, COMEDY, CRIME,
    DOCUMENTARY, DRAMA, FAMILY, FANTASY, HORROR,
    MUSICAL, MYSTERY, ROMANCE, SCI_FI, THRILLER, WESTERN
}

public enum MovieRating
{
    G, PG, PG13, R, NC17, UNRATED
}
