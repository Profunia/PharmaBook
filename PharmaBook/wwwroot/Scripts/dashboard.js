$(document).ready(function () {

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/topSellingMedicone",
        data: "{}",
        dataType: "json",
        success: function (Result) {
            console.log(Result)
            var data = [];
            for (var i in Result) {
                var serie = new Array(Result[i].name, Result[i].value);
                data.push(serie);
            }
            console.log(data);
            DreawChart(data);
        },
        error: function (Result) {
            alert("Error");
        }
    });


    function DreawChart(series) {

        var chart = new Highcharts.Chart({
            chart: {
                type: 'column',
                renderTo: 'container'
            },
            title: {
                text: 'Top 10 Selling Medicine'
            },
            subtitle: {
                text: 'Chart display based on last 3 months records.'
            },
            xAxis: {
                type: 'category'
            },
            yAxis: {
                title: {
                    text: 'Total Selling Medicine'
                }

            },

            legend: {
                enabled: false
            },
            plotOptions: {
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y}'
                    }
                }
            },
            tooltip: {
                headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> of total<br />'
            },
            series: [{
                data: series
            }]
        });
    }
});