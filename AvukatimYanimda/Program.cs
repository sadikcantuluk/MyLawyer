using AvukatimYanimda.Services;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Chat/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize ChatPDF with Anayasa document
try
{
    using (var scope = app.Services.CreateScope())
    {
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs", "anayasa.pdf");
            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException("Anayasa PDF dosyası bulunamadı", pdfPath);
            }

            await chatService.InitializeSourceDocument(pdfPath);
            logger.LogInformation("PDF başarıyla yüklendi");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PDF yüklenirken hata oluştu, uygulama PDF olmadan devam edecek");
        }
    }
}
catch (Exception ex)
{
    // Log error but don't prevent app from starting
    Console.WriteLine($"Error initializing ChatPDF: {ex.Message}");
}

app.Run();
