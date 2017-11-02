$(document).ready(function () {

    // top 10 vendor List

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/topVendorList",
        data: "{}",
        dataType: "json",
        success: function (Result) {
            console.log(Result)
            var vendordata = [];
            for (var i in Result) {
                var serie = new Array(Result[i].name, Result[i].value, );
                vendordata.push(serie);
            }
            console.log(vendordata);
            DreawPieChart(vendordata);
        },
        error: function (err) {
            console.log("topVendorList");
            console.log(err);
        }
    });

    function DreawPieChart(series) {

        var chart = new Highcharts.Chart({
            chart: {
                type: 'line',
                renderTo: 'topvendorchart'
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Top 10 Vendors'
            },
            subtitle: {
                text: 'Frequent transaction accourding to last 3 months data.'
            },
            xAxis: {
                type: 'category'
                
            },
            yAxis: {
                title: {
                    text: 'Total No of transaction'
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




    // Top 10 medicine column chart

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Home/topSellingMedicine",
        data: "{}",
        dataType: "json",
        success: function (Result) {            
            var data = [];
            for (var i in Result) {
                var serie = new Array(Result[i].name, Result[i].value);
                data.push(serie);
            }            
            DreawChart(data);
        },
        error: function (err) {
            console.log("topSellingMedicine");
            console.log(err);
        }
    });


    function DreawChart(series) {

        var chart = new Highcharts.Chart({
            chart: {
                type: 'column',
                renderTo: 'topmedicinechart'
            },
            credits: {
                enabled: false
            },
            title: {
                text: 'Top 10 Selling Medicine'
            },
            subtitle: {
                text: 'Chart display accourding to last 3 months records.'
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