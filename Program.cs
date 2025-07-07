using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeskBookingApplication.Data;
using DeskBookingApplication.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DeskBookingAuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'DeskBookingAuthDbContextConnection' not found.");

builder.Services.AddDbContext<DeskBookingAuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<DeskBookingApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<DeskBookingAuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
