using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MicroServiceApp.UserService;
using MicroServiceApp.UserService.Dto;
using MicroServiceApp.UserService.Repositories;
using MicroServiceApp.UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = configuration["IdentityServer:Authority"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IServiceBusProcessor<UserRegisteredMessage>>(provider =>
{
    var connectionString = configuration.GetConnectionString("AzureServiceBus");
    var queueName = configuration["AzureServiceBus:RegistrationQueue"];
    return new AzureServiceBusMessageHandler<UserRegisteredMessage>(connectionString, queueName);
});

var app = builder.Build();

app.Services.GetService<IServiceBusProcessor<UserRegisteredMessage>>()?.StartAsync((user) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var mapper = scope.ServiceProvider.GetService<IMapper>();
        var userService = scope.ServiceProvider.GetService<IUserService>();
        var userDto = mapper?.Map<UserDto>(user);
        if (userDto != null) userService?.RegisterUserAsync(userDto);
    }
});

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
{
    serviceScope?.ServiceProvider.GetRequiredService<UserDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();