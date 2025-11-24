using Dapper;
using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Services;
using MISA.Core.Services;
using MISA.CustomerManagement.Api.Helpers;
using MISA.Infrastructure.Repository;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        //Đăng ký kiểu GuidTypeHandler với Dapper
        SqlMapper.AddTypeHandler(new GuidTypeHelper());
        // Add services to the container.
        builder.Services.AddControllers();

        // Đăng ký ICustomerService
        builder.Services.AddScoped<ICustomerService, CustomerService>();
       // builder.Services.AddScoped<ICustomerService, CustomerService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        IServiceCollection serviceCollection = builder.Services.AddSwaggerGen();

        //Cấu hình DI
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // Service
        builder.Services.AddScoped<IUserService, UserService>();


        string hashFromDb = "$2a$10$7QJ7G1x7dDYrVdzSsfXr/OdT95YtTpr0aS/z5tH6tFNEwG.ZVS2k6";
        string passwordToTest = "123456"; // mật khẩu bạn muốn kiểm tra

        bool isValid = BCrypt.Net.BCrypt.Verify(passwordToTest, hashFromDb);

        Console.WriteLine(isValid
            ? "Password đúng với hash"
            : "Password KHÔNG đúng với hash");



        // Thêm CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173") // frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors(); // bật CORS

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