$(document).ready(function () {
    loadBarChart();
});

function loadBarChart() {
    $.ajax({
        url: "Home/BarChart",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadBarChartFunc("dataBarChart", data);
        }
    });
}

function loadBarChartFunc(id, data) {
    var options = {
        series: [{
            data: data.series,
        }],
        
        chart: {
            type: 'bar',
            height: 350
        },
        plotOptions: {
            bar: {
                borderRadius: 4,
                borderRadiusApplication: 'end',
                horizontal: true,
            }
        },
        dataLabels: {
            enabled: false
        },
        xaxis: {
            categories: data.labels,
        }

        
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}