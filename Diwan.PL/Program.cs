using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.BLL.Repositories;
using Diwan.DAL.Contexts;
using Diwan.DAL.Models;
using Diwan.PL.MappingProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diwan.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DiwanDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
            });// Allow DI LifeTime
            builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IReactionRepository, ReactionRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(M => M.AddProfiles(new List<Profile>() { new PostProfile(), new UserProfile(), new FriendshipProfile(), new NotificationProfile(), new CommentProfile()}));
            builder.Services.AddIdentity<DiwanUser, IdentityRole>(Options =>
            {
                Options.Password.RequireNonAlphanumeric = true; // @#
                Options.Password.RequireDigit = true; // 1151
                Options.Password.RequireLowercase = true; // aa
                Options.Password.RequireUppercase = true; // AA
            })
               .AddEntityFrameworkStores<DiwanDbContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(Options =>
                {
                    Options.LoginPath = "/Account/Login";
                    Options.AccessDeniedPath = "/Home/Error";
                });
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
