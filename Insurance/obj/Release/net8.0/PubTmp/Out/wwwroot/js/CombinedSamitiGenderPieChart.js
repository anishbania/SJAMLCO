$(document).ready(function () {
    loadPieChart("/User/PieChartBewasthapan", "bewasthapanPieChart");

    $("#bewasthapan-tab").on("click", function () {
        loadPieChart("/User/PieChartBewasthapan", "bewasthapanPieChart");
    });

    $("#sanchalak-tab").on("click", function () {
        loadPieChart("/User/PieChartSanchalak", "sanchalakPieChart");
    });

    $("#rinupa-tab").on("click", function () {
        loadPieChart("/User/PieChartRinUpa", "rinupaPieChart");
    });
});

function loadPieChart(url, chartId) {
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        success: function (data) {
            loadPieChartFunc(chartId, data);
        }
    });
}

function loadPieChartFunc(id, data) {
    var chartColours = getChartColorsArray(id);
    var options = {
        series: data.series,
        labels: data.labels,
        colors: chartColours,
        chart: {
            type: 'pie',
            height: 350
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: "#000",
            },
        },
        tooltip: {
            theme: "dark",
            y: {
                formatter: function (value) {
                    return value + " सदस्यहरू";
                },
            },
        },
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}
