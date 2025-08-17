// Program.cs
// This single file contains our entire C# backend.
// It uses ASP.NET Core Minimal APIs to create a web server and a SignalR hub.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// 1. --- Define a Hub for real-time communication ---
// This hub is the connection point for our frontend.
// The frontend will connect to "/locationHub".
public class LocationHub : Hub
{
    // We can add methods here to be called by clients, but for this project,
    // the server only pushes data, so we don't need any.
}

// 2. --- Define our data models ---
// A simple class to represent a vehicle.
public class Vehicle
{
    public int Id { get; set; }
    public string DriverName { get; set; }
    public string Model { get; set; }
    public string Status { get; set; } // e.g., "In-Transit", "Parked"
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

// 3. --- Create a background service to simulate vehicle movement ---
// This service will run in the background for the lifetime of the application.
// It simulates our fleet of vehicles moving around.
public class VehicleSimulator : BackgroundService
{
    private readonly IHubContext<LocationHub> _hubContext;
    private readonly List<Vehicle> _vehicles;
    private readonly Random _random = new Random();

    public VehicleSimulator(IHubContext<LocationHub> hubContext)
    {
        _hubContext = hubContext;

        // Initialize our fleet with some sample data.
        // In a real application, this would come from a SQL database.
        _vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, DriverName = "John Doe", Model = "Volvo VNL 860", Status = "In-Transit", Latitude = 34.0522, Longitude = -118.2437 },
            new Vehicle { Id = 2, DriverName = "Jane Smith", Model = "Freightliner Cascadia", Status = "In-Transit", Latitude = 40.7128, Longitude = -74.0060 },
            new Vehicle { Id = 3, DriverName = "Mike Ross", Model = "Kenworth T680", Status = "Parked", Latitude = 41.8781, Longitude = -87.6298 },
            new Vehicle { Id = 4, DriverName = "Rachel Zane", Model = "Peterbilt 579", Status = "In-Transit", Latitude = 29.7604, Longitude = -95.3698 },
            new Vehicle { Id = 5, DriverName = "Harvey Specter", Model = "Mack Anthem", Status = "Loading", Latitude = 39.9526, Longitude = -75.1652 }
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Send the initial list of vehicles to any new client that connects.
        // This ensures the dashboard is populated immediately.
        await _hubContext.Clients.All.SendAsync("ReceiveInitialVehicles", _vehicles, stoppingToken);
        await Task.Delay(2000, stoppingToken); // Initial delay

        while (!stoppingToken.IsCancellationRequested)
        {
            // Simulate movement for vehicles that are "In-Transit"
            foreach (var vehicle in _vehicles.Where(v => v.Status == "In-Transit"))
            {
                // Move the vehicle by a small random amount
                vehicle.Latitude += (_random.NextDouble() - 0.5) * 0.01;
                vehicle.Longitude += (_random.NextDouble() - 0.5) * 0.01;

                // Broadcast the updated location of this single vehicle to all clients.
                // The frontend will listen for the "UpdateVehicleLocation" event.
                await _hubContext.Clients.All.SendAsync("UpdateVehicleLocation", vehicle, stoppingToken);
            }

            // Wait for 2 seconds before the next update cycle.
            await Task.Delay(2000, stoppingToken);
        }
    }
}

// 4. --- Setup and run the web application ---
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add SignalR and our simulator service to the application's services.
        builder.Services.AddSignalR();
        builder.Services.AddHostedService<VehicleSimulator>();
        
        // Allow Cross-Origin Requests (CORS) for development flexibility.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowAnyOrigin();
            });
        });

        var app = builder.Build();
        
        app.UseCors();

        // This tells the app to look for and serve files like index.html
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Map the hub to the "/locationHub" endpoint.
        app.MapHub<LocationHub>("/locationHub");

        Console.WriteLine("Backend server is running.");
        Console.WriteLine("Navigate to http://localhost:5000 in your browser.");

        app.Run();
    }
}