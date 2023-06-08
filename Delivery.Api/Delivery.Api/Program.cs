using Delivery.Api;
using Delivery.Api.Data;
using Delivery.Api.Filters;
using Delivery.Api.Service;
using Delivery.Api.Service.Interfaces;
using Delivery.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Delivery.Api.Service;

var builder = WebApplication.CreateBuilder(args);
//DataBase
builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(GlobalExceptionFilter));//Global registration
})
.AddJsonOptions(options =>
{
    //serialization of Enum as String
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDistributedMemoryCache();
//Repository
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();

//Mapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

//Swagger
builder.AddSwaggerExt();


//Add authentication
builder.AddAuthenticationExt();

var app = builder.Build();
//SeedData
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

app.UseSwaggerExt();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
