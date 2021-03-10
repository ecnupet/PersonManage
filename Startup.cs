using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using person.Model;
using person.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Services;

namespace person
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddDbContext<PersonContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MySqlConnection")));
            services.AddControllers().AddNewtonsoftJson();
            services.AddScoped<IPersonAuthService, PersonAuthService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCors(options =>
            {
                options.AddPolicy(name: "policy",
                                  builder =>
                                  {
                                      builder.SetIsOriginAllowed((host) => true)
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials();
                                  });
            });
            var keyBase64 = "494812665@qq.com";
            var keyByeArray = Encoding.ASCII.GetBytes(keyBase64);
            var signKey = new SymmetricSecurityKey(keyByeArray);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signKey,//Êý×ÖÇ©Ãû
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie(option =>
            {
                option.LoginPath= "/api/Person/PersonLogin";
                option.LogoutPath = "/api/Person/PersonLogout";
                option.Cookie.HttpOnly = false;
                option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                option.Cookie.SameSite = SameSiteMode.None;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true;
                o.SaveToken = true;
                o.TokenValidationParameters = tokenValidationParameters;
            });
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


            app.UseCors("policy");
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
