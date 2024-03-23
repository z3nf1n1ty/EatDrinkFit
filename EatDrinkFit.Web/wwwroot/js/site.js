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
        labels: ['Fri', 'Sat', 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [{
            label: 'Weekly Sales',
            data: [1832, 1587, 2136, 2079, 1981, 1863, 1875, 1634, 1958, 1867],
            backgroundColor: [
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
            ],
            borderColor: [
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(255, 206, 86, 1)',
            ],
            borderWidth: 1
        }]
    };

    // custom topline plugin
    const topLine = {
        id: 'topLine',
        afterDatasetsDraw(chart, args, plugins) {
            const { ctx, data } = chart;

            ctx.save();
            chart.getDatasetMeta(0).data.forEach((datapoint, index) => {

                // topline content removed

                // text
                ctx.font = 'bold 12px sans-serif';
                //ctx.fillStyle = 'black';
                ctx.fillStyle = 'rgba(31, 45, 61, 0.8)';
                ctx.textAlign = 'center';
                const txtValue = data.datasets[0].data[index];
                ctx.fillText(txtValue, datapoint.x, datapoint.y - 10);
            });            
        }
    }

    // config 
    const config = {
        type: 'bar',
        data,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    grace: '5%'
                }
            }
        },
        plugins: [topLine]
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