using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mytrade.Data;
using mytrade.Models;
using mytrade.Services;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using mytrade.LogProvider;

namespace mytrade
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("azure")));
            }            

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));
                options.AddPolicy("tm", policy => policy.RequireClaim(ClaimTypes.Role, "tm"));
                options.AddPolicy("client", policy => policy.RequireClaim(ClaimTypes.Role, "client"));
            });

            if (HostingEnvironment.IsDevelopment())
            {
                CustomLoggerDBContext.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                CustomLoggerDBContext.ConnectionString = Configuration.GetConnectionString("azure");
            }
                
            services.AddDbContext<CustomLoggerDBContext>();

            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (HostingEnvironment.IsDevelopment())
            {
                loggerFactory.AddContext(LogLevel.Information, Configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                loggerFactory.AddContext(LogLevel.Information, Configuration.GetConnectionString("azure"));
            }

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
