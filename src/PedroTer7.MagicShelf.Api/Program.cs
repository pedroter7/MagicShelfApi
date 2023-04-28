using Microsoft.EntityFrameworkCore;
using PedroTer7.MagicShelf.Api.Data.DataContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetSection("ConnectionStrings")["SqlServerDb"];
builder.Services.AddDbContext<MagicShelfDbContext>(opt =>
{
    opt.UseSqlServer(connectionString, b => b.MigrationsAssembly("PedroTer7.MagicShelf.Api"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
