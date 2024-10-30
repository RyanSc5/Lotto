using Microsoft.EntityFrameworkCore;
using Lotto.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies; // 身分驗證

namespace Lotto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加身份驗證服務，使用 Cookie 驗證
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login"; // 登錄頁面的路徑
                    options.LogoutPath = "/Member/Logout"; // 登出路徑
                    options.AccessDeniedPath = "/Home/Login"; // 訪問被拒絕的頁面
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            // DI注入
            builder.Services.AddDbContext<GameContext2>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("GameDatabase")));

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

            // 使用身份驗證和授權
            app.UseAuthentication();// 只有加這個, 下面app.UseAuthorization();原本就有
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Clock}/{id?}");

            app.Run();
        }
    }
}