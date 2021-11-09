using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Migrations;

namespace WestmeathLibraryEMS.Models
{
    public class EMSContext : DbContext
    {

        public EMSContext(DbContextOptions<EMSContext> options) : base(options) { }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Facilitator> Facilitators { get; set; }
        public DbSet<MarketingType> MarketingTypes { get; set; }
        public DbSet<EventStatus> EventStatuses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventDay> EventDays { get; set; }
        public DbSet<EventMarketing> EventMarketings { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }


    }
}
