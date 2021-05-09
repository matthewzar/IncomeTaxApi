using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PointsTaxAPI.Models;
using PointsTaxAPI.Services;
using PointsTaxAPI.Services.IncomeTaxCalculators;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointsTaxAPI
{
    public class Startup
    {
        /// <summary>
        /// The address of the data access layer. Used by ITaxBracketGetter's Refit API wrapper.
        /// TODO: Move to config.
        /// </summary>
        const string TAXBRACKET_API_ADDRESS = "http://localhost:5000";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Model and Service injection

            // Controler-assistance services
            services.AddSingleton<IIncomeTaxCalculator, PointIncomeTaxCalculator>();

            // Model 
            var pointApiConnectionService = RestService.For<ITaxBracketGetter>(TAXBRACKET_API_ADDRESS);
            services.AddSingleton(pointApiConnectionService);

            #endregion

            #region Enabling Cors policies   

            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1#attr
            services.AddCors(options =>
            {
                options.AddPolicy("Unrestricted",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod();
                    });
            });

            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
