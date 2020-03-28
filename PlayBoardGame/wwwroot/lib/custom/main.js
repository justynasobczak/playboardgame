function generateTooltip() {
    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
}

function setDates() {
    $(document).ready(function () {
        $('#startDatetimepicker').datetimepicker();
        $('#endDatetimepicker').datetimepicker();
    });
}

function setTimeZoneInput() {
    document.getElementById("TimeZoneInput").value = Intl.DateTimeFormat().resolvedOptions().timeZone;
}

function setTimeZoneSelect() {
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    document.getElementById("TimeZone").value = timeZone;
    document.getElementById("TimeZoneInput").value = timeZone;
}

function setFileName() {
    $(document).ready(function () {
        $('.custom-file-input').on("change", function () {
            const fileName = $(this).val().split("\\").pop();
            $(this).next('.custom-file-label').html(fileName);
        })
    })
}

function GenerateCalendar(eventsToList) {
    $('#calendar').fullCalendar('destroy');
    $('#calendar').fullCalendar({
        contentHeight: 500,
        defaultDate: new Date(),
        timeFormat: 'h(:mm)a',
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,basicWeek,basicDay,agenda'
        },
        eventLimit: true,
        events: eventsToList,
        eventClick: function (calEvent, jsEvent, view) {
            window.location.href = "/meeting/edit/" + calEvent.id;
        }
    })
}

function getEventsAndGenerateCalendar() {
    $(document).ready(function () {
        const events = [];
        $.ajax({
            url: "/api/FullCalendar",
            contentType: "application/json",
            method: "GET",
            dataType: "json",
            success: function (data) {
                $.each(data,
                    function (i, v) {
                        events.push({
                            id: v.meetingId,
                            title: v.title,
                            start: moment(v.startDateTime),
                            end: moment(v.endDateTime)
                        });
                    })
                GenerateCalendar(events);
            },
            error: function (error) {
                alert('failed');
            }
        })
    })
}

function copyAddress() {
    $.ajax({
        url: "/api/HomeAddress",
        contentType: "application/json",
        method: "GET",
        dataType: "json",
        success: function (data) {
            $.each(data,
                function (index, element) {
                    switch (index) {
                        case "Street":
                            document.getElementById("Street").value = element;
                            break;
                        case "City":
                            document.getElementById("City").value = element;
                            break;
                        case "PostalCode":
                            document.getElementById("Code").value = element;
                            break;
                        case "Country":
                            document.getElementById("Country").value = element;
                            break;
                    }
                });
        }
    });
}

function renderMarkdownEditor() {
    const editor = new Editor({
        element: document.getElementById("meetingDescription")
    });

    editor.render();
}

function showMessage(id) {
    $.ajax({
        type: "POST",
        url: "/Message/ShowMessage",
        data: {id: id},
        success: function (response) {
            $("#messageModalBody").html(response);
            $("#messageModal").modal("show");
        }

    })
}

function editMessage() {
    $(document).ready(function () {
        const message = $("#messageForm").serialize();

        $.ajax({
                type: "POST",
                url: "/Message/Edit",
                data: message,
                success: function () {
                    $("#messageModal").modal("hide");
                    location.reload();
                }
            }
        )
    })
}