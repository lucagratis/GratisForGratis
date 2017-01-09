$(document).ready(function () {

    // inizializzo pulsanti di accettazione dell' offerta
    $('#grid .ok').one('click', function (event) {
        accettaOfferta(this, $(this).parents('.purchase').attr('id'));
    });
    $('#grid .ko').one('click', function (event) {
        rifiutaOfferta(this, $(this).parents('.purchase').attr('id'));
    });
});

function accettaOfferta(link, token) {
    $.ajax({
        type: "POST",
        url: '/Offerte/AccettaOfferta',
        data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (msg) {
            if (msg) {
                //alert(msg);
                $('.ok').remove();
                $('.ko').remove();
                $('.stateText').html(msg);
            }
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                accettaOfferta(link, token);
            });
            alert("Errore accettazione offerta: " + errore.responseText);
        }
    });
}

function rifiutaOfferta(link, token) {
    $.ajax({
        type: "POST",
        url: '/Offerte/RifiutaOfferta',
        data: "token=" + token,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (msg) {
            if (msg) {
                //alert(msg);
                $('.ok').remove();
                $('.ko').remove();
                $('.stateText').html(msg);
            }
        },
        error: function (errore, stato, messaggio) {
            $(link).one('click', function (event) {
                rifiutaOfferta(link, token);
            });
            alert("Errore rifiuto offerta: " + errore.responseText);
        }
    });
}