using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExistingAPI.Models;

namespace ExistingAPI.Data;

public class BooksContext(DbContextOptions<BooksContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var books = Enumerable.Range(1, 100).Select(i => new Book
        {
            Id = i,
            Title = $"Book {i}",
            Author = $"Author {i}",
            PublishedDate = DateTime.UtcNow.AddYears(-i),  // UTC for PostgreSQL compatibility
            Genre = "Fiction",
            Price = i * 10.0m
        }).ToList();

        modelBuilder.Entity<Book>().HasData(books);
    }
}
