Real-Time Fleet Management Dashboard
A full-stack web application designed to simulate a real-world logistics dashboard for monitoring a fleet of vehicles in real-time. This project demonstrates a modern, scalable architecture using C#, .NET, Entity Framework Core, and modern frontend technologies.

✨ Features
Live Vehicle Tracking: View the real-time location of all vehicles on an interactive map.

Dynamic Dashboard: Vehicle information cards in the sidebar update instantly with status and location changes.

Real-Time Communication: Utilizes SignalR with WebSockets to push live data from the server to the client, eliminating the need for polling.

Persistent Data: Vehicle data and locations are stored in a SQLite database, managed by Entity Framework Core.

Responsive UI: A clean, modern, and responsive user interface built with HTML5, CSS3, and vanilla JavaScript.

Automated Database Setup: The application automatically creates and seeds the database on first launch using EF Core Migrations.

🛠️ Technology Stack
Backend:

C# & ASP.NET Core

SignalR (for real-time web functionality)

Database:

SQLite

Entity Framework Core (as the Object-Relational Mapper)

Frontend:

HTML5

CSS3 (with animations)

JavaScript (ES6+)

Leaflet.js (for the interactive map)

🚀 Getting Started
Follow these instructions to get a copy of the project up and running on your local machine.

Prerequisites
.NET 6 SDK (or newer)

Installation & Setup
Clone the repository:

git clone https://github.com/your-username/your-repo-name.git

Navigate to the project directory:

cd your-repo-name/Backend

Install EF Core tools (if not already installed):

dotnet tool install --global dotnet-ef

Restore dependencies:

dotnet restore

Run the application:

dotnet run

The application will automatically create the fleet.db database, apply migrations, and seed the initial data.

Open your browser and navigate to http://localhost:5000.

🏛️ Architecture
This project follows a classic 3-Tier Architecture:

Data Tier (SQLite & EF Core): The DataContext.cs file and EF Core migrations define the database schema and handle all data persistence.

Logic Tier (ASP.NET Core & C#): The backend server handles all business logic. A VehicleSimulator background service simulates vehicle movement, and a LocationHub (SignalR) pushes real-time updates to clients.

Presentation Tier (HTML, CSS, JS): A single-page application (SPA) style frontend that connects to the SignalR hub to receive and display live data on an interactive map without page reloads.
