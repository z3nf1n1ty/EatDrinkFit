// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


    //window.onload = function () {
    //    document.getElementById("<%=HiddenField1.ClientID %>").value = Intl.DateTimeFormat().resolvedOptions().timeZone;
    //    alert(document.getElementById("<%=HiddenField1.ClientID %>").value);
    //};

function setValue(id, newvalue) {
    var s = document.getElementById(id);
    s.value = newvalue;
}

window.onload = function () {
    //setValue("ManualTimeZone", Intl.DateTimeFormat().resolvedOptions().timeZone);
    $(".hiddentimezone").val(Intl.DateTimeFormat().resolvedOptions().timeZone);
}