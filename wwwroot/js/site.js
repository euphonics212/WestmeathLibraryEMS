



$(document).ready(function () {

    //display end date if multi-day check box is checked and if it unchecked make the end Date  equal to the start date

    if (!$(this).is(":checked")) {
        $('#EventDay_EndDate').attr('readonly', true);
    } else {
        $('#EventDay_EndDate').attr('readonly', false);
    }

    if ($('#EventDay_EndDate').val() > $('#EventDay_EventDate').val()) {
        $('#consecDays').prop('checked', true);
        $('#EventDay_EndDate').attr('readonly', false);

    }

    $('#consecDays').change(function () {
        var date = $('#EventDay_EventDate').val();
        $('#EventDay_EndDate').val(date)

        if (!$(this).is(":checked")) {
            $('#EventDay_EndDate').attr('readonly', true);
        } else {
            $('#EventDay_EndDate').attr('readonly', false);
        }

    });

    $('#EventDay_EventDate').change(function () {
        var date = $(this).val();
        var end = $('#EventDay_EndDate').val();
        if ($('#consecDays').is(":checked")) {

            if (date > end) {
                $('#EventDay_EndDate').val(date)
            }
            // it is checked
        } else {

            $('#EventDay_EndDate').val(date)
        }
        if (date > end) {
            $('#EventDay_EndDate').val(date)
        }

    });


    $('#EventDay_EndDate').change(function () {

        var end = $(this).val();
        var date = $('#EventDay_EventDate').val();

        if ($('#consecDays').is(":checked")) {
            if (date > end) {
                $('#EventDay_EndDate').val(date)
            }
        } else {

            $('#EventDay_EndDate').val(date)
        }
        if (date > end) {
            $('#EventDay_EndDate').val(date)
        }

    });

    //Datatables




    $('#userActivityTable').DataTable({
        "order": [[2, "desc"]],

    });
    $('.table-container').removeClass('dataTableParentHidden');


    $('#eventDeleteListTable').DataTable({
        "order": [[3, "desc"]],
        "columnDefs": [


            { "width": "8%", "targets": 3 },

            { "width": "5%", "targets": 7 },
            { "width": "5%", "targets": 9 }


        ]
    });

    $('#eventCancelledTable').DataTable({
        "order": [[7, "desc"]],
        "columnDefs": [
            { "width": "10%", "targets": 0 },
            { "width": "8%", "targets": 1 },
            { "width": "10%", "targets": 2 },
            { "width": "8%", "targets": 3 },
            { "width": "8%", "targets": 4 },
            { "width": "8%", "targets": 5 },
            { "width": "8%", "targets": 6 },
            { "width": "8%", "targets": 7 },
            { "width": "8%", "targets": 8 }
        ]
    });

    $('#venueTable').DataTable({
        "order": [[0, "desc"]],

    });

    $('#eventTypeTable').DataTable({
        "order": [[0, "desc"]],
        "columnDefs": [
            { "width": "80%", "targets": 0 },
            { "width": "25%", "targets": 1 },
            { "width": "25%", "targets": 2 }
        ]
    });

    $('#facilitatorsTable').DataTable({
        "order": [[0, "desc"]]
    });

    $('#marketingTypesTable').DataTable({
        "order": [[0, "desc"]]
    });

    $('#eventStatusTable').DataTable({
        "order": [[0, "desc"]]
    });

    $('#eventUpTable').DataTable({
        "order": [[8, "desc"]],
        "columnDefs": [
            { "width": "10%", "targets": 0 },
            { "width": "8%", "targets": 1 },
            { "width": "10%", "targets": 2 },
            { "width": "8%", "targets": 3 },
            { "width": "8%", "targets": 4 },
            { "width": "8%", "targets": 5 },
            { "width": "8%", "targets": 6 },
            { "width": "8%", "targets": 7 },
            { "width": "8%", "targets": 8 },
            { "width": "10%", "targets": 9 }
        ]
    });

    $('#eventAllTable').DataTable({
        "order": [[3, "desc"]],
        "columnDefs": [
            { "width": "10%", "targets": 0 },
            { "width": "10%", "targets": 1 },
            { "width": "10%", "targets": 2 },
            { "width": "6%", "targets": 3 },
            { "width": "6%", "targets": 4 },
            { "width": "6%", "targets": 5 },
            { "width": "6%", "targets": 6 },
            { "width": "8%", "targets": 7 },
            { "width": "8%", "targets": 8 },
            { "width": "8%", "targets": 9 },
            { "width": "8%", "targets": 10 },
            { "width": "5%", "targets": 11 },
            { "width": "7%", "targets": 12 },


        ]
    });

    $('#eventMarketingTable').DataTable({
        "order": [[3, "desc"]]
    });
    $('#eventMarketingTableDelete').DataTable({
        "order": [[4, "desc"]]
    });

    $('#eventPendingTable').DataTable({
        "order": [[3, "desc"]],
        "columnDefs": [
            { "width": "8%", "targets": 0 },
            { "width": "8%", "targets": 1 },
            { "width": "8%", "targets": 2 },
            { "width": "8%", "targets": 3 },
            { "width": "8%", "targets": 4 },
            { "width": "8%", "targets": 5 },
            { "width": "8%", "targets": 6 },
            { "width": "15%", "targets": 7 },



        ]

    });
    $('#eventHomeTable').DataTable({
        "order": [[3, "desc"]],
        "pageLength": 3
    });


    /*____________________________________________*/

    //$('#actionlink-add-container').hide();
    //$('#actionlink-view-container').hide();
    //$('#actionlink-upcoming-container').hide();
    //$('#actionlink-closed-container').hide();
    //$('#actionlink-pending-container').hide();
    //$('#actionlink-canceled-container').hide();


    $('#actionlink-add-marketing-container').hide();
    $('#actionlink-view-marketing-container').hide();
    $('#actionlink-reports-container').hide();
    $('#actionlink-delete-container').hide();
    $('#actionlink-restore-container').hide();
    $('#actionlink-maintenance-container').hide();


    $('#actionlink-event-types-container').hide();
    $('#actionlink-marketing-types-container').hide();
    $('#actionlink-facilitators-container').hide();
    $('#actionlink-status-container').hide();
    $('#actionlink-log-container').hide();
    $('#actionlink-setup-container').hide();

    /*____________________________________________*/

    /*Setup: -- add marketing description box*/
    $(".actionlink-event-types").mouseenter(function () {

        $('#actionlink-event-types-container').fadeIn(10)

    });
    $(".actionlink-event-types").mouseleave(function () {

        $('#actionlink-event-types-container').fadeOut(10)

    });


    $(".actionlink-marketing-types").mouseenter(function () {

        $('#actionlink-marketing-types-container').fadeIn(10)

    });
    $(".actionlink-marketing-types").mouseleave(function () {

        $('#actionlink-marketing-types-container').fadeOut(10)

    });


    $(".actionlink-facilitators").mouseenter(function () {

        $('#actionlink-facilitators-container').fadeIn(10)

    });
    $(".actionlink-facilitators").mouseleave(function () {

        $('#actionlink-facilitators-container').fadeOut(10)

    });


    $(".actionlink-status").mouseenter(function () {

        $('#actionlink-status-container').fadeIn(10)

    });
    $(".actionlink-status").mouseleave(function () {

        $('#actionlink-status-container').fadeOut(10)

    });


    $(".actionlink-log").mouseenter(function () {

        $('#actionlink-log-container').fadeIn(10)

    });
    $(".actionlink-log").mouseleave(function () {

        $('#actionlink-log-container').fadeOut(10)

    });

    /*____________________________________________*/

    /*Admin: -- default admin description box*/
    $(".admin-btn-menu").mouseenter(function () {

        $('.admin-actionlink-default').hide(200)

    });
    $(".admin-btn-menu").mouseleave(function () {

        $('.admin-actionlink-default').show(200)

    });



    /*Admin: -- add marketing description box*/
    $(".actionlink-add-marketing").mouseenter(function () {

        $('#actionlink-add-marketing-container').fadeIn(10)

    });
    $(".actionlink-add-marketing").mouseleave(function () {

        $('#actionlink-add-marketing-container').fadeOut(10)

    });


    /*Admin: -- add marketing description box*/
    $(".actionlink-view-marketing").mouseenter(function () {

        $('#actionlink-view-marketing-container').fadeIn(10)

    });
    $(".actionlink-view-marketing").mouseleave(function () {

        $('#actionlink-view-marketing-container').fadeOut(10)

    });


    /*Admin: -- reports description box*/
    $(".actionlink-reports").mouseenter(function () {

        $('#actionlink-reports-container').fadeIn(10)

    });
    $(".actionlink-reports").mouseleave(function () {

        $('#actionlink-reports-container').fadeOut(10)

    });


    /*Admin: -- Delete events description box*/
    $(".actionlink-delete-events").mouseenter(function () {

        $('#actionlink-delete-container').fadeIn(10)

    });
    $(".actionlink-delete-events").mouseleave(function () {

        $('#actionlink-delete-container').fadeOut(10)

    });

    /*Admin: -- restore events description box*/
    $(".actionlink-restore-events").mouseenter(function () {

        $('#actionlink-restore-container').fadeIn(10)

    });
    $(".actionlink-restore-events").mouseleave(function () {

        $('#actionlink-restore-container').fadeOut(10)

    });



    /*Admin: -- restore events description box*/
    $(".actionlink-sys-setup").mouseenter(function () {

        $('#actionlink-setup-container').fadeIn(10)

    });
    $(".actionlink-sys-setup").mouseleave(function () {

        $('#actionlink-setup-container').fadeOut(10)

    });
    /*____________________________________________*/

    /*Events: -- default events description box*/
    $(".events-btn-menu").mouseenter(function () {

        $('.events-actionlink-default').hide(200)

    });
    $(".events-btn-menu").mouseleave(function () {

        $('.events-actionlink-default').show(200)

    });


    /*Events: -- add events description box*/
    $(".actionlink-add").mouseenter(function () {

        $("#actionlink-add-container").fadeIn(10);

    });
    $(".actionlink-add").mouseleave(function () {

        $("#actionlink-add-container").fadeOut(10);
    });



    /*Events: -- View events description box*/
    $(".actionlink-view").mouseenter(function () {

        $("#actionlink-view-container").fadeIn(10);

    });
    $(".actionlink-view").mouseleave(function () {

        $("#actionlink-view-container").fadeOut(10);

    });



    /*Events: -- upcoming events description box*/
    $(".actionlink-upcoming").mouseenter(function () {

        $("#actionlink-upcoming-container").fadeIn(10);

    });
    $(".actionlink-upcoming").mouseleave(function () {

        $("#actionlink-upcoming-container").fadeOut(10);

    });



    /*Events: -- closed events description box*/
    $(".actionlink-closed").mouseenter(function () {

        $("#actionlink-closed-container").fadeIn(10);

    });
    $(".actionlink-closed").mouseleave(function () {

        $("#actionlink-closed-container").fadeOut(10);

    });



    /*Events: -- pending items description box*/
    $(".actionlink-pending").mouseenter(function () {

        $("#actionlink-pending-container").fadeIn(10);

    });
    $(".actionlink-pending").mouseleave(function () {

        $("#actionlink-pending-container").fadeOut(10);

    });



    /*Events: --  canceled events description box*/
    $(".actionlink-canceled").mouseenter(function () {

        $("#actionlink-canceled-container").fadeIn(10);

    });
    $(".actionlink-canceled").mouseleave(function () {

        $("#actionlink-canceled-container").fadeOut(10);

    });


    var venueName = "All";

    $("#type_filter").change(function () {
        venueName = $("#type_filter option:selected").text();
        $("#tester").text(venueName);
        $('#calendar').fullCalendar('rerenderEvents');


    })



    $('#calendar').fullCalendar({

        header:
        {
            left: 'prev,next today',
            center: 'title',
            right: '',
            allDay: true
        },

        eventSources: {
            url: '/home/GetEventDates',
            type: 'GET',
            error: function () {
                alert("error");
            },




            failure: function () {
                alert('there was an error while fetching events!');
            },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {




                    if (data[i].status == "Upcoming") {//If event time is in the past change the general event background & border color
                        data[i]["backgroundColor"] = "#CFF3B4";
                        data[i]["borderColor"] = "#CFF3B4";
                    }
                    if (data[i].status == "Pending") {
                        data[i]["backgroundColor"] = "#F3E4B4";
                        data[i]["borderColor"] = "#F3E4B4";
                    }

                    if (data[i].status == "Canceled") {
                        data[i]["backgroundColor"] = "#d9d7d2";
                        data[i].title += " - canceled"
                    }

                    if (data[i].status == "Closed") {
                        data[i]["backgroundColor"] = "#d3d9ed";
                        data[i].title += " - canceled"
                    }

                }
                console.log("successfully loaded");
            },

        },

        eventRender: function (event, element, view) {
            if (event.venue != venueName) {
                $(element).css("display", "none");
            } else {
                $(element).css("display", "normal");
            }

            if (venueName == "All") {
                $(element).css("display", "block");
            }

        },

        eventLimit: true, // for all non-TimeGrid views

        views: {
            timeGrid: {
                eventLimit: 6 // adjust to 6 only for timeGridWeek/timeGridDay
            }
        },


        eventClick: function (event) {

            window.location = "/Events/Details/" + event.id; /*+ JSON.stringify(event.id);*/
        },

        displayEventTime: false,
        //selectable: false,
        //selectHelper: false,
        //editable: false,
        //eventLimit: true,
        //allDay: false,

    });


    //tool tips 
    $("#calendarBtn").attr('title', 'View the event calendar on the homepage');
    $("#addneweventBtn").attr('title', 'Add new events to the EMS database');
    $("#pendingItemsBtn").attr('title', 'View or close pending events');
    $("#allEventsBtn").attr('title', 'View all events or add non-consecutive dates for an event');
    $("#upcomingBtn").attr('title', 'Cancel an upcoming event');
    $("#addmarketingBtn").attr('title', 'Add a marketing item to be associated with an event ');
    $("#deletemarketingBtn").attr('title', 'Delete a marketing item associated with an event  ');
    $("#viewmarketingBtn").attr('title', 'View all marketing items associated with events  ');
    $("#deleteEventsBtn").attr('title', 'Delete an event  ');
    $("#GenerateReportsBtn").attr('title', 'Generate reports on events or export data to Excel.');
    $("#systemSetupBtn").attr('title', 'Add event types, venues, and view system changes in the activity log ');
    $("#helpBtn").attr('title', 'View help videos on how to use the EMS application ');


});

function filterFunction() {
    var input, filter, ul, li, a, i;
    input = document.getElementById("eventTxtBox");
    filter = input.value.toUpperCase();
    div = document.getElementById("eventsDropdown");
    a = div.getElementsByTagName("option");
    for (i = 0; i < a.length; i++) {
        txtValue = a[i].textContent || a[i].innerText;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}

function goBack() {
    window.history.back();
}

