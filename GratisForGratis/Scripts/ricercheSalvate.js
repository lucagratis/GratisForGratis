function deleteRicerca(link) {
    $ricerca = $(link).parents('.ricerca');
    $.ajax({
        type: 'DELETE',
        url: '/Utente/RicercheSalvate',
        dataType: "json",
        data: {
            ricerca: $ricerca.attr('id')
        },
        success: function (data) {
            $ricerca.remove();
        }
    });
}

$(function () {
    
});