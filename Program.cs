using Doctors_Web_Forum.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
     if (!context.Admins.Any())
    {
        var hasher = new PasswordHasher<Doctors_Web_Forum.Models.Admin>();
        var admin = new Doctors_Web_Forum.Models.Admin { Username = "Dua" };
        admin.PasswordHash = hasher.HashPassword(admin, "Dua@123");

        context.Admins.Add(admin);
        context.SaveChanges();
    }
    if (!context.Admins.Any(a => a.Username == "Fatima"))
    {
        var hasher2 = new PasswordHasher<Doctors_Web_Forum.Models.Admin>();
        var newAdmin = new Doctors_Web_Forum.Models.Admin { Username = "Fatima" };
        newAdmin.PasswordHash = hasher2.HashPassword(newAdmin, "Fatima@123");

        context.Admins.Add(newAdmin);
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
