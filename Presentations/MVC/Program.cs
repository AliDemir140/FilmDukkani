using Infrastructure.DependencyResolvers;
using MVC.Filters;
using MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Cache + Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClient
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<MovieApiService>();
builder.Services.AddHttpClient<AuthApiService>();

// Onion (ServiceManager / Repo / DbContext vs)
DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

// Filters
builder.Services.AddScoped<RequireLoginAttribute>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session (Authentication yok, Cookie yok)
app.UseSession();

// AREA route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
