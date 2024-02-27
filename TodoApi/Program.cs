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
        // קוד לקבלת רשימת הפריטים מהמסד נתונים
        var items =await dbContext.Items.ToListAsync();

        // טקסט להחזרה כתוצאה (לדוגמה, פורמט JSON)
        return items;
    }
);
app.MapGet(
    "/items/{id}",
    async (int id, ToDoDbContext dbContext) =>
    {
        // קוד לקבלת רשימת הפריטים מהמסד נתונים
        var items =await dbContext.Items.ToListAsync();
        var i = items.Find(x => x.Id == id);
        // טקסט להחזרה כתוצאה (לדוגמה, פורמט JSON)
        return i;
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

// app.MapPost("/", () => "This is a POST");
app.MapPut(
    "/items/{id}",
    async (int id, Item item, ToDoDbContext dbContext) =>
    {
        var i = await dbContext.Items.FindAsync(id);
        if (i == null)
            return Results.BadRequest("there is no such item!!!");

        i.Name = item.Name;
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

// app.MapMethods(
//     "/options-or-head",
//     new[] { "OPTIONS", "HEAD" },
//     () => "This is an options or head request "
// );

app.Run();
