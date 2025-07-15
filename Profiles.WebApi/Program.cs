using Profiles.Infrastructure;
using Profiles.Application;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddInfrastructureServices();

builder.Services.AddApplicationServices();

var app = builder.Build();

app.MapControllers();

app.Run();
