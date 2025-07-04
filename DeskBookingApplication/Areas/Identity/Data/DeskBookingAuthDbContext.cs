﻿using DeskBookingApplication.Areas.Identity.Data;
using DeskBookingApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeskBookingApplication.Data;

public class DeskBookingAuthDbContext : IdentityDbContext<DeskBookingApplicationUser>
{
    public DeskBookingAuthDbContext(DbContextOptions<DeskBookingAuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<Desk> Desks { get; set; }
    public DbSet<DeskBooking> DeskBookings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
