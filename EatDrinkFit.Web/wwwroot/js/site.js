﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
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

function dashboardCaloriesChartOld() {
    var areaChartData = {
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'May', 'June', 'July'],
        datasets: [
            {
                label: '',
                backgroundColor: 'rgba(255,193,7,0.6)',
                borderColor: 'rgba(255,193,7,0.7)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(255,193,7,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(255,193,7,1)',
                data: [1718, 1621, 2235, 1864, 1892, 2137, 1789, 1934, 2013, 1876]
            },

        ]
    }


    var barChartCanvas = $('#CaloriesBarChart').get(0).getContext('2d')
    var barChartData = $.extend(true, {}, areaChartData)
    var temp0 = areaChartData.datasets[0]
    var temp1 = areaChartData.datasets[1]
    barChartData.datasets[0] = temp0
    //barChartData.datasets[1] = temp1

    var barChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        datasetFill: false,
        legend: {
            display: false
        },
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                }
            }]
        },
    }

    new Chart(barChartCanvas, {
        type: 'bar',
        data: barChartData,
        options: barChartOptions,

    })
}

function dashboardCaloriesChart() {
    const data = {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [{
            label: 'Weekly Sales',
            data: [18, 12, 6, 9, 12, 3, 9],
            backgroundColor: [
                'rgba(255, 26, 104, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)',
                'rgba(0, 0, 0, 0.2)'
            ],
            borderColor: [
                'rgba(255, 26, 104, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)',
                'rgba(0, 0, 0, 1)'
            ],
            borderWidth: 1
        }]
    };


    // config 
    const config = {
        type: 'bar',
        data,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    };

    // render init block
    const myChart = new Chart(
        document.getElementById('CaloriesBarChart'),
        config
    );

    // Instantly assign Chart.js version
    //const chartVersion = document.getElementById('chartVersion');
    //chartVersion.innerText = Chart.version;
}