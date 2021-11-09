using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using System.Linq;
using WestmeathLibraryEMS.Models;
using Microsoft.AspNetCore.Hosting;
using WestmeathLibraryEMS.ViewModels;
using System.Collections.Generic;
using System;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace WestmeathLibraryEMS.Controllers
{
    public class ExportDataController : Controller
    {
        private IRepository repository;



        //variables used for reporting and filtering data result sets
        private decimal? totalCost = 0;
        private int? totalAttendees = 0;
        private int? totalMaxAttendees = 0;
        private decimal? attendeePercentage = 0;
        private int totalNumEvents = 0;
        private int totalMarketingCount = 0;
        private int posterCount = 0;
        private int facebookCount = 0;
        private int twitterCount = 0;
        private int calendarCount = 0;
        private int onlineEventCount = 0;



        private IWebHostEnvironment _hostEnvironment;


        public ExportDataController(IRepository repo, IWebHostEnvironment environment)
        {
            repository = repo;
            _hostEnvironment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }


        //push excel data to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFileAsync(IFormFile file)
        {

            //import license for EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            //get file name and path
            string wwwPath = _hostEnvironment.WebRootPath;
            string contentPath = _hostEnvironment.ContentRootPath;

            //check directory and create if it doesn't exsist
            string path = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);

            FileStream stream = null;
            GC.Collect();
            while (stream == null)
            {
                using (stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {

                    file.CopyTo(stream);
                    stream.Dispose();
                }
            }


            var excelFile = new FileInfo(@"wwwroot\Uploads\" + fileName);

            List<EventFormViewModel> excelEvents = await LoadEvents(excelFile);

            var NumberOfRetries = 3;
            var DelayOnRetry = 1000;

            for (int i = 1; i <= NumberOfRetries; ++i)
            {
                try
                {
                    stream.Close();
                    stream.Dispose();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    System.IO.File.Delete(@"wwwroot\Uploads\" + fileName);
                    break;
                }
                catch (IOException) when (i <= NumberOfRetries)
                {
                    Thread.Sleep(DelayOnRetry);
                }
            };

            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .ToList();

            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .ToList();

            var duplicateChecker = false; //check for duplicates flag variable 
            var rowCounter = 0; //row counter to display number of records inserted

            foreach (var viewModel in excelEvents)
            {

                foreach (var item in eventDay)
                {
                    if (viewModel.Event.EventName.ToLower() == item.Event.EventName.ToLower()
                        && viewModel.EventDay.EventDate == item.EventDate && viewModel.Event.VenueId == item.Event.VenueId
                        )
                        duplicateChecker = true;

                }
                if (duplicateChecker)
                {
                    continue;
                }

                if (!ModelState.IsValid)
                {
                    return RedirectToAction("New", "Events", viewModel);
                }

                if (viewModel.EventDay.Id == 0 && viewModel.Event.Id == 0)
                {

                    viewModel.Event.DateAdded = DateTime.Now;
                    viewModel.EventDay.DateAdded = DateTime.Now;



                    //Id code for events 
                    if (String.IsNullOrWhiteSpace(viewModel.Event.Guid))
                        viewModel.Event.Guid = EventsController.RandomString(8);

                    //new events that have already transpired are set to pending
                    if (viewModel.EventDay.EventStatusId == 1 && viewModel.EventDay.EventDate < DateTime.Now)
                        viewModel.EventDay.EventStatusId = 3;


                    if (viewModel.Event.Requirements == null)
                        viewModel.Event.Requirements = "no requirements";




                    if (viewModel != null)
                    {
                        repository.SaveEvent(viewModel.Event);
                        repository.CreateEvent(viewModel.Event);

                        viewModel.EventDay.EventId = viewModel.Event.Id;

                        repository.SaveEventDay(viewModel.EventDay);
                        repository.CreateEventDay(viewModel.EventDay);
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Possible duplicate event");
                        return RedirectToAction("Error", "Shared");
                    }


                }
                rowCounter++;
            }

            ViewBag.rowCounter = rowCounter;


            return View("ImportedData", ViewBag.rowCounter);
        }


        //get events from excel
        private async Task<List<EventFormViewModel>> LoadEvents(FileInfo file)
        {

            List<EventFormViewModel> output = new List<EventFormViewModel>();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                await package.LoadAsync(file);
                var ws = package.Workbook.Worksheets[1];

                int row = 2;
                int col = 1;

                while (string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()) == false)
                {
                    EventFormViewModel e = new EventFormViewModel
                    {
                        Event = new Event(),

                        EventDay = new EventDay(),

                        EventStatuses = repository.EventStatuses.ToList(),
                        EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                        Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                        Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()
                    };

                    var m = new EventMarketingFormViewModel
                    {
                        EventMarketing = new EventMarketing(),
                        Events = repository.Events.ToList(),
                        MarketingTypes = repository.MarketingTypes.ToList()
                    };

                    //process venue names
                    var venueName = ws.Cells[row, 1].Value.ToString().ToLower().Trim();
                    switch (venueName)
                    {
                        case "mgr":
                            e.Event.VenueId = 1;
                            break;

                        case "kln":
                            e.Event.VenueId = 2;
                            break;

                        case "kbn":
                            e.Event.VenueId = 3;
                            break;

                        case "ath":
                            e.Event.VenueId = 4;
                            break;

                        case "cpd":
                            e.Event.VenueId = 5;
                            break;

                        case "mte":
                            e.Event.VenueId = 6;
                            break;

                        default:
                            e.Event.VenueId = 1;
                            break;
                    }

                    //process event dates
                    e.EventDay.EventDate = DateTime.Parse(ws.Cells[row, col + 1].Value.ToString());

                    //process event name
                    e.Event.EventName = ws.Cells[row, col + 2].Value.ToString();

                    //process event type
                    var eventType = ws.Cells[row, col + 3].Value?.ToString().ToLower().Trim();

                    List<EventType> eventTypeInDb = repository.EventTypes.ToList();


                    foreach (var item in eventTypeInDb)
                    {

                        if (item.TypeName.ToLower() == eventType)
                        {
                            int typeId = item.Id;
                            e.Event.EventTypeId = typeId;
                            break;
                        }
                        else
                        {
                            e.Event.EventTypeId = 20;
                        }

                    }

                    //process start time
                    e.EventDay.StartTime = new TimeSpan();
                    e.EventDay.EndTime = new TimeSpan();


                    //process event description
                    e.Event.Description = string.IsNullOrWhiteSpace(ws.Cells[row, col + 6].Value?.ToString()) ? "None Given" : ws.Cells[row, col + 6].Value.ToString();

                    //process event requirements
                    e.Event.Requirements = string.IsNullOrWhiteSpace(ws.Cells[row, col + 7].Value?.ToString()) ? "N/A" : ws.Cells[row, col + 7].Value.ToString();

                    //process ties in with
                    e.Event.TiesInWith = string.IsNullOrWhiteSpace(ws.Cells[row, col + 8].Value?.ToString()) ? "N/A" : ws.Cells[row, col + 8].Value.ToString();


                    //process cost
                    if (string.IsNullOrWhiteSpace(ws.Cells[row, col + 9].Value?.ToString()))
                    {
                        e.Event.Cost = 0;
                    }
                    else
                    {
                        int i = 0;
                        Decimal.Parse(i.ToString());
                        string s = ws.Cells[row, col + 9].Value.ToString();
                        bool result = int.TryParse(s, out i);
                        if (result)
                        {
                            e.Event.Cost = Decimal.Parse(ws.Cells[row, col + 9].Value.ToString());
                        }
                        else
                        {
                            e.Event.Cost = 0;
                        };

                    };

                    //process event status
                    var status = ws.Cells[row, col + 10].Value?.ToString().Trim();
                    switch (status)
                    {
                        case "Completed":
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Closed").Select(e => e.Id).SingleOrDefault();
                            break;

                        case "Booked":
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Upcoming").Select(e => e.Id).SingleOrDefault(); ;
                            break;

                        case "Canceled":
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Canceled").Select(e => e.Id).SingleOrDefault();
                            break;

                        case null:
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Closed").Select(e => e.Id).SingleOrDefault();
                            break;

                        case "":
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Closed").Select(e => e.Id).SingleOrDefault();
                            break;

                        default:
                            e.EventDay.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Closed").Select(e => e.Id).SingleOrDefault();
                            break;
                    }

                    //process facilitator type
                    var facilitator = ws.Cells[row, col + 11].Value?.ToString().Trim().ToLower();

                    switch (facilitator)
                    {
                        case null:
                            e.Event.FacilitatorId = 1;
                            break;

                        case "":
                            e.Event.FacilitatorId = 1;
                            break;

                        case "outside facilitator":
                            e.Event.FacilitatorId = 2;
                            break;

                        case "inhouse":
                            e.Event.FacilitatorId = 1;
                            break;

                        case "in house":
                            e.Event.FacilitatorId = 1;
                            break;

                        default:
                            e.Event.FacilitatorId = 2;
                            break;
                    }

                    //process contact details
                    e.Event.ContactFirstName = string.IsNullOrWhiteSpace(ws.Cells[row, col + 12].Value?.ToString()) ? "no details given" : ws.Cells[row, col + 12].Value?.ToString();
                    e.Event.ContactLastName = string.IsNullOrWhiteSpace(ws.Cells[row, col + 12].Value?.ToString()) ? "no details given" : ws.Cells[row, col + 12].Value?.ToString();
                    e.Event.ContactPhoneNumber = string.IsNullOrWhiteSpace(ws.Cells[row, col + 12].Value?.ToString()) ? "no details given" : ws.Cells[row, col + 12].Value?.ToString();
                    e.Event.ContactEmail = string.IsNullOrWhiteSpace(ws.Cells[row, col + 12].Value?.ToString()) ? "no details given" : ws.Cells[row, col + 12].Value?.ToString();

                    //process if event is booked
                    var booked = ws.Cells[row, col + 13].Value?.ToString().ToLower().Trim();
                    if (string.IsNullOrWhiteSpace(ws.Cells[row, col + 13].Value?.ToString()))
                        booked = "no";

                    switch (booked)
                    {

                        case "no":
                            e.Event.BookedEvent = false;
                            break;

                        case "yes":
                            e.Event.BookedEvent = true;
                            break;

                        default:
                            e.Event.BookedEvent = true;
                            break;

                    }

                    //process maximum attendees
                    if (string.IsNullOrWhiteSpace(ws.Cells[row, col + 14].Value?.ToString()))
                        e.Event.MaxAttendees = 0;
                    else
                    {
                        int i = 0;
                        int.Parse(i.ToString());
                        string s = ws.Cells[row, col + 14].Value.ToString();
                        bool result = int.TryParse(s, out i);
                        e.Event.MaxAttendees = i;
                    }



                    if (e.Event.Id == m.EventMarketing.EventId)
                    {
                        var facebook = ws.Cells[row, col + 15].Value?.ToString().ToLower().Trim();
                        if (!string.IsNullOrWhiteSpace(facebook) || facebook != "n")
                        {
                            var fb = repository.MarketingTypes.Where(e => e.MarketingTypeName == "Facebook").SingleOrDefault();

                            m.EventMarketing.MarketingTypeId = fb.Id;
                        };

                        var twitter = ws.Cells[row, col + 16].Value?.ToString().ToLower().Trim();
                        if (!string.IsNullOrWhiteSpace(twitter) || twitter != "n")
                        {
                            var fb = repository.MarketingTypes.Where(e => e.MarketingTypeName == "Twitter").SingleOrDefault();

                            m.EventMarketing.MarketingTypeId = fb.Id;

                        }

                        var calendar = ws.Cells[row, col + 17].Value?.ToString().ToLower().Trim();
                        if (!string.IsNullOrWhiteSpace(calendar) || calendar != "n")
                        {
                            var fb = repository.MarketingTypes.Where(e => e.MarketingTypeName == "Calendar").SingleOrDefault();

                            m.EventMarketing.MarketingTypeId = fb.Id;

                        }

                        var poster = ws.Cells[row, col + 18].Value?.ToString().ToLower().Trim();
                        if (!string.IsNullOrWhiteSpace(poster) || poster != "n")
                        {
                            var fb = repository.MarketingTypes.Where(e => e.MarketingTypeName == "Poster").SingleOrDefault();

                            m.EventMarketing.MarketingTypeId = fb.Id;

                        }
                    }
                    //process event attendance 
                    if (string.IsNullOrWhiteSpace(ws.Cells[row, col + 19].Value?.ToString()))
                        e.EventDay.ActualAttendees = 0;
                    else
                    {
                        int i = 0;
                        int.Parse(i.ToString());
                        string s = ws.Cells[row, col + 19].Value.ToString();
                        bool result = int.TryParse(s, out i);
                        e.EventDay.ActualAttendees = i;
                    }

                    //process event feedback
                    e.EventDay.Feedback = string.IsNullOrWhiteSpace(ws.Cells[row, col + 20].Value?.ToString()) ? "None Given" : ws.Cells[row, col + 20].Value.ToString();

                    output.Add(e);
                    row += 1;
                }

                ws.Dispose();
                GC.Collect();
                return output;
            }

        }


        //reporting services
        [HttpPost]
        public IActionResult MetricsReport(DateTime to, DateTime from, string venue, string eventType, string eventStatus, string tiesInWith, bool cost, bool attendees, bool marketing, bool online)
        {
            var viewModel = repository.EventDays
              .Include(e => e.Event)
              .Include(e => e.Event.Facilitator)
              .Include(e => e.Event.EventType)
              .Include(e => e.Event.Venue)
              .Include(e => e.EventStatus)

              .Where(e => e.EventDate >= from && e.EventDate <= to)
              .ToList();

            if (venue != "All")
            {
                viewModel = viewModel.Where(e => e.Event.Venue.VenueName == venue).ToList();
            }

            if (eventType != "All")
            {
                viewModel = viewModel.Where(e => e.Event.EventType.TypeName == eventType).ToList();
            }

            if (eventStatus != "All")
            {
                viewModel = viewModel.Where(e => e.EventStatus.EventStatusName == eventStatus).ToList();
            }

            if (tiesInWith != "All")
            {
                viewModel = viewModel.Where(e => e.Event.TiesInWith == tiesInWith).ToList();
            }

            if (cost == true)
            {
                foreach (var item in viewModel)
                {
                    if (item.Event.Cost == null)
                    {
                        item.Event.Cost = 0;
                    }
                    totalCost += item.Event.Cost;
                }
            }


            if (attendees == true)
            {
                foreach (var item in viewModel)
                {
                    if (item.ActualAttendees == null)
                    {
                        item.ActualAttendees = 0;
                    }
                    totalAttendees += item.ActualAttendees;
                }
            }


            foreach (var item in viewModel)
            {
                if (item.Event.MaxAttendees != 0)
                {

                    attendeePercentage += Convert.ToDecimal(item.ActualAttendees) / Convert.ToDecimal(item.Event.MaxAttendees);

                    totalMaxAttendees += item.Event.MaxAttendees;
                }

                totalNumEvents++;
            }



            if (marketing == true)
            {
                var eventmarketing = repository.EventMarketings.ToList();

                foreach (var item in eventmarketing)
                {
                    foreach (var inner in viewModel)
                    {
                        if (item.EventId == inner.EventId)
                        {
                            totalMarketingCount++;
                            switch (item.MarketingTypeId)
                            {
                                case 1:
                                    posterCount++;
                                    break;
                                case 2:
                                    calendarCount++;
                                    break;
                                case 3:
                                    twitterCount++;
                                    break;
                                case 4:
                                    facebookCount++;
                                    break;
                                default:
                                    break;

                            }
                        }
                    }
                }
            }

            if (online == true)
            {
                foreach (var item in viewModel)
                {
                    if (item.Event.OnlineEvent == true)
                    {
                        onlineEventCount++;
                    }

                }
            }

            ViewBag.TotalNumEvents = totalNumEvents;
            ViewBag.TotalCost = totalCost;
            ViewBag.TotalAttendees = totalAttendees;
            ViewBag.Venue = venue;
            ViewBag.EventType = eventType;
            ViewBag.eventStatus = eventStatus;

            ViewBag.Dates = from.ToString("dd/MM/yyyy") + " - " + to.ToString("dd/MM/yyyy");
            ViewBag.AttendeePercentage = attendeePercentage * 100;

            ViewBag.Max = totalMaxAttendees;
            ViewBag.MarketingCount = totalMarketingCount;

            ViewBag.Poster = posterCount;
            ViewBag.Calendar = calendarCount;
            ViewBag.Twitter = twitterCount;
            ViewBag.Facebook = facebookCount;

            ViewBag.Online = onlineEventCount;

            ViewBag.TiesInWith = tiesInWith;
            return View("MetricsReport", viewModel);
        }

        //exporting data to excel
        [HttpPost]
        public IActionResult ExportDataToFile(DateTime from, DateTime to, string venue, string eventType, string eventStatus, string tiesInWith)
        {

            var dictioneryexportType = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            var exportType = dictioneryexportType["Export"];
            var eEvents = GetEventDetails(to, from, venue, eventType, eventStatus, tiesInWith);
            ExportToExcel(eEvents);
            return null;
        }

        private void ExportToExcel(DataTable events)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Events");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "GUID";
                worksheet.Cell(currentRow, 3).Value = "Venue";
                worksheet.Cell(currentRow, 4).Value = "Date";
                worksheet.Cell(currentRow, 5).Value = "Title";
                worksheet.Cell(currentRow, 6).Value = "Type of event";
                worksheet.Cell(currentRow, 7).Value = "Start time";
                worksheet.Cell(currentRow, 8).Value = "End time";
                worksheet.Cell(currentRow, 9).Value = "Description";
                worksheet.Cell(currentRow, 10).Value = "Requirements";
                worksheet.Cell(currentRow, 11).Value = "Ties-in with";
                worksheet.Cell(currentRow, 12).Value = "Cost";
                worksheet.Cell(currentRow, 13).Value = "Event status";
                worksheet.Cell(currentRow, 14).Value = "Run by";
                //worksheet.Cell(currentRow, 15).Value = "Contact";
                worksheet.Cell(currentRow, 16).Value = "Booking";
                worksheet.Cell(currentRow, 16).Value = "Online";
                worksheet.Cell(currentRow, 17).Value = "Max bookings allowed";
                worksheet.Cell(currentRow, 18).Value = "Facebook";
                worksheet.Cell(currentRow, 19).Value = "Twitter";
                worksheet.Cell(currentRow, 20).Value = "Calendar";
                worksheet.Cell(currentRow, 21).Value = "Poster";
                worksheet.Cell(currentRow, 22).Value = "Attendance";
                worksheet.Cell(currentRow, 23).Value = "Feedback";

                worksheet.Row(1).Style.Fill.SetBackgroundColor(XLColor.LightGreen);
                worksheet.Row(1).Style.Font.SetBold();
                worksheet.Row(1).Style.Font.FontSize = 15;
                worksheet.Columns("A", "W").AdjustToContents();


                for (int i = 0; i < events.Rows.Count; i++)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = events.Rows[i]["Id"];
                    worksheet.Cell(currentRow, 2).Value = events.Rows[i]["GUID"];
                    worksheet.Cell(currentRow, 3).Value = events.Rows[i]["Venue"];
                    worksheet.Cell(currentRow, 4).Value = events.Rows[i]["Date"];
                    worksheet.Cell(currentRow, 5).Value = events.Rows[i]["Title"];
                    worksheet.Cell(currentRow, 6).Value = events.Rows[i]["Type of event"];
                    worksheet.Cell(currentRow, 7).Value = events.Rows[i]["Start time"];
                    worksheet.Cell(currentRow, 8).Value = events.Rows[i]["End time"];
                    worksheet.Cell(currentRow, 9).Value = events.Rows[i]["Description"];
                    worksheet.Cell(currentRow, 10).Value = events.Rows[i]["Requirements"];
                    worksheet.Cell(currentRow, 11).Value = events.Rows[i]["Ties-in with"];
                    worksheet.Cell(currentRow, 12).Value = events.Rows[i]["Cost"];
                    worksheet.Cell(currentRow, 13).Value = events.Rows[i]["Event status"];
                    worksheet.Cell(currentRow, 14).Value = events.Rows[i]["Run by"];
                    //worksheet.Cell(currentRow, 15).Value = events.Rows[i]["Contact"];
                    worksheet.Cell(currentRow, 16).Value = events.Rows[i]["Booking?"];
                    worksheet.Cell(currentRow, 16).Value = events.Rows[i]["Online?"];
                    worksheet.Cell(currentRow, 17).Value = events.Rows[i]["Max bookings allowed"];
                    worksheet.Cell(currentRow, 18).Value = events.Rows[i]["Facebook"];
                    worksheet.Cell(currentRow, 19).Value = events.Rows[i]["Twitter"];
                    worksheet.Cell(currentRow, 20).Value = events.Rows[i]["Calendar"];
                    worksheet.Cell(currentRow, 21).Value = events.Rows[i]["Poster"];
                    worksheet.Cell(currentRow, 22).Value = events.Rows[i]["Attendance"];
                    worksheet.Cell(currentRow, 23).Value = events.Rows[i]["Feedback"];

                }
                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                var content = stream.ToArray();

                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=EventDetails.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);

                Response.Body.Flush();

            }
        }

        //get event details based on criteria
        private DataTable GetEventDetails(DateTime to, DateTime from, string venue, string eventType, string eventStatus, string tiesInWith)
        {
            var dataSet = repository.EventDays
             .Include(e => e.Event)
             .Include(e => e.Event.Facilitator)
             .Include(e => e.Event.EventType)
             .Include(e => e.Event.Venue)
             .Include(e => e.EventStatus)
             .Where(e => e.EventDate >= from && e.EventDate <= to)
             .ToList()
             .GroupJoin(repository.EventMarketings,
                e => e.EventId, m => m.EventId,
                (events, marketing) => new
                {
                    Events = events,
                    Marketing = marketing.Select(m => m.MarketingTypeId)
                });

            if (venue != "All")
            {
                dataSet = dataSet.Where(e => e.Events.Event.Venue.VenueName == venue);
            }

            if (eventType != "All")
            {
                dataSet = dataSet.Where(e => e.Events.Event.EventType.TypeName == eventType);
            }

            if (eventStatus != "All")
            {
                dataSet = dataSet.Where(e => e.Events.EventStatus.EventStatusName == eventStatus);
            }

            if (tiesInWith != "All")
            {
                dataSet = dataSet.Where(e => e.Events.Event.TiesInWith == tiesInWith);
            }

            DataTable dtEvents = new DataTable("EventDetails");

            dtEvents.Columns.AddRange(new DataColumn[23]
            {
                new DataColumn("Id"),
                new DataColumn("GUID"),
                new DataColumn("Venue"),
                new DataColumn("Date"),
                new DataColumn("Title"),
                new DataColumn("Type of event"),
                new DataColumn("Start time"),
                new DataColumn("End time"),
                new DataColumn("Description"),
                new DataColumn("Requirements"),
                new DataColumn("Ties-in with"),
                new DataColumn("Cost"),
                new DataColumn("Event status"),
                new DataColumn("Run by"),
                //new DataColumn("Contact"),
                new DataColumn("Booking?"),
                new DataColumn("Online?"),
                new DataColumn("Max bookings allowed"),
                new DataColumn("Facebook"),
                new DataColumn("Twitter"),
                new DataColumn("Calendar"),
                new DataColumn("Poster"),
                new DataColumn("Attendance"),
                new DataColumn("Feedback"),

            });

            foreach (var items in dataSet)
            {
                dtEvents.Rows.Add(
                    items.Events.Id,
                    items.Events.Event.Guid,
                    items.Events.Event.Venue.VenueName,
                    items.Events.EventDate,
                    items.Events.Event.EventName,
                    items.Events.Event.EventType.TypeName,
                    items.Events.StartTime,
                    items.Events.EndTime,
                    items.Events.Event.Description,
                    items.Events.Event.Requirements,
                    items.Events.Event.TiesInWith,
                    items.Events.Event.Cost,
                    items.Events.EventStatus.EventStatusName,
                    items.Events.Event.Facilitator.FacilitatorType,

                    //items.Events.Event.ContactFirstName +
                    //   " " + items.Events.Event.ContactLastName +
                    //   "    " + items.Events.Event.ContactPhoneNumber +
                    //   "    " + items.Events.Event.ContactEmail,

                    items.Events.Event.BookedEvent,
                    items.Events.Event.OnlineEvent,
                    items.Events.Event.MaxAttendees,

                    items.Marketing.Any(e => e.Equals(1)),
                    items.Marketing.Any(e => e.Equals(2)),
                    items.Marketing.Any(e => e.Equals(3)),
                    items.Marketing.Any(e => e.Equals(4)),

                    items.Events.ActualAttendees,
                    items.Events.Feedback
                    );
            }



            return dtEvents;
        }

        //filter duplicate event uploads
        private EventFormViewModel CheckEventDuplicates(EventFormViewModel v)
        {
            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .ToList();



            foreach (var item in eventDay)
            {
                if (v.Event.EventName.ToLower() == item.Event.EventName.ToLower()
                    && v.EventDay.EventDate == item.EventDate
                    && v.Event.VenueId == item.Event.VenueId
                    )
                    return null;
            }
            return v;

        }

        public bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

    }
}
