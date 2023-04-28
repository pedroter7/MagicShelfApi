using Microsoft.EntityFrameworkCore;
using PedroTer7.MagicShelf.Api.Config.Mappings;
using PedroTer7.MagicShelf.Api.CrossCutting;
using PedroTer7.MagicShelf.Api.Data.DataContexts;
using PedroTer7.MagicShelf.Api.Service.Config.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.Configure<ShelfOptions>(
    builder.Configuration.GetSection(ShelfOptions.Key));

var connectionString = builder.Configuration.GetSection("ConnectionStrings")["SqlServerDb"];
builder.Services.AddDbContext<MagicShelfDbContext>(opt =>
{
    opt.UseSqlServer(connectionString, b => b.MigrationsAssembly("PedroTer7.MagicShelf.Api"));
});

builder.Services.AddAutoMapper(typeof(ServiceToPresentationMappingProfile));
builder.Services.AddAutoMapper(typeof(PresentationToServiceMappingProfile));

CrossCuttingDIConfiguration.ConfigureAutoMapperMappingProfiles(builder.Services);
CrossCuttingDIConfiguration.ConfigureServices(builder.Services);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

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

app.UseHealthChecks("/health");

app.Run();
