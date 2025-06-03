namespace ExistingAPI.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Author { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? Genre { get; set; }
    public decimal Price { get; set; }
}
