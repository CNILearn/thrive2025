using Microsoft.EntityFrameworkCore;
using ExistingAPI.Data;
using ExistingAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace ExistingAPI;

public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book").WithTags(nameof(Book));

        group.MapGet("/", async (BooksContext db) =>
        {
            return await db.Books.ToListAsync();
        })
        .WithName("GetAllBooks")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Book>, NotFound>> (int id, BooksContext db) =>
        {
            return await db.Books.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Book model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBookById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Book book, BooksContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, book.Id)
                    .SetProperty(m => m.Title, book.Title)
                    .SetProperty(m => m.Author, book.Author)
                    .SetProperty(m => m.PublishedDate, book.PublishedDate)
                    .SetProperty(m => m.Genre, book.Genre)
                    .SetProperty(m => m.Price, book.Price)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBook")
        .WithOpenApi();

        group.MapPost("/", async (Book book, BooksContext db) =>
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{book.Id}",book);
        })
        .WithName("CreateBook")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, BooksContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBook")
        .WithOpenApi();
    }
}
