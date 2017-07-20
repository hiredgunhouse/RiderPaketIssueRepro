using System;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;
using NLog.Web;

namespace MyProject.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the Configure method, below.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection.

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
                //options.ForwardClientCertificate = true;
                ////options.ForwardWindowsAuthentication = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyPolicy",
                    policyBuilder =>
                    {
                        policyBuilder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
            services.AddMvc();

            // Create the container builder.
            var builder = new ContainerBuilder();
            // tools
            builder.RegisterInstance(new NLog.LogFactory().GetLogger("MyProject API")).As<NLog.ILogger>();

            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            // TODO update this to work for both Dev and Prod envs
            ////#if DEBUG
            ////loggerFactory
            ////    ////.AddConsole((category, LogLevel) => true, includeScopes: true)
            ////    .AddConsole(Configuration.GetSection("Logging"))
            ////    .AddDebug(LogLevel.Trace);
            ////#endif

            // add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            // add NLog.Web
            app.AddNLogWeb();

            ////if (env.IsDevelopment())
            ////{
            ////    // TODO update this to use /Home/Error which is better and more convenient
            ////    app.UseDeveloperExceptionPage();

            ////    ////app.UseExceptionHandler("/Home/Error");
            ////    ////app.UseBrowserLink();
            ////}
            ////else
            ////{
            app.UseExceptionHandler("/Home/Error");
            ////}

            app.UseCors("AllowAnyPolicy");

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                ////routes.MapRoute(
                ////    name: "/",
                ////    template: "",
                ////    defaults: new {controller="Home", action="Index"});
                ////routes.MapRoute(
                ////    name: "/Home",
                ////    template: "Home",
                ////    defaults: new {controller="Home", action="Index"});
                ////routes.MapRoute(
                ////    name: "/Home/Index",
                ////    template: "Home/Index",
                ////    defaults: new {controller="Home", action="Index"});
                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}/{action}/{id?}");
                routes.MapRoute(
                    name: "/Home/Error",
                    template: "Home/Error",
                    defaults: new {controller="Home", action="Error"});
                routes.MapRoute(
                    name: "CatchAllToHomeIndex",
                    template: "{*url}",
                    defaults: new {controller="Home", action="Index"});
            });
        }
    }
}
