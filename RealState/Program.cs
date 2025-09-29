using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealState.BLL.Common.Services.AttachmentService;
using RealState.BLL.Common.Services.EmailSettings;
using RealState.BLL.Services.Property;
using RealState.DAL.Models.Identity;
using RealState.DAL.Presistance.Data;
using RealState.DAL.Presistance.UnitOfWork;

namespace RealState
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Configure Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion
            builder.Services.AddScoped<IPropertyService, PropertyService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IEmailSettings, EmailSettings>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>((option) =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequiredLength = 6;
                option.Password.RequireLowercase = true;
                option.Lockout.AllowedForNewUsers = true;
                option.Lockout.MaxFailedAccessAttempts = 7; //???? ??????? ??? ?? ??? ????? ??? 7 ???? 
            }).AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Account/SignIn";
                option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
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
