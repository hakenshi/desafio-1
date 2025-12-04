using HypeSoft.Infraestructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;

namespace HypeSoft.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .WithUsername("admin")
        .WithPassword("admin123")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(MongoDbContext));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.Configure<MongoDbSettings>(options =>
            {
                options.ConnectionString = _mongoContainer.GetConnectionString();
                options.DatabaseName = "hypesoft_test";
            });

            services.AddSingleton<MongoDbContext>();
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }
}
