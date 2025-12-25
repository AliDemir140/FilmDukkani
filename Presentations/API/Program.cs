using Infrastructure.DependencyResolvers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Onion Infrastructure servislerini kaydet
DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migration + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FilmDukkaniDbContext>();
        await context.Database.MigrateAsync();

        // ✅ Role seed
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeed.SeedAsync(roleManager);

        // ✅ Seed: reference + admin + (dev data)
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedReferenceDataAsync();

        if (app.Environment.IsDevelopment())
        {
            await seeder.SeedAdminAsync(app.Configuration);
            await seeder.SeedDevDataAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seed sırasında hata: {ex.Message}");
    }
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
