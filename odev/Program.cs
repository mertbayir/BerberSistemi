using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using odev.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(
        options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        }

);

var conn = "Server=(localdb)\\mssqllocaldb;Database=odevDb;Trusted_Connection=True";

builder.Services.AddDbContext<UserContext>(

            options => options.UseSqlServer(conn)

 );

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "userauth";
        options.LoginPath = "/User/Login"; // Login sayfasý
        options.LogoutPath = "/User/Logout"; // Logout iþlemi
        options.AccessDeniedPath = "/User/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Oturum süresi
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserContext>();

    // Varsayýlan admin ve personel hesaplarý
    if (!context.Users.Any())
    {
        context.Users.Add(new User { Email = "mert", Password = "sau" });
        context.Users.Add(new User { Email = "ahmet", Password = "sau" });
        context.SaveChanges();
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
