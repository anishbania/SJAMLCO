$(document).ready(function () {
    loadPieChart1();
});

function loadPieChart1() {
    $.ajax({
        url: "User/PieChartSamiti",
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadPieChartFunc("dataPieChart", data);
        }
    });
}

function getChartColorsArray(id) {
    if (document.getElementById(id) !== null) {
        var colors = document.getElementById(id).getAttribute("data-colors");
        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;
                }
            });
        }

    }
}

function loadPieChartFunc(id, data) {
    var chartColours = getChartColorsArray(id);
    options = {
        series: data.series,
        labels: data.labels,
        colors: chartColours,
        chart: {
            type: 'pie',
            width: 380
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: "#fff",
                useSeriesColors: true
            },
        },
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);


    chart.render();
}