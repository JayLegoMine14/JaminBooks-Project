using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JaminBooks.Model;
using JaminBooks.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


namespace JaminBooks
{
    /// <summary>
    /// Initializes the web server
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Starts the web server with the given configuration.
        /// </summary>
        /// <param name="configuration">The web server's configuration</param>
        public Startup(IConfiguration configuration)
        {
            //Set all of the server details from the configuration file
            Configuration = configuration;
            SQL.ConnectionString = Configuration["ConnectionString"];
            Authentication.Email = Configuration["Email"];
            Authentication.Password = Configuration["EmailPassword"];
            Authentication.Name = Configuration["WebsiteName"];
        }

        /// <summary>
        /// The server's configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Service to add</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            services.AddDistributedMemoryCache();

            //Set the timeout for sessions to 20 minutes
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="env">The hosting environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseSession();
            app.UseMvc();
        }
    }
}
