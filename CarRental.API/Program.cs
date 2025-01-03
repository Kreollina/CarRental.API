using CarRental.API.DTOs;
using CarRental.API.Enums;
using CarRental.API.Services;
using CarRental.API.Services.Interfaces;
using CarRental.API.Validation.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CarRental.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(setup =>
            {
                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' followed by a space and the JWT token value.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                        new string[]{}
                    }
                });
            });

            builder.Services.AddCarRentalServices();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<IValidator<CustomerDTO>, CustomerValidator>();
            //builder.Services.AddTransient<IValidator<AddressDTO>, AddressValidator>();
            //builder.Services.AddTransient<IValidator<UserDTO>, UserValidator>();
            builder.Services.AddTransient<IValidator<VehicleDTO>, VehicleValidator>();
            builder.Services.AddTransient<IValidator<ReservationDTO>, ReservationValidator>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.UseSecurityTokenValidators = true;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"]
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRole.Admin.ToString()));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole(UserRole.User.ToString()));
                options.AddPolicy("AdminAndUserPolicy", policy => policy.RequireRole(UserRole.User.ToString(), UserRole.Admin.ToString()));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    policy =>
                    {
                        policy.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers().RequireAuthorization();
            app.UseCors("AllowAnyOrigin");

            app.Run();
        }
    }
}
