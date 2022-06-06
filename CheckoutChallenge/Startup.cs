using Interfaces;
using Interfaces.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services;
using Services.BackgroundServices;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

namespace CheckoutChallenge
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
            services.AddControllers();
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSwaggerGen(options =>
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                    options.IncludeXmlComments(xmlPath);
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Checkout Api",
                        Version = "v1",
                        Description = "This is an interface for interaction with a Checkout challenge application.",
                        Contact = new OpenApiContact()
                        {
                            Name = "Vlad Gaman",
                            Email = "vladionut.gaman@gmail.com"
                        }
                    });
                    options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement());
                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                })
                .AddSingleton<IFileHandler<ItemDto>, JsonFileHandler<ItemDto>>()
                .AddSingleton<IFileHandler<PricingRuleDto>, JsonFileHandler<PricingRuleDto>>()
                .AddSingleton<ICheckoutFactory, CheckoutFactory>()
                .AddScoped<IPricingRulesRepository, PricingRulesFileRepository>()
                .AddScoped<IItemsRepository, ItemsFileRepository>()
                .AddScoped<IRulesFactory, RulesFactory>()
                .AddScoped<ICheckout, Checkout>()
                .AddScoped<IPricingRules>(serviceProvider =>
                {
                    var factory = serviceProvider.GetRequiredService<IRulesFactory>();
                    return new PricingRules(factory.GetImmutablePricingRulesForItems());
                })
                .AddHostedService<CheckoutInstancesCleanerWorker>()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout API");
                })
                .UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
