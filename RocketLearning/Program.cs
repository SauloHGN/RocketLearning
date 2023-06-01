using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RocketLearning.Models;
using System.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
    
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // CONEXÃO COM O BANCO DE DADOS
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContextPool<DataContext>(options =>
        options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString)));
        //


        // API YOUTUBE
        //var credential = GoogleCredential.FromApiKey("YOUR_API_KEY");
        //

        // API GOOGLE DRIVE
        /*var clientSecretPath = Path.Combine(builder.Environment.ContentRootPath, "client_secret_1031212755190-f2gnec77k5vaksm9l9oq8bsitk6in2i1.apps.googleusercontent.com.json");

        builder.Services.AddTransient<DriveService>();
        builder.Services.AddAuthentication()
           .AddGoogle(options =>           {
               options.ClientId = clientSecretPath;
               options.ClientSecret = clientSecretPath;
           });
        */
        //

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
    }
}
