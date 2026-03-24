var builder = WebApplication.CreateBuilder(args);

// Dodanie obs³ugi kontrolerów i widoków MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja potoku ¿¹dañ HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Wymuszenie HTTPS w produkcji
    app.UseHsts();
}

app.UseHttpsRedirection();
// Umo¿liwia serwowanie plików statycznych (CSS, JS, obrazki z folderu wwwroot)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Skonfigurowanie domyœlnego routingu na HomeController i akcjê Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
