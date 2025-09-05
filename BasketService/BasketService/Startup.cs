using BasketService.Infrastructure.Contexts;
using BasketService.Infrastructure.MappingProfile;
using BasketService.MessagingBus;
using BasketService.MessagingBus.RecivedMessages.ProductMessages;
using BasketService.Model.Services;
using BasketService.Model.Services.BasketServices.MessagesDto;
using BasketService.Model.Services.DiscountServices;
using BasketService.Model.Services.ProductServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BasketService
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasketService", Version = "v1" });
            });
            services.AddDbContext<BasketDataBaseContext>(o => o.UseSqlServer
            (Configuration["BasketConnection"]),ServiceLifetime.Singleton);

            services.AddAutoMapper(typeof(BasketMappingProfile));

            services.AddTransient<IBasketService, BasketService.Model.Services.BasketService>();
            services.AddScoped<IDiscountService, BasketService.Model.Services.DiscountServices.DiscountService>();

            services.Configure<RabbitMQConfig>(Configuration.GetSection("RabbitMQConfig"));

            services.AddScoped<RabbitMQBus<BasketCheckOutMessage>, RabbitMQMessageBus>();

            services.AddTransient<IProductService, ProductSerrvice>();

            services.AddHostedService<RecivedUpdateProductName>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
