using System.Data;
using Microsoft.Data.SqlClient;
using NexusCortex.Application.Interfaces;
using NexusCortex.Application.Services;
using NexusCortex.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5000", "https://localhost:5001", "http://localhost:5124", "https://localhost:7124", "http://localhost:5123", "http://localhost:5200") // Added some standard blazor ports just in case
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=NexusCortex;Trusted_Connection=True;";

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Services.AddScoped<INodeRepository, NodeRepository>();
builder.Services.AddScoped<IRelationshipRepository, RelationshipRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<INodeService, NodeService>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IMomentumService, MomentumService>();
builder.Services.AddScoped<IStagnationService, StagnationService>();

var app = builder.Build();

app.UseCors("AllowBlazorOrigin");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();