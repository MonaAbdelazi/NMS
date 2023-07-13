// Morris.js Charts sample data for SB Admin template

$(function() {
     AjaxCall({}, "/Home/GetChartData/", "loadChart", "");
     AjaxCall({}, "/Home/GetDonutChartData/", "loadDonutChart", "");
     AjaxCall({}, "/Home/GetDriverDonutChartData/", "loadDonutChart2", "");
     AjaxCall({}, "/Home/GetSpareDonutChartData/", "loadDonutChart3", "");

     
});


function loadChart(data, extraPrem) {
     // Area Chart
     Morris.Area({
          element: 'morris-area-chart',
          data: data,
          xkey: 'Period',
          ykeys: ['Active', 'Closed'],
          labels: ['Active', 'Closed'],
          pointSize: 2,
          hideHover: 'auto',
          resize: true
     });
}


function loadDonutChart(data, extraPrem) {
     Morris.Donut({
          element: 'morris-donut-chart',
          data: data,
          resize: true
     });
}

function loadDonutChart2(data, extraPrem) {
    Morris.Donut({
        element: 'morris-donut-chart2',
        data: data,
        resize: true
    });
}

function loadDonutChart3(data, extraPrem) {
    Morris.Donut({
        element: 'morris-donut-chart3',
        data: data,
        resize: true
    });
}