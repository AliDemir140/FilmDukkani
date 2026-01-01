using Infrastructure.DependencyResolvers;
using Infrastructure.Persistence;
using Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Application.Constants;
using Infrastructure.Services;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Onion Infrastructure servislerini kaydet
DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

// Hosted Service (Billing)
builder.Services.AddHostedService<MonthlyBillingHostedService>();

// JWT Authentication (DEFAULT SCHEME ZORLA)
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")
            ),

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });

// API'de cookie redirect yerine 401/403 dönsün (swagger'da 404 görünme derdi biter)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleNames.Admin));

    options.AddPolicy("DashboardAccess", policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Accounting, RoleNames.Warehouse, RoleNames.Purchasing));

    options.AddPolicy("AccountingAccess", policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Accounting));

    options.AddPolicy("WarehouseAccess", policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Warehouse));

    options.AddPolicy("PurchasingAccess", policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.Purchasing));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT (mevcut halin çalışır; aynen bıraktım)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FilmDukkani API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Sadece token gir (Bearer yazma).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Migration + Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FilmDukkaniDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeed.SeedAsync(roleManager);

        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedReferenceDataAsync();
        await seeder.SeedAdminAsync(app.Configuration);

        if (app.Environment.IsDevelopment())
        {
            await seeder.SeedDevDataAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FilmDukkani API v1");
    });

    app.MapGet("/debug/routes", (IEnumerable<EndpointDataSource> sources) =>
    {
        var list = sources
            .SelectMany(s => s.Endpoints)
            .Select(e => e.DisplayName)
            .ToList();

        return Results.Ok(list);
    });
}

app.UseHttpsRedirection();

// Routing
app.UseRouting();

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
