// Initialize the web application builder with command-line arguments
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register MVC controllers and views for the application
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Make the Program class accessible to the test project
public partial class Program { }
