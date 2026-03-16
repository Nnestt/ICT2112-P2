using ProRental.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

//Services builder(add your mappers/gateways, controllers, control and interface classes here)
//Team P2-1
// Data source

// Domain

// Presentation/Controllers


//Team P2-2
// Data source

// Domain

// Presentation/Controllers

//Team P2-3
// Data source

// Domain

// Presentation/Controllers


//Team P2-4
// Data source

// Domain

// Presentation/Controllers


//Team P2-5
// Data source

// Domain

// Presentation/Controllers


//Team P2-6
// Data source

// Domain

// Presentation/Controllers


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

app.Run();
