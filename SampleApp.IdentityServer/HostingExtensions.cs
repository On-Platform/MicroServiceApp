using System.Configuration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SampleApp.IdentityServer.Data;
using SampleApp.IdentityServer.Models;
using Serilog;

namespace SampleApp.IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        //builder.Services.AddRazorPages();
        
        //register appsettings.json
        var configuration = builder.Configuration;
        
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        
        builder.Services.AddSingleton<IConfiguration>(configuration);
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));
        
        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
            })
            .AddAspNetIdentity<ApplicationUser>();
        
            builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IServiceBusSender<UserRegisteredMessage>>(provider =>
                {
                    var azureServiceBusBusConnectionString = configuration.GetConnectionString("AzureServiceBus");
                    var queueName = configuration["AzureServiceBus:RegistrationQueue"];
                    return new AzureServiceBusMessageSender<UserRegisteredMessage>(azureServiceBusBusConnectionString, queueName);
                });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        InitializeDatabase(app);

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        //app.UseRouting();
            
        app.UseIdentityServer();
        app.MapControllers();

        // uncomment if you want to add a UI
        //app.UseAuthorization();
        //app.MapRazorPages().RequireAuthorization();

        return app;
    }
    
    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            
            var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (!applicationDbContext.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                userManager.CreateAsync(adminUser, "Password@123").Wait();

                var adminRole = new IdentityRole<Guid>
                {
                    Name = "Admin"
                };

                roleManager.CreateAsync(adminRole).Wait();
                userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }
        }
    }
}
