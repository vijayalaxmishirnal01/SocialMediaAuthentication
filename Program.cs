using AuthenticationDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthentication()
                        .AddGoogle(options =>
                        {
                            IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");

                            options.ClientId = googleAuthSection["ClientId"];
                            options.ClientSecret = googleAuthSection["ClientSecret"];
                        })
                        .AddFacebook(facebookOptions =>
                        {
                            facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                            facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                        })
                        .AddTwitter(twitterOptions =>
                        {
                            twitterOptions.ConsumerKey = builder.Configuration["Authentication:Twitter:ClientId"];
                            twitterOptions.ConsumerSecret = builder.Configuration["Authentication:Twitter:ClientSecret"];
                        })
                       .AddGitHub(options =>
                       {
                           options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
                           options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
                       })
                         .AddLinkedIn(options =>
                         {
                             options.ClientId = builder.Configuration["Authentication:LinkedIn:ClientId"];
                             options.ClientSecret = builder.Configuration["Authentication:LinkedIn:ClientSecret"];
                         });



        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
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
    }
}