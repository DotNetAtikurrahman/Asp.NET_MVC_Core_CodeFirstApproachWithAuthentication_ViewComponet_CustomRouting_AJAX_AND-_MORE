using ASP.NET_CORE_CodeFirst.Data;
using ASP.NET_CORE_CodeFirst.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

// CUSTOM CONVENTIOANL  ROUTING FOR CUSTOMER   

app.MapControllerRoute(
      name : "amercustomroute",            // I CAN GIVE ANY NAME
      pattern:"add/newcustomer/mydatabase", // I CAN CREATE ANY PATTERN
      defaults : new
      {
          controller ="Customers", action = "Create" // ITS MY DEFAULT ROUTING IN CUSTOMERS CONTROLLER WITH SPECIFIC ACTION LIKE CREATE ACTION HERE ITS THE MAGIC THAT UNDERSTAND THE ASP.NET WHAT ACTION AND CONTROLER MAKE IT CUSTOM ROUTING
      }

    );




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

  

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
