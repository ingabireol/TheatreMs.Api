using System.ComponentModel.DataAnnotations;
using TheatreMs.Api.Models;

namespace TheatreMs.Api.DTOs.Movie;

public class MovieDto
{
    public long? Id { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? DurationMinutes { get; set; }
    public MovieGenre? Genre { get; set; }

    [MaxLength(255)]
    public string? Director { get; set; }

    [MaxLength(255)]
    public string? Cast { get; set; }

    public DateOnly? ReleaseDate { get; set; }
    public string? PosterImageUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public MovieRating? Rating { get; set; }
}
