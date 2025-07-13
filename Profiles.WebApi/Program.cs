using Profiles.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructureServices();

var app = builder.Build();

app.MapControllers();

app.Run();
