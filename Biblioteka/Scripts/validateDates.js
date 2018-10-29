$(document).ready(function () {
    if ($('#current').is(':checked')){
        var table = $("#rentalsTable").find('tbody').find('tr');
        for (var i = 1; i < table.length; i++) {
            var row = $(table[i]);
            var cell = $(table[i]).find('td:eq(4)');
            var cellValue = $(table[i]).find('td:eq(4)').text();
            var dateTo = cellValue.split(".");
            var date = new Date(dateTo[2], dateTo[1] - 1, dateTo[0], 0, 0, 0, 0);
            var today = new Date();
            today.setHours(0, 0, 0, 0);


            if (date < today) {
                console.log('eeeee');
                row.css("background-color", "#E3817F");
                row.css("font-weight", "bold");
            }

            else {
                console.log(date);

                date.setDate(date.getDate() - 3);
                console.log(date);

                if (date < today) {
                    row.css("background-color", "#f7ff7f");
                }
            }

        }
    }

});
