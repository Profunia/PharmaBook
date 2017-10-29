using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PharmaBook.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using PharmaBook.Services;
using AutoMapper;
using PharmaBook.ViewModel;

namespace PharmaBook
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.CookieHttpOnly = true;
            });
            services.AddMvc();
            services.AddDbContext<PharmaBookContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("dbPharmaBook")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<PharmaBookContext>();

            // services register 
            services.AddScoped<IProduct, ProductServices>();
            services.AddScoped<IPurchasedHistory, PurchasedHistoryService>();
            services.AddScoped<IVendorServices, VendorServices>();
            services.AddScoped<Imaster,MasterInvcSrvice>();
            services.AddScoped<IChild, ChildInvcSrvice>();

            services.AddScoped<IMasterPOServices, MasterPOServices>();
            services.AddScoped<IchildPoServices, ChildPOServices>();

            services.AddScoped<IProfileServices, ProfileServices>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<VendorDtl, Vendor>().ReverseMap();
                config.CreateMap<ProductViewModel, Product>().ReverseMap();
                config.CreateMap<InvcMstrVmdl, MasterInvoice>().ReverseMap();
                config.CreateMap<InvcChildVmdl, ChildInvoice>().ReverseMap();
            });
           
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseStaticFiles();
            app.UseIdentity();
           
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");
            });
        }
    }
}
