using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


namespace WestmeathLibraryEMS.Models
{
    public class SeedData
    {


        public static void EnsurePopulated(IApplicationBuilder app)
        {
            EMSContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<EMSContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            if (!context.Venues.Any())
            {
                context.Venues.AddRange(
                    new Venue
                    {
                        VenueName = "Mullingar Library",
                        Address = "County Buildings, Mount St, Bellview, Mullingar, Co. Westmeath",
                        Coordinates = "53.5404910999683, -7.344035508103283",
                        Eircode = "N91 T183"
                    },


                     new Venue
                     {
                         VenueName = "Kilbeggan Library",
                         Address = "Kilbeggan Library, Kilbeggan Civic Centre, The Square, Kilbeggan, Co. Westmeath",
                         Coordinates = "53.36743878693228, -7.49599333733989",
                         Eircode = "N91 CHV0"
                     },

                     new Venue
                     {
                         VenueName = "Athlone Library",
                         Address = "Athlone Civic Centre, Church St, Athlone, Co. Westmeath",
                         Coordinates = "53.40197161451588, -7.493517924111086",
                         Eircode = "N37 N625"
                     },
                     new Venue
                     {
                         VenueName = "Castlepollard Library",
                         Address = "Mullingar Road, Castlepollard, Co. Westmeath",
                         Coordinates = "53.690263269928856, -7.298629239187122",
                         Eircode = "N91 T183"
                     },
                      new Venue
                      {
                          VenueName = "Moate Library",
                          Address = "Courthouse, Main St, Cartronkeel, Moate, Co. Westmeath",
                          Coordinates = "53.41587831919484, -7.720204637926014",
                          Eircode = "N37 R3P4"
                      }

                    );

                context.SaveChanges();
            }

            /////////////////////////////

            if (!context.EventTypes.Any())
            {
                context.EventTypes.AddRange(
                    new EventType
                    {
                        TypeName = "StoryTime"
                    },
                    new EventType
                    {
                        TypeName = "Author Visit"
                    },
                    new EventType
                    {
                        TypeName = "Talk"
                    },
                    new EventType
                    {
                        TypeName = "Workshop"
                    },
                    new EventType
                    {
                        TypeName = "Colouring"
                    },
                    new EventType
                    {
                        TypeName = "Visit"
                    },
                    new EventType
                    {
                        TypeName = "School Visit"
                    },
                    new EventType
                    {
                        TypeName = "Support Group"
                    },
                    new EventType
                    {
                        TypeName = "Meeting"
                    },
                    new EventType
                    {
                        TypeName = "Launch"
                    },
                    new EventType
                    {
                        TypeName = "Info Day"
                    },
                    new EventType
                    {
                        TypeName = "Book Club"
                    },
                    new EventType
                    {
                        TypeName = "Quiz"
                    },
                    new EventType
                    {
                        TypeName = "Public Consultation"
                    },
                    new EventType
                    {
                        TypeName = "Reading"
                    },
                    new EventType
                    {
                        TypeName = "Exhibition"
                    },
                    new EventType
                    {
                        TypeName = "Class"
                    },
                    new EventType
                    {
                        TypeName = "Singalong"
                    },
                    new EventType
                    {
                        TypeName = "Prize Giving"
                    },
                    new EventType
                    {
                        TypeName = "Other"
                    }
               );
                context.SaveChanges();
            }

            ///////////////////////////////
            ///
            if (!context.Facilitators.Any())
            {
                context.Facilitators.AddRange(
                    new Facilitator
                    {
                        FacilitatorType = "Inhouse"
                    },
                    new Facilitator
                    {
                        FacilitatorType = "Outside"
                    }

                );
                context.SaveChanges();
            }

            ///////////////////////////////
            ///
            if (!context.MarketingTypes.Any())
            {
                context.MarketingTypes.AddRange(
                    new MarketingType
                    {
                        MarketingTypeName = "Facebook"
                    },
                    new MarketingType
                    {
                        MarketingTypeName = "Twitter"
                    },

                    new MarketingType
                    {
                        MarketingTypeName = "Calendar"
                    },
                    new MarketingType
                    {
                        MarketingTypeName = "Poster"
                    }
                );
                context.SaveChanges();
            }

            if (!context.EventStatuses.Any())
            {
                context.EventStatuses.AddRange(
                    new EventStatus
                    {
                        EventStatusName = "Upcoming"
                    },
                    new EventStatus
                    {

                        EventStatusName = "Canceled"
                    },
                    new EventStatus
                    {

                        EventStatusName = "Pending"
                    },
                    new EventStatus
                    {

                        EventStatusName = "Closed"
                    }

                );
                context.SaveChanges();
            }
        }
    }
}