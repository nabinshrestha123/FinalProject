using System.Text;
using Confluent.Kafka;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;              // ✅ keep only this
using ProductAndOrder.Api.DependencyInjection;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Application.Kafka.Producer.ProducerInterface;
using ProductAndOrder.Application.Kafka.Producer.ProducerService;
using ProductAndOrder.Application.Services;
using ProductAndOrder.Domain.Interfaces;
using ProductAndOrder.Infrastructure.Data;
using ProductAndOrder.Infrastructure.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;     // ✅ add this

		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddDbContext<AppDBContext>(options =>
			options.UseNpgsql(
				"Host=localhost;Port=5432;Database=NewProject;Username=postgres;Password=koeJ2449k"));

		builder.Services.AddScoped<IProductDto, ProductService>();
		builder.Services.AddScoped<IProduct, ProductRepository>();
		builder.Services.AddFluentValidationAutoValidation();
		builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDto>();
		builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDto>();
		builder.Services.AddScoped<IOrderDto, OrderService>();
		builder.Services.AddScoped<IOrder, OrderRepository>();
		builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();
        builder.Services.AddScoped<IKafkaProducer, KafkaProducerService>();
      

builder.Services.AddControllers();
		builder.Services.AddApiDI();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>((serviceProvider, client) =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();

			var baseUrl = configuration["ServiceUrls:UserService"];

			client.BaseAddress = new Uri(baseUrl);
		});


		// ✅ Swagger with JWT Authorize button
		builder.Services.AddSwaggerGen(c =>
		{
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Enter your JWT token here"
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
						Encoding.UTF8.GetBytes(
							builder.Configuration["Jwt:Key"]!  // ✅ added ! to fix null warning
						))
				};
			});


		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			// ✅ Removed app.MapOpenApi() — was conflicting
		}




		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		app.MapGet("/", () => "ProductAndOrder API is running successfully...");

		app.Run();
	
	