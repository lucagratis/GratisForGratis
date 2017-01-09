$(document).ready(function () {
    $tipoOfferta = $('#Offerta_TipoOfferta');
    sceltaOfferta($tipoOfferta);
    $tipoOfferta.change(function () {
        sceltaOfferta(this);
    });

    $('#CercaOggetto').autocomplete({
        position: { my: "left top", at: "left bottom+15px", collision: "flip" },
        delay: 0,
        minLength: 0,
        //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
        source: function (request, response) {
            $.ajax({
                url: "/Cerca/OggettiBarattabili",
                dataType: "json",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $baratto = $('<input class="' + ui.item.Value + '" type="hidden" name="Offerta.OggettiBarattati" value="' + ui.item.Value + '" />');
            $oggetto = $('<p class="barter ' + ui.item.Value + '">' + ui.item.Label + '<a class="remove" href="javascript:void(0);" onclick="rimuoviBaratto(\'' + ui.item.Value + '\',\'#' + $(this).attr('id') + '\');"> X</a></p>')
            $('#previewBarter').append($baratto);
            $('#previewBarter').append($oggetto);
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.Label);
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        if ($('#previewBarter .' + item.Value).length <= 0)
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        else
            return ul;
    };

    $('#CercaServizio').autocomplete({
        position: { my: "left top", at: "left bottom+15px", collision: "flip" },
        delay: 0,
        minLength: 0,
        //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
        source: function (request, response) {
            $.ajax({
                url: "/Cerca/ServiziBarattabili",
                dataType: "json",
                data: {
                    term: request.term
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        select: function (event, ui) {
            $baratto = $('<input class="' + ui.item.Value + '" type="hidden" name="Offerta.ServiziBarattati" value="' + ui.item.Value + '" />');
            $servizio = $('<span class="barter ' + ui.item.Value + '">' + ui.item.Label + '<a class="remove" href="javascript:void(0);" onclick="rimuoviBaratto(\'' + ui.item.Value + '\',\'#' + $(this).attr('id') + '\');"> X</a></span>')
            $('#previewBarter').append($baratto);
            $('#previewBarter').append($servizio);
            return false;
        },
        focus: function (event, ui) {
            event.preventDefault();
            $(this).val(ui.item.Label);
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        if ($('#previewBarter .' + item.Value).length <= 0)
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        else
            return ul;
    };

    $('.section-3 .photo img').each(function (key, value) {
        if ($(this).data('src') != undefined) {
            $(this).attr('src', $(this).data('src'));
        }
    });
    //$("img").lazyload();
    // override Highslide settings here
    // instead of editing the highslide.js file
    //hs.graphicsDir = '/highslide/graphics/';
});

function sceltaOfferta(elemento)
{
    if ($(elemento).val() == 1) {
        $('#boxBarter').show();
        $("#CercaOggetto").autocomplete("search", "");
    } else {
        $('#boxBarter').hide();
    }
}

function rimuoviBaratto(link,cerca) {
    $('.' + link).remove();
    $(cerca).val('');
}