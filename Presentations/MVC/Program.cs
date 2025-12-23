using Infrastructure.DependencyResolvers;
using MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SESSION (JWT burada tutulacak)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HTTP CLIENT (API ile konuþmak için)
builder.Services.AddHttpClient<AuthApiService>();

// Onion Infrastructure servisleri
DependencyResolver.RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SESSION MIDDLEWARE (Authentication YOK)
app.UseSession();

// Authorization þimdilik boþ, dursun
app.UseAuthorization();

// AREA route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
