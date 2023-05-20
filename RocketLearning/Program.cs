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

        // CONEX�O COM O BANCO DE DADOS
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContextPool<DataContext>(options =>
        options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString)));
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
