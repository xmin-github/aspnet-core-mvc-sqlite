using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using employeesapi.Models;

namespace employeesapi
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=employees.db"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //Add strongly-typed OktaApiTokenValidation config object so it can be injected,e.g. into a service or controller
            services.Configure<OktaApiTokenValidation>("OktaApiTokenValidationOptions", Configuration.GetSection("OktaApiTokenValidation"));
    
            string tokenAuthority="";
            string tokenAudience="";

            try
            {
                var monitor = services.BuildServiceProvider().GetService<IOptionsMonitor<OktaApiTokenValidation>>();
                var namedOptions = monitor.Get("OktaApiTokenValidationOptions");
                tokenAuthority = string.IsNullOrEmpty(Configuration["OktaApiTokenValidation:Authority"]) ? namedOptions.Authority:Configuration["OktaApiTokenValidation:Authority"];
                tokenAudience = string.IsNullOrEmpty(Configuration["OktaApiTokenValidation:Audience"]) ? namedOptions.Authority:Configuration["OktaApiTokenValidation:Audience"];
            }
            catch (OptionsValidationException e) 
            {
              Console.Write("cannot find " + e.OptionsName);
            }
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //this is used for jwt token validation and the token will be sent to the service for authentication
                .AddJwtBearer(options =>
                {
                    options.Authority = tokenAuthority;
                    options.Audience = tokenAudience;
                    options.RequireHttpsMetadata = false;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
