using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MorePracticeMalodyServer.Data;

namespace MorePracticeMalodyServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DataContext>(option =>
            {
#if DEBUG
                option.LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();
#endif
                if (string.IsNullOrWhiteSpace(Configuration["Data:ConnectionString"]))
                    throw new ArgumentException(
                        "ConnectionString cannot be null or whitespace! Check your configuration!");
                switch (Configuration["Data:Provider"].ToLower())
                {
                    case "sqlite":
                        option.UseSqlite(Configuration["Data:ConnectionString"]);
                        break;
                    case "mysql":
                        option.UseMySql(Configuration["Data:ConnectionString"],
                            new MySqlServerVersion(Configuration["Data:ServerVersion"]));
                        break;
                    case "sqlserver":
                        option.UseSqlServer(Configuration["Data:ConnectionString"]);
                        break;
                    default:
                        throw new ArgumentException(
                            "Provider is invalid. Make sure it's one of 'MySql' 'Sqlite' 'SqlServer'!");
                }
            }, int.Parse(Configuration["Data:PoolSize"]));
            services.AddMemoryCache();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            // Support .mc file download.
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".mc"] = "application/octet-stream";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}