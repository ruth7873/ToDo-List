using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ToDoDbContext>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet(
    "/items",
    async (ToDoDbContext dbContext) =>
    {
        var items =await dbContext.Items.ToListAsync();
        return items;
    }
);
app.MapPost(
    "/items",
    async (Item item, ToDoDbContext dbContext) =>
    {
        await dbContext.Items.AddAsync(item);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/", item);
    }
);
app.MapPut(
    "/items/{id}",
    async (int id, Item item, ToDoDbContext dbContext) =>
    {
        var i = await dbContext.Items.FindAsync(id);
        if (i == null)
            return Results.BadRequest("there is no such item!!!");
        i.IsComplete = item.IsComplete;

        await dbContext.SaveChangesAsync();
        return Results.Created($"/", i);
    }
);
app.MapDelete("/items/{id}", async (int id, ToDoDbContext dbContext) =>
    {
        var i = await dbContext.Items.FindAsync(id);
        if (i != null)
        {
            dbContext.Items.Remove(i);
            await dbContext.SaveChangesAsync();
        }
    }
);
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});
app.Run();