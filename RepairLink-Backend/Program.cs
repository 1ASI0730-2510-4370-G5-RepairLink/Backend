using Microsoft.EntityFrameworkCore;
using RepairLink_Backend.Shared.Domain.Repositories;
using RepairLink_Backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using RepairLink_Backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using RepairLink_Backend.Shared.Interfaces.ASP.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// Add Database Connection
var connectionString = builder.Configuration.GetConnectionString(@"DefaultConnection");

// Verify Database Connection String
if (connectionString is null)
    throw new Exception("Connection string is null");

// Configure Database Context and Logging Levels
if (builder.Environment.IsDevelopment())
    builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseMySQL(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    );
else if (builder.Environment.IsProduction())
    builder.Services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseMySQL(connectionString)
                .LogTo(Console.WriteLine, LogLevel.Error)
                .EnableDetailedErrors();
        }
    );

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// News Bounded Context Injection Configuration
/*builder.Services.AddScoped<IFavoriteSourceRepository, FavoriteSourceRepository>();
builder.Services.AddScoped<IFavoriteSourceQueryService, FavoriteSourceQueryService>();
builder.Services.AddScoped<IFavoriteSourceCommandService, FavoriteSourceCommandService>();*/

var app = builder.Build();

// Verify Database Objects are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();