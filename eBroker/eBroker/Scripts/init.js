$(document).ready(function () {

    // Setup JQuery UI date picker format to dd/mm/yy
    $.datepicker.regional[""].dateFormat = 'dd/mm/yy';
    $.datepicker.setDefaults($.datepicker.regional['']);

    $('#myDataTable').dataTable({
        "bServerSide": true,
        "sAjaxSource": "/ExpiryPolice/DataProviderAction"
    }).columnFilter({
        "aoColumns": [
                       { "type": "select" },
                       { "type": "select" },
                       { "type": "select" },
                       { "type": "date-range" },
                       { "type": "date-range" },
                       { "type": "number-range" }
        ]
    });
});