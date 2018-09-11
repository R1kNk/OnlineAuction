$(document).ready(function () {
    console.log($('#balance').text());
});

$(function () {
   
   
});

   
function updateBalance() {
    $.ajax({
        type: 'GET',
        url: '/Account/UpdateBalance',
        success: function (response) {
            if (response !== "no") {
                var txt = response;
                $('#balance').text(txt);
                console.log(response);
            }
        }
    });
}
