using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Models;

namespace SportsStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        Configuration["Data:SportStoreProducts:ConnectionString"]));
            services.AddTransient<IProductRepository, EFProductRepository>();

            //services.AddTransient<IProductRepository, FakeProductRepository>(); //resgistering service of IProduct repository
            services.AddMvc(); //sets up shared objects used in Mvc applications
        }

        // This method gets called by the runtime. Use this method to set up features that receive and process
        //Http requests 
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage(); //only available in development process, not available in deployed applications
            app.UseStatusCodePages(); //add simple messages to Http responses 
            app.UseStaticFiles();    //method enables support for serving static content from the wwwroot folder.
            app.UseMvc(routes =>
            {
                //add new route before default
                routes.MapRoute(
                    name: "pagination",
                    template: "Products/Page{productPage}",
                    defaults: new { Controller = "Product", action = "List" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Product}/{action=List}/{id?}");
            //I need to tell MVC that it should send requests that arrive for the root URL of my application (http://
            //mysite /) to the List action method in the ProductController class
            });

            SeedData.EnsurePopulated(app); //seed the database when the application starts
        }
    }
}
