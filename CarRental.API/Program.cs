using CarRental.Api.DTOs;
using CarRental.API.DTOs;
using CarRental.API.Validation.Validators;
using FluentValidation;

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
            builder.Services.AddSwaggerGen();

            builder.Services.AddCarRentalServices();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<IValidator<CustomerDTO>, CustomerValidator>();
            //builder.Services.AddTransient<IValidator<AddressDTO>, AddressValidator>();
            //builder.Services.AddTransient<IValidator<UserDTO>, UserValidator>();
            builder.Services.AddTransient<IValidator<VehicleDTO>, VehicleValidator>();
            builder.Services.AddTransient<IValidator<ReservationDTO>, ReservationValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
