using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeskBookingApplication.Data;
using DeskBookingApplication.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DeskBookingAuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'DeskBookingAuthDbContextConnection' not found.");

builder.Services.AddDbContext<DeskBookingAuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<DeskBookingApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DeskBookingAuthDbContext>();

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

//Seeding roles into app - called everytime app is restarted 
using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Manager", "Employee" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

}

//Seeding a manager account with a manager role into app - called everytime app is restarted 
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DeskBookingApplicationUser>>();

    string email = "manager@manager.com";
    string password = "Password1!";

    if(await userManager.FindByEmailAsync(email) == null)
    {
        var user = new DeskBookingApplicationUser();
        user.UserName = email;
        user.Email = email;
        user.FirstName = "System";
        user.LastName = "Manager";

        await userManager.CreateAsync(user, password);

       await userManager.AddToRoleAsync(user, "Manager");

    }

}

app.Run();
