// // Program.cs
// // This single file contains our entire C# backend.
// // It uses ASP.NET Core Minimal APIs to create a web server and a SignalR hub.

// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;

// // 1. --- Define a Hub for real-time communication ---
// // This hub is the connection point for our frontend.
// // The frontend will connect to "/locationHub".
// public class LocationHub : Hub
// {
//     // We can add methods here to be called by clients, but for this project,
//     // the server only pushes data, so we don't need any.
// }

// // 2. --- Define our data models ---
// // A simple class to represent a vehicle.
// public class Vehicle
// {
//     public int Id { get; set; }
//     public required string DriverName { get; set; }
//     public required string Model { get; set; }
//     public required string Status { get; set; } // "In-Transit", "Parked", "Loading"
//     public double Latitude { get; set; }
//     public double Longitude { get; set; }
//     public double FuelLevel { get; set; } = 100.0; // Fuel level in percentage
//     public double Speed { get; set; } = 0.0; // Speed in km/h
//     public DateTime LastUpdated { get; set; } = DateTime.Now;
//     public string CurrentLocation { get; set; } = ""; // City/Area name
//     public double Temperature { get; set; } = 25.0; // Engine temperature in Celsius
//     public int LoadCapacity { get; set; } = 100; // Load capacity in percentage
// }

// // 3. --- Create a background service to simulate vehicle movement ---
// // This service will run in the background for the lifetime of the application.
// // It simulates our fleet of vehicles moving around.
// public class VehicleSimulator : BackgroundService
// {
//     private readonly IHubContext<LocationHub> _hubContext;
//     private readonly List<Vehicle> _vehicles;
//     private readonly Random _random = new Random();

//     public VehicleSimulator(IHubContext<LocationHub> hubContext)
//     {
//         _hubContext = hubContext;

//         // Initialize fleet with major Indian cities
//         _vehicles = new List<Vehicle>
//         {
//             // North India
//             new Vehicle { Id = 1, DriverName = "Amit Kumar", Model = "Tata Prima 3718.T", Status = "In-Transit", Latitude = 28.6139, Longitude = 77.2090 }, // Delhi
//             new Vehicle { Id = 2, DriverName = "Rajesh Singh", Model = "BharatBenz 3523R", Status = "Loading", Latitude = 26.8467, Longitude = 80.9462 }, // Lucknow
//             new Vehicle { Id = 3, DriverName = "Gurpreet Singh", Model = "Tata Ultra 3021.T", Status = "In-Transit", Latitude = 30.7333, Longitude = 76.7794 }, // Chandigarh
            
//             // West India
//             new Vehicle { Id = 4, DriverName = "Priya Sharma", Model = "Ashok Leyland 3118", Status = "In-Transit", Latitude = 19.0760, Longitude = 72.8777 }, // Mumbai
//             new Vehicle { Id = 5, DriverName = "Nitin Patel", Model = "Mahindra Blazo X", Status = "Parked", Latitude = 23.0225, Longitude = 72.5714 }, // Ahmedabad
//             new Vehicle { Id = 6, DriverName = "Deepak Patil", Model = "Eicher Pro 3015", Status = "In-Transit", Latitude = 18.5204, Longitude = 73.8567 }, // Pune
            
//             // South India
//             new Vehicle { Id = 7, DriverName = "Rahul Verma", Model = "Tata Signa 4018.S", Status = "Parked", Latitude = 12.9716, Longitude = 77.5946 }, // Bengaluru
//             new Vehicle { Id = 8, DriverName = "Karthik Raja", Model = "BharatBenz 1917R", Status = "Loading", Latitude = 13.0827, Longitude = 80.2707 }, // Chennai
//             new Vehicle { Id = 9, DriverName = "Mohammed Ali", Model = "Ashok Leyland 2820", Status = "In-Transit", Latitude = 17.3850, Longitude = 78.4867 }, // Hyderabad
            
//             // East India
//             new Vehicle { Id = 10, DriverName = "Sunita Singh", Model = "Eicher Pro 6037", Status = "In-Transit", Latitude = 22.5726, Longitude = 88.3639 }, // Kolkata
//             new Vehicle { Id = 11, DriverName = "Manoj Das", Model = "Tata Yodha", Status = "Loading", Latitude = 20.2961, Longitude = 85.8245 }, // Bhubaneswar
//             new Vehicle { Id = 12, DriverName = "Biplab Roy", Model = "Mahindra Furio", Status = "Parked", Latitude = 25.5941, Longitude = 85.1376 }  // Patna
//         };
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // Send initial vehicle list immediately when service starts
//         try
//         {
//             await _hubContext.Clients.All.SendAsync("ReceiveInitialVehicles", _vehicles, stoppingToken);
//             Console.WriteLine($"Sent initial vehicle list: {_vehicles.Count} vehicles");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error sending initial vehicles: {ex.Message}");
//         }

//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 // Update all vehicles with different behaviors
//                 foreach (var vehicle in _vehicles)
//                 {
//                     // Update common properties
//                     vehicle.LastUpdated = DateTime.Now;
                    
//                     // Simulate fuel consumption and temperature changes
//                     if (vehicle.Status == "In-Transit")
//                     {
//                         vehicle.FuelLevel = Math.Max(0, vehicle.FuelLevel - _random.NextDouble() * 0.5);
//                         vehicle.Speed = 40 + _random.NextDouble() * 20; // Speed between 40-60 km/h
//                         vehicle.Temperature = 75 + _random.NextDouble() * 10; // Engine temp 75-85°C
                        
//                         // Move the vehicle
//                         vehicle.Latitude += (_random.NextDouble() - 0.5) * 0.01;
//                         vehicle.Longitude += (_random.NextDouble() - 0.5) * 0.01;
//                     }
//                     else if (vehicle.Status == "Loading")
//                     {
//                         vehicle.LoadCapacity = Math.Min(100, vehicle.LoadCapacity + _random.Next(1, 5));
//                         vehicle.Speed = 0;
//                         vehicle.Temperature = 30 + _random.NextDouble() * 5; // Engine temp 30-35°C
//                     }
//                     else // Parked
//                     {
//                         vehicle.Speed = 0;
//                         vehicle.Temperature = 25 + _random.NextDouble() * 5; // Engine temp 25-30°C
//                     }

//                     // Randomly change status (small chance)
//                     if (_random.NextDouble() < 0.02) // 2% chance
//                     {
//                         var statuses = new[] { "In-Transit", "Parked", "Loading" };
//                         vehicle.Status = statuses[_random.Next(statuses.Length)];
//                     }

//                     // Send update
//                     await _hubContext.Clients.All.SendAsync("UpdateVehicleLocation", vehicle, stoppingToken);
//                     Console.WriteLine($"Updated vehicle {vehicle.Id}: Status={vehicle.Status}, Speed={vehicle.Speed:F1}km/h, Fuel={vehicle.FuelLevel:F1}%");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Error in VehicleSimulator: {ex.Message}");
//             }

//             // Wait before next update
//             await Task.Delay(2000, stoppingToken);
//         }
//     }
// }

// // 4. --- Setup and run the web application ---
// public class Program
// {
//     public static void Main(string[] args)
//     {
//         var builder = WebApplication.CreateBuilder(args);

//         // Add SignalR and our simulator service to the application's services.
//         builder.Services.AddSignalR();
//         builder.Services.AddHostedService<VehicleSimulator>();
        
//         // Configure CORS - Allow all origins for development
//         builder.Services.AddCors(options =>
//         {
//             options.AddDefaultPolicy(builder =>
//             {
//                 builder.SetIsOriginAllowed(origin => true)
//                        .AllowAnyHeader()
//                        .AllowAnyMethod()
//                        .AllowCredentials();
//             });
//         });

//         var app = builder.Build();

//         if (app.Environment.IsDevelopment())
//         {
//             app.UseDeveloperExceptionPage();
//         }

//         // Configure the HTTP request pipeline in the correct order
//         app.UseRouting();
//         app.UseDefaultFiles();
//         app.UseStaticFiles();
//         app.UseCors();

//         // Map the SignalR hub
//         app.MapHub<LocationHub>("/locationHub");

//         // Serve index.html for all routes
//         app.MapGet("/", async context =>
//         {
//             context.Response.ContentType = "text/html";
//             await context.Response.SendFileAsync("wwwroot/index.html");
//         });

//         Console.WriteLine("Backend server is running.");
//         Console.WriteLine("Navigate to http://localhost:5000 in your browser.");

//         app.Run();
//     }
// }




// Program.cs
// This is the updated backend using Entity Framework Core to connect to a SQL Server database.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// 1. --- Define a Hub for real-time communication ---
public class LocationHub : Hub { }

// 2. --- Define our data model (Entity) ---
// This class now maps directly to the "Vehicles" table in our SQL database.
public class Vehicle
{
    public int Id { get; set; }
    public string DriverName { get; set; }
    public string Model { get; set; }
    public string Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

// 3. --- Updated background service to use the database ---
public class VehicleSimulator : BackgroundService
{
    private readonly IHubContext<LocationHub> _hubContext;
    private readonly IServiceProvider _serviceProvider; // Used to access services like the DbContext
    private readonly Random _random = new Random();

    public VehicleSimulator(IHubContext<LocationHub> hubContext, IServiceProvider serviceProvider)
    {
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Create a new scope to get a fresh instance of the DbContext for this operation.
            // This is the correct way to use DbContext in a long-running background service.
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                
                // Get all vehicles that are "In-Transit" from the database.
                var vehiclesToUpdate = await dbContext.Vehicles
                    .Where(v => v.Status == "In-Transit")
                    .ToListAsync(stoppingToken);

                // Broadcast the initial state or current state on an interval
                var allVehicles = await dbContext.Vehicles.ToListAsync(stoppingToken);
                await _hubContext.Clients.All.SendAsync("ReceiveInitialVehicles", allVehicles, stoppingToken);

                foreach (var vehicle in vehiclesToUpdate)
                {
                    // Move the vehicle by a small random amount
                    vehicle.Latitude += (_random.NextDouble() - 0.5) * 0.01;
                    vehicle.Longitude += (_random.NextDouble() - 0.5) * 0.01;

                    // Broadcast the updated location to all clients.
                    await _hubContext.Clients.All.SendAsync("UpdateVehicleLocation", vehicle, stoppingToken);
                }

                // If any vehicles were updated, save the changes to the database.
                if (vehiclesToUpdate.Any())
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            // Wait for 2 seconds before the next update cycle.
            await Task.Delay(2000, stoppingToken);
        }
    }
}

// --- Main Program Setup ---
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 4. Configure Services ---

        // Add the DbContext to the services, configuring it to use SQL Server.
        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddSignalR();
        builder.Services.AddHostedService<VehicleSimulator>();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });

        var app = builder.Build();

        // --- 5. Configure Middleware ---
        app.UseCors();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapHub<LocationHub>("/locationHub");

        // --- 6. Apply Migrations and Seed Data ---
        // This block automatically creates the database and seeds it with initial data on startup.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            // Apply any pending migrations to the database. Creates the DB if it doesn't exist.
            dbContext.Database.Migrate();
            // Seed the database with initial vehicle data if it's empty.
            SeedData(dbContext);
        }

        Console.WriteLine("Backend server is running with SQL Server integration.");
        Console.WriteLine("Navigate to http://localhost:5000 in your browser.");
        app.Run();
    }

    // --- 7. Data Seeding Method ---
    public static void SeedData(DataContext context)
    {
        // Check if the Vehicles table is already populated.
        if (context.Vehicles.Any())
        {
            return; // DB has been seeded
        }

        var vehicles = new Vehicle[]
        {
            new Vehicle { DriverName = "Amit Kumar", Model = "Tata Prima", Status = "In-Transit", Latitude = 28.6139, Longitude = 77.2090 }, // Delhi
            new Vehicle { DriverName = "Priya Sharma", Model = "Ashok Leyland", Status = "In-Transit", Latitude = 19.0760, Longitude = 72.8777 }, // Mumbai
            new Vehicle { DriverName = "Rahul Verma", Model = "Eicher Pro", Status = "Parked", Latitude = 12.9716, Longitude = 77.5946 }, // Bengaluru
            new Vehicle { DriverName = "Sunita Singh", Model = "BharatBenz", Status = "In-Transit", Latitude = 22.5726, Longitude = 88.3639 }, // Kolkata
            new Vehicle { DriverName = "Vikram Rathod", Model = "Mahindra Blazo", Status = "Loading", Latitude = 13.0827, Longitude = 80.2707 }  // Chennai
        };

        context.Vehicles.AddRange(vehicles);
        context.SaveChanges();
    }
}
