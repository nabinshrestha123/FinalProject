using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductAndOrder.Api.DependencyInjection;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Application.Kafka.Producer.ProducerInterface;
using ProductAndOrder.Application.Kafka.Producer.ProducerService;
using ProductAndOrder.Application.Services;
using ProductAndOrder.Application.Services.Strategy_pattern;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;
using ProductAndOrder.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);


// ====================== DATABASE ======================
builder.Services.AddDbContext<AppDBContext>(options =>
	options.UseNpgsql(
		builder.Configuration.GetConnectionString("DefaultConnection")
	));


// ====================== DEPENDENCY INJECTION ======================
builder.Services.AddScoped<IProductDto, ProductService>();
builder.Services.AddScoped<IProduct, ProductRepository>();

builder.Services.AddScoped<IOrderDto, OrderService>();
builder.Services.AddScoped<IOrder, OrderRepository>();

builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();

builder.Services.AddScoped<IKafkaProducer, KafkaProducerService>();

// Strategy Pattern
builder.Services.AddScoped<DiscountStrategyFactory>();


// ====================== VALIDATION ======================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDto>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDto>();


// ====================== HTTP CLIENT ======================
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>((sp, client) =>
{
	var config = sp.GetRequiredService<IConfiguration>();
	client.BaseAddress = new Uri(config["ServiceUrls:UserService"]);
});


// ====================== CONTROLLERS ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiDI();


// ====================== SWAGGER + JWT ======================
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter JWT token"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});


// ====================== AUTHENTICATION ======================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],

			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
			)
		};
	});

builder.Services.AddAuthorization();


// ====================== BUILD APP ======================
var app = builder.Build();


// ====================== MIDDLEWARE ======================
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "ProductAndOrder API is running successfully...");

app.Run();