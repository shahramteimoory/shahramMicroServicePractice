using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrderService.Infrastructure.Context;
using OrderService.MessagingBus;
using OrderService.MessagingBus.RecivedMessage;
using OrderService.Model.Services;
using OrderService.Model.Services.MessagesDto;
using OrderService.Model.Services.ProductServices;
using OrderService.Model.Services.RegisterOrderServices;

namespace OrderService
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderService", Version = "v1" });
            });
            services.AddDbContext<OrderDataBaseContext>(o => o.UseSqlServer
                (Configuration["OrderConnection"]),ServiceLifetime.Singleton);

            services.AddTransient<IOrderService, OrderService.Model.Services.OrderService>();

            services.Configure<RabbitMQConfig>(Configuration.GetSection("RabbitMQConfig"));


            services.AddHostedService<RecivedOrderMessage>();

            services.AddTransient<IProductServices, ProductService>();

            services.AddTransient<IRegisterOrderService, RegisterOrderService>();

            services.AddTransient<RabbitMQBus<SendOrderToPaymentMessage>, RabbitMQOrderToPaymentMessageBus>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.Authority = "https://localhost:7036";
                    option.Audience = "orderservice";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
