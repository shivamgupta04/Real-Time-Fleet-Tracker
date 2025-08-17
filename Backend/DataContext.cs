// DataContext.cs
// This file defines the database session and maps our C# models to database tables.

using Microsoft.EntityFrameworkCore;

// The DbContext is the primary class that coordinates Entity Framework functionality.
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    // This DbSet property will become a table named "Vehicles" in the database.
    public DbSet<Vehicle> Vehicles { get; set; }
}
