using ApiGateWayForWeb.Models.DiscountServices;
using Grpc.Net.Client;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

var discountServer =builder.Configuration["MicroservicAddress:Discount:Uri"];
builder.Services.AddSingleton(GrpcChannel.ForAddress(discountServer));
// اگه چنتا کانال داشتیم باید یچی مثل استراتژی درست کنم که کانال ها رو تشخیص بده و درست پخش کنه


builder.Services.AddTransient<IDiscountService, DiscountServiceClass>();

builder.Services.AddControllers();

IConfiguration configuration = builder.Configuration;
IWebHostEnvironment webHostEnvironment = builder.Environment;

builder.Configuration.SetBasePath(webHostEnvironment.ContentRootPath)
    .AddJsonFile("ocelot.json")
    .AddOcelot(webHostEnvironment)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(configuration)
    .AddCacheManager(x => x.WithDictionaryHandle())
    .AddPolly();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseOcelot().Wait();

app.Run();
