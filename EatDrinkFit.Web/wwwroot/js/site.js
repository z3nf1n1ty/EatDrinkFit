// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Get AINI timezone for the current user
    const timeZoneValue = Intl.DateTimeFormat().resolvedOptions().timeZone;

    // Set timezone for form post when marked with hiddentimezone class
    $(".hiddentimezone").val(timeZoneValue);

    // Set Timezone Cookie
    document.cookie = "userTimezone=" + timeZoneValue;
});

function setValue(id, newvalue) {
    var s = document.getElementById(id);
    s.value = newvalue;
}

//old moved to view
function dashboardCaloriesChartSiteJS() {
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
                'rgba(40, 167, 69, 0.6)',
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
                'rgba(40, 167, 69, 1)',
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

function dashboardCaloriesChart(inputLables, inputData) {
    const data = {
        labels: inputLables,
        datasets: [{
            label: '14 Day - Calories',
            data: inputData,
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
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(255, 193, 7, 0.6)',
                'rgba(40, 167, 69, 0.6)',
            ],
            //borderColor: [
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(255, 206, 86, 1)',
            //    'rgba(40, 167, 69, 1)',
            //],
            borderWidth: 0
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
            },
            plugins: {
                legend: {
                    display: false
                },
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