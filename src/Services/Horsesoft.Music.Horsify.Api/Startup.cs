using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using AspNet.Security.OAuth.Spotify;
using System;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Api
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
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Horsify API", Version = "v1" });
            });

            services.Add(new ServiceDescriptor(typeof(Repositories.Services.IHorsifySongService), typeof(Repositories.Services.HorsifySongService), ServiceLifetime.Singleton));

            services.AddAuthentication().AddSpotify(options => 
            {
                options.ClientId = Configuration["Spotify:ClientId"];
                options.ClientSecret = Configuration["Spotify:ClientSecret"];
                options.CallbackPath = "/";
                options.Scope.Add("user-read-private user-read-email");
            });
            //Add spotify api
            var spotEnabled = Convert.ToBoolean(Configuration["Spotify:Enabled"]);
            if (spotEnabled)
            {
                //ClientId = Configuration["Spotify:ClientId"],
                //    ClientSecret = Configuration["Spotify:ClientSecret"],
                //    RedirectUri = "http://localhost:40573/api/spotify",
          
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHsts();
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Horsify API V1");
            });

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
