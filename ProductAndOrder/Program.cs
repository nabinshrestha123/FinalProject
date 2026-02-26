using Microsoft.EntityFrameworkCore;
using ProductAndOrder.Api.DependencyInjection;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Application.Services;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;
using ProductAndOrder.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDBContext>(options =>
	options.UseNpgsql(
		"Host=localhost;Port=5432;Database=NewProject;Username=postgres;Password=koeJ2449k"));
builder.Services.AddScoped<IProductDto, ProductService>();
builder.Services.AddScoped<IProduct, ProductRepository>();

builder.Services.AddScoped<IOrderDto, OrderService>();
builder.Services.AddScoped<IOrder, OrderRepository>();

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddApiDI();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


// ✅ ADD THIS (Fix for 404 on root)
app.MapGet("/", () => "ProductAndOrder API is running successfully...");

app.Run();