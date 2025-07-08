using Microsoft.EntityFrameworkCore;
using RepairLink_Backend.Availability.Application.Internal.CommandServices;
using RepairLink_Backend.Availability.Application.Internal.QueryServices;
using RepairLink_Backend.Availability.Domain.Repositories;
using RepairLink_Backend.Availability.Domain.Services;
using RepairLink_Backend.Availability.Infrastructure.Repositories;
using RepairLink_Backend.Booking.Application.Internal.CommandServices;
using RepairLink_Backend.Booking.Application.Internal.QueryServices;
using RepairLink_Backend.Booking.Domain.Repositories;
using RepairLink_Backend.Booking.Domain.Services;
using RepairLink_Backend.Booking.Infrastructure.Repositories;
using RepairLink_Backend.LocationRouting.Application.Internal.CommandServices;
using RepairLink_Backend.LocationRouting.Application.Internal.QueryServices;
using RepairLink_Backend.LocationRouting.Domain.Repositories;
using RepairLink_Backend.LocationRouting.Domain.Services;
using RepairLink_Backend.LocationRouting.Infrastructure.Repositories;
using RepairLink_Backend.Notification.Application.Internal.CommandServices;
using RepairLink_Backend.Notification.Application.Internal.QueryServices;
using RepairLink_Backend.Notification.Domain.Repositories;
using RepairLink_Backend.Notification.Domain.Services;
using RepairLink_Backend.Notification.Infrastructure.Repositories;
using RepairLink_Backend.Payment.Application.Internal.CommandServices;
using RepairLink_Backend.Payment.Application.Internal.QueryServices;
using RepairLink_Backend.Payment.Domain.Repositories;
using RepairLink_Backend.Payment.Domain.Services;
using RepairLink_Backend.Payment.Infrastructure.Repositories;
using RepairLink_Backend.ServiceCatalog.Application.Internal.CommandServices;
using RepairLink_Backend.ServiceCatalog.Application.Internal.QueryServices;
using RepairLink_Backend.ServiceCatalog.Domain.Repositories;
using RepairLink_Backend.ServiceCatalog.Domain.Services;
using RepairLink_Backend.ServiceCatalog.Infrastructure.Repositories;
using RepairLink_Backend.Shared.Domain.Repositories;
using RepairLink_Backend.Shared.Infrastructure.Persistence.EFC.Configuration;
using RepairLink_Backend.Shared.Infrastructure.Persistence.EFC.Repositories;
using RepairLink_Backend.Shared.Interfaces.ASP.Configuration;
using RepairLink_Backend.UserManagement.Application.Internal.CommandServices;
using RepairLink_Backend.UserManagement.Application.Internal.QueryServices;
using RepairLink_Backend.UserManagement.Domain.Repositories;
using RepairLink_Backend.UserManagement.Domain.Services;
using RepairLink_Backend.UserManagement.Infrastructure.Repositories;

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

//Users BC Injection Configuration
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersQueryService, UsersQueryService>();
builder.Services.AddScoped<IUsersCommandService, UsersCommandService>();

//Addresses BC Injection Configuration
builder.Services.AddScoped<IAddressesRepository, AddressesRepository>();
builder.Services.AddScoped<IAddressesQueryService, AddressQueryService>();
builder.Services.AddScoped<IAddressesCommandService, AddressCommandService>();

//Service BC Injection Configuration
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();
builder.Services.AddScoped<IServicesQueryService, ServicesQueryService>();
builder.Services.AddScoped<IServicesCommandService, ServicesCommandService>();

//Notification BC Injection Configuration
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationsQueryService, NotificationQueryService>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();

//Payment BC Injection Configuration
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentQueryService, PaymentQueryService>();
builder.Services.AddScoped<IPaymentCommandService, PaymentCommandService>();

//Booking BC Injection Configuration
builder.Services.AddScoped<IBookingRepository, BookingsRepository>();
builder.Services.AddScoped<IBookingsQueryService, BookingQueryService>();
builder.Services.AddScoped<IBookingsCommandService, BookingCommandService>();

//Availability BC Injection Configuration
builder.Services.AddScoped<IAvailabilityDayRepository, AvailabilityDayRepository>();
builder.Services.AddScoped<IAvailabilityDayQueryService, AvailabilityDayQueryService>();
builder.Services.AddScoped<IAvailabilityDayCommandService, AvailabilityDayCommandService>();
builder.Services.AddScoped<IAvailabilitySlotRepository, AvailabilitySlotsRepository>();
builder.Services.AddScoped<IAvailabilitySlotQueryService, AvailabilitySlotQueryService>();
builder.Services.AddScoped<IAvailabilitySlotCommandService, AvailabilitySlotCommandService>();

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();