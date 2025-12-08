using Infrastructure.DependencyResolvers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Onion Infrastructure servislerini kaydet
DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Migration + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FilmDukkaniDbContext>();

        // Migration'ları otomatik uygula
        context.Database.Migrate();

        var seeder = services.GetRequiredService<DatabaseSeeder>();

        // 1) Her ortamda sabit referans datayı yükle
        await seeder.SeedReferenceDataAsync();

        // 2) Sadece development ortamında fake data yükle
        if (app.Environment.IsDevelopment())
        {
            await seeder.SeedDevDataAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seed sırasında hata: {ex.Message}");
    }
}

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
