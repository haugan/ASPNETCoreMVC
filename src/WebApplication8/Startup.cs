using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication8.Data;
using WebApplication8.Models;
using WebApplication8.Services;
using System.Globalization;
using WebApplication8.Middleware;

namespace WebApplication8
{
    public class Startup
    {
        // ASP.NET vil automatisk prøve finne objekter som refereres i konstruktørparameter.
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Konfigurer dependency injections og middleware pipeline.
        // Forbered systemet på middleware som senere skal brukes (i Configure).
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();


            // !!!
            // Det er ingenting som heter ASP.NET Core MVC.
            // ASP.NET Core; inneholder alt..

            //services.AddMvcCore(); 

            // Legger altså til essensiell MVC-funksjonalitet (f.eks. ikke Razor).
            // !!!


            // Legg til requestID service (som jeg har skrevet i RequestID.cs)
            // Si til ASP.NET; om noen etterspør "interface", er det "factory" som håndterer det.
            // Factory må implementere interface..
            services.AddTransient<IRequestIdFactory, RequestIdFactory>();
            services.AddScoped<IRequestId, RequestId>();

            // Add application services.
            services.AddSingleton<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            log.AddConsole(Configuration.GetSection("Logging"));
            log.AddDebug();

            // Konfigurer third-party logger; SeriLog..
            // Stilig NuGet-pakke muliggjør operasjonen på kun èn kodelinje!
            log.AddFile("Logs/webapplication8-{Date}.txt");

            // Konfigurer custom Logger; (log early, log often)..
            // Nivå på logging hentes fra appsettings.json, eller programmatisk.
            // loggerFactory.AddConsole(LogLevel.Critical);
            var logger = log.CreateLogger<Program>();
            logger.LogCritical($"Current culture is {CultureInfo.CurrentCulture.NativeName}, {CultureInfo.CurrentCulture.Name}\n\n");

            // De forskjellige logging-nivåene:
            // *Critical, *Debug, *Error, *Information, *Warning, *Trace

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                // Dette er standard exception handler; default i ASP.NET
                app.UseExceptionHandler("/Home/Error");
            }

            // Vis HTML i en custom 404 error page. (ctrl+m, ctrl+u / ctrl+m, ctrl+h)
            /*app.UseStatusCodePages(subApp =>
            {
                subApp.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<strong>IKKE FUNNET!</strong>");
                    await context.Response.WriteAsync(new String(' ', 512)); // Padding for IE.
                });
            });

            // Kast en custom exception; sjekk debug i browser.
            app.Run(context =>
            {
                //throw new InvalidOperationException("Et eller annet feil har skjedd.");

                // Returner manuell 404 HTTP status.
                context.Response.StatusCode = 404;
                return Task.FromResult(0);
            });*/

            // Bruk custom made Middleware; skrevet i RequestID.cs og RequestMiddleware.cs
            app.UseMiddleware<RequestIdMiddleware>();

            app.UseStaticFiles();
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            // Konfigurer ekstern autentisering via Facebook.
            // User secrets; ID og Secret, hentes fra secrets.json..
            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Movies}/{action=Index}/{id?}");
            });
        }
    }
}
