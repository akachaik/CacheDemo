using CacheDemo.Application.Caching;
using CacheDemo.Infrastructure.Caching;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddOptions<CacheSettings>()
    .BindConfiguration(nameof(CacheSettings))
    .Validate(settings =>
    {
        if (!settings.ExpirationInSeconds.HasValue)
        {
            return false;
        }

        return true;
    }, $"'{nameof(CacheSettings)}' section is missing in configuration file")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<CacheSettings>>().Value);

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
