
$(document).ready(function() {
    loadLoaneeBarChart();
});

//function loadLoaneeBarChart() {
//    debugger;
//    $.ajax({
//        url: "User/GetLoaneeData",
//        type: "GET",
//        dataType: "json",
//        success: function (data) {
//            debugger;
//            console.log(data);
//            renderBarChart(data);
//        },
//        error: function (err) {
//            console.error("AJAX Error", err);
//        }
//    });
//}

function loadLoaneeBarChart() {
    console.log("Starting AJAX Request");
      debugger;
    $.ajax({
        url: "User/GetLoaneeData",
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log("AJAX Response:", data); 
            if (data && data.length > 0) {
                renderBarChart(data);
            } else {
                console.error("No data returned from the server");
            }
        },
        error: function (err) {
            console.error("AJAX Error:", err); 
        }
    });
}
function renderBarChart(data) {
    try {
        var chartColors = getChartColorsArray("loaneeBarChart");

        var categories = data.map(d => d.Sanstha);
        var seriesData = data.map(d => d.LoaneeCount);

        if (categories.length === 0 || seriesData.length === 0) {
            throw new Error("No data available to render the chart");
        }

        var options = {
            series: [{
                name: 'ऋण लिनेहरूको संख्या',
                data: seriesData
            }],
            chart: {
                type: 'bar',
                height: 350,
                background: 'transparent'
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '50%',
                    endingShape: 'rounded'
                }
            },
            dataLabels: {
                enabled: true
            },
            xaxis: {
                categories: categories,
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            colors: chartColors.length ? chartColors : ['#008FFB'],
            legend: {
                position: 'top',
                horizontalAlign: 'center',
            },
            fill: {
                opacity: 1
            }
        };

        var chart = new ApexCharts(document.querySelector("#loaneeBarChart"), options);
        chart.render();
    } catch (error) {
        console.error("Error rendering chart:", error.message);
    }
}

//function renderBarChart(data) {
//    debugger;
//    var chartColors = getChartColorsArray("loaneeBarChart");
//    var options = {
//        series: [{
//            name: 'ऋण लिनेहरूको संख्या',
//            data: data.map(d => d.LoaneeCount)
//        }],
//        chart: {
//            type: 'bar',
//            height: 350,
//            background: 'transparent'
//        },
//        plotOptions: {
//            bar: {
//                horizontal: false,
//                columnWidth: '50%',
//                endingShape: 'rounded'
//            }
//        },
//        dataLabels: {
//            enabled: true
//        },
//        xaxis: {
//            categories: data.map(d => d.Sanstha),
//            labels: {
//                style: {
//                    fontSize: '12px'
//                }
//            }
//        },
//        colors: chartColors.length ? chartColors : ['#008FFB'],
//        legend: {
//            position: 'top',
//            horizontalAlign: 'center',
//        },
//        fill: {
//            opacity: 1
//        }
//    };

//    var chart = new ApexCharts(document.querySelector("#loaneeBarChart"), options);
//    chart.render();
//}
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


