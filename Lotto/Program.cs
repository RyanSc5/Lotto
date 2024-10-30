using Microsoft.EntityFrameworkCore;
using Lotto.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies; // ��������

namespace Lotto
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // �K�[�������ҪA�ȡA�ϥ� Cookie ����
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login"; // �n�����������|
                    options.LogoutPath = "/Member/Logout"; // �n�X���|
                    options.AccessDeniedPath = "/Home/Login"; // �X�ݳQ�ڵ�������
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            // DI�`�J
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

            // �ϥΨ������ҩM���v
            app.UseAuthentication();// �u���[�o��, �U��app.UseAuthorization();�쥻�N��
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Clock}/{id?}");

            app.Run();
        }
    }
}