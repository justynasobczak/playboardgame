function generateTooltip() {
    $(document).ready(function () {
        $('[data-toggle="tooltip"]').tooltip();
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