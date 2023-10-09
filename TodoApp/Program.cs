using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TodoApp.Data;
using TodoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

#region MinimalApi

app.MapGet("/Todo", async (AppDbContext dbContext) => await dbContext.Todos.ToListAsync());
app.MapPost("/Todo/{todo}", async (TodoDto todo, AppDbContext dbContext) =>
{
    await dbContext.Todos.AddAsync(new Todo { Body = todo.Body, Title = todo.Title });
    await dbContext.SaveChangesAsync();
});
app.MapDelete("/Todo/{id}", async (int id, AppDbContext dbContext) =>
{
    var todo = await dbContext.Todos.Where(todo => todo.Id == id).FirstOrDefaultAsync();
    if (todo is not null)
    {
        dbContext.Todos.Remove(todo);
        await dbContext.SaveChangesAsync();
    }
});

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();