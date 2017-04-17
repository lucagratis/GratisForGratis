$inputAllegatoSegnalazione = $('#AllegatoSegnalazione');
$urlSegnalazione = '/Home/Segnalazione';
/* VARIABILI GLOBALI */
var CLICK_PULSANTE = 0;

$(document).ready(function () {
    initAutocomplete();
    initCercaAcquisto();

    //$("img.lazy").show().lazyload();
    $('#menuMobile .icona').click(function (event) {
        /*if ($('body').css('overflow') == 'hidden')
            $('body').css('overflow', '');
        else
            $('body').css('overflow', 'hidden');*/
        $('#menuMobileOverlay').slideToggle('slow');
        $('#menuMobile .menu').slideToggle('slow');
    });

    //$('form').submit(pauseSubmit(this));
    
    $('.btn:not(.dropdown-toggle)').click(function (event) {
        //attendiInvio(this);
        $('html').loader('show');
    });

    var tryNumber = 0;
    $('input[type=submit]').click(function (event) {
        var self = $(this);
        if (self.closest('form').valid()) {
            $('html').loader('show');
            if (tryNumber > 0) {
                tryNumber++;
                //alert('Your form has been already submited. wait please');
                return false;
            }
            else {
                tryNumber++;
            }
        };
    });
    /*
    $('a.link').click(function (event) {
        $('html').loader('show');
        if (tryNumber > 0) {
            tryNumber++;
            //alert('Your form has been already submited. wait please');
            return false;
        }
        else {
            tryNumber++;
        }
    });*/

    $("form").bind("invalid-form.validate", function () {
        $('html').loader('hide');
    });
    $(document).ajaxComplete(function () {
        $('html').loader('hide');
    });
    
    // SEGNALAZIONE
    $('#reporting').click(function (event) {
        $('#boxSegnalazione').dialog({
            title: 'Reporting',
            width: 550,
            modal: true,
            open: function (event, ui) {
                initUploadSegnalazione();
            },
            close: function (event, ui) {
                $('#boxSegnalazione select[name="Tipologia"]').val(0);
                $('#boxSegnalazione input[name="EmailRisposta"]').val('');
                $('#boxSegnalazione input[name="Oggetto"]').val('');
                $('#boxSegnalazione textarea[name="Testo"]').val('');
                $inputAllegatoSegnalazione.uploadifive('destroy');
            }
        });

        $('#boxSegnalazione .button').click(function (event) {
            var numeroUpload = $('.uploadifive-queue-item').length;
            //alert(numeroUpload);
            // verificare la presenza o meno dell'allegato per mandare la segnalazione
            if (numeroUpload <= 0) {
                $.ajax({
                    type: "POST",
                    url: $urlSegnalazione,
                    data: $('#boxSegnalazione').serialize(),
                    dataType: "html",
                    success: function (msg) {
                        segnalazioneInviata(msg);
                    },
                    error: function (error, status, msg) {
                        segnalazioneErrata(msg);
                    }
                });
            } else {
                $inputAllegatoSegnalazione.data('uploadifive').settings.formData = {
                    '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
                    'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
                    'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
                    'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
                    'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
                    'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
                    'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
                };
                $inputAllegatoSegnalazione.uploadifive('upload');
            }
        });
    });
    $('#reportingMobile').click(function (event) {
        $('#boxSegnalazione').dialog({
            title: 'Reporting',
            width: 550,
            modal: true,
            open: function (event, ui) {
                initUploadSegnalazione();
            },
            close: function (event, ui) {
                $('#boxSegnalazione select[name="Tipologia"]').val(0);
                $('#boxSegnalazione input[name="EmailRisposta"]').val('');
                $('#boxSegnalazione input[name="Oggetto"]').val('');
                $('#boxSegnalazione textarea[name="Testo"]').val('');
                $inputAllegatoSegnalazione.uploadifive('destroy');
            }
        });

        $('#boxSegnalazione .button').click(function (event) {
            var numeroUpload = $('.uploadifive-queue-item').length;
            //alert(numeroUpload);
            // verificare la presenza o meno dell'allegato per mandare la segnalazione
            if (numeroUpload <= 0) {
                $.ajax({
                    type: "POST",
                    url: $urlSegnalazione,
                    data: $('#boxSegnalazione').serialize(),
                    dataType: "html",
                    success: function (msg) {
                        segnalazioneInviata(msg);
                    },
                    error: function (error, status, msg) {
                        segnalazioneErrata(msg);
                    }
                });
            } else {
                $inputAllegatoSegnalazione.data('uploadifive').settings.formData = {
                    '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
                    'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
                    'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
                    'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
                    'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
                    'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
                    'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
                };
                $inputAllegatoSegnalazione.uploadifive('upload');
            }
        });
    });
    $('[data-toggle="tooltip"]').tooltip();
    $('#dropdownMenu1').click(function () {
        $('#menu').toggle();
    });

    // nuova funzione per il replace
    String.prototype.replaceAll = function (search, replacement) {
        var target = this;
        return target.replace(new RegExp(search, 'g'), replacement);
    };
});

function initUploadSegnalazione() {
    // crea oggetto uploadifive
    $inputAllegatoSegnalazione.uploadifive({
        'buttonText': 'UPLOAD FILE',
        'buttonClass': 'upload',
        'auto': false,
        'removeCompleted': true,
        'fileObjName': 'Allegato',
        'fileSizeLimit': 500,
        'multi': false,
        'queueSizeLimit': 1,
        'simUploadLimit': 1,
        'uploadLimit': 0,
        'formData': {
            '__RequestVerificationToken': $('#boxSegnalazione input[name="__RequestVerificationToken"]').val(),
            'Tipologia': $('#boxSegnalazione select[name="Tipologia"] option:selected').val(),
            'EmailRisposta': $('#boxSegnalazione input[name="EmailRisposta"]').val(),
            'Oggetto': $('#boxSegnalazione input[name="Oggetto"]').val(),
            'Testo': $('#boxSegnalazione textarea[name="Testo"]').val(),
            'Controller': $('#boxSegnalazione input[name="Controller"]').val(),
            'Vista': $('#boxSegnalazione input[name="Vista"]').val(),
        },
        'uploadScript': $urlSegnalazione,
        'onUploadComplete': function (file, data) {
            segnalazioneInviata(data);
        },
        'onError': function (errorType, file) {
            switch (errorType) {
                case 'QUEUE_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error number file');
                    break;
                case 'UPLOAD_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error number file');
                    break;
                case 'FILE_SIZE_LIMIT_EXCEEDED':
                    segnalazioneErrata('Error size file');
                    break;
                case 'FORBIDDEN_FILE_TYPE':
                    segnalazioneErrata('Error format file');
                    break;
                case '404_FILE_NOT_FOUND':
                    segnalazioneErrata('File not found');
                    break;
                default:
                    segnalazioneErrata(errorType);
                    break;
            }
        }
    });
}

function segnalazioneInviata(data) {
    alert(data);
    $('#boxSegnalazione').dialog('close');
    $('html').loader('hide');
}

function segnalazioneErrata(msg) {
    $inputAllegatoSegnalazione.uploadifive('destroy');
    initUploadSegnalazione();
    $('html').loader('hide');
    alert(msg);
}

// Inizializza i campi di ricerca rapida (autocomplete)
// tolto proprietà delay a 0 perchè rallentava troppo le chiamate remote
function initAutocomplete() {
    $('*[data-autocomplete-url]').each(function () {
        if ($(this).autocomplete("instance")!=undefined)
            $(this).autocomplete("destroy");
        var url = $(this).data("autocomplete-url");
        var filtro = $(this).data("autocomplete-filtro-extra");
        $(this).autocomplete({
            position: { my: "left top", at: "left bottom+15px", collision: "flip" },
            delay: 500,
            minLength: 2,
            //source: $(this).data("autocomplete-url") + ($(this).data("autocomplete-filtro-extra"))? "?filtroExtra=" + $($(this).data("autocomplete-filtro-extra")).val() : "",
            source: function (request, response) {
                $.ajax({
                    url: url,
                    dataType: "json",
                    data: {
                        term: request.term,
                        filtroExtra: $(filtro).val()
                    },
                    success: function (data) {
                        response(data);
                    }
                });
            },
            select: function (event, ui) {
                //$("#" + $(this).data("autocomplete-id")).val(ui.item.Value);
                return false;
            },
            focus: function (event, ui) {
                event.preventDefault();
                $(this).val(ui.item.Label);
            },
            change: function (event, ui) {
                if (ui.item != null) {
                    $("#" + $(this).data("autocomplete-id")).val(ui.item.Value);
                } else {
                    $("#" + $(this).data("autocomplete-id")).val('');
                }
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            return $("<li>").data("item.autocomplete", item).append("<a id='cat" + item.Value + "'>" + item.Label + "</a>").appendTo(ul);
        };
    });
}

/**
** slider = selettore del div da trasformare in slider
** element = selettore dell'input dove mostrare il valore
**/
function setSlider(slider, label, min, max, unitaMisura, inputMin, inputMax) {
    min = (min) ? min : 0;
    max = (max) ? max : 100000;
    unitaMisura = (unitaMisura) ? unitaMisura : "";

    //alert("Slider: " + slider + " => Min: " + min + " Max: " + max);

    $(slider).slider({
        range: true,
        min: min,
        max: max,
        values: [$(inputMin).val(), $(inputMax).val()],
        create: function (event, ui) {
            //$(label).text(unitaMisura + " " + $(inputMin).val() + " - " + unitaMisura + " " + $(inputMax).val());
            $(label).text(unitaMisura);
        },
        slide: function (event, ui) {
            //$(label).text(unitaMisura + " " + ui.values[0] + " - " + unitaMisura + " " + ui.values[1]);
            $(inputMin).val(ui.values[0]);
            $(inputMax).val(ui.values[1]);
        }
    });
}

/*
    Inizializza toolbar ricerca minima
*/
function initCercaAcquisto() {
    /*$('#menu').menu({
        classes: {
            "ui-menu": "highlight"
        },
        items: ".custom-item"
    });*/
    inizializzaMenu();
    impostaCategoria('#menuCategoriaRicerca', '#menu');
}

function inizializzaMenu() {
    $(".dropdown-menu > li .trigger").hover(function (e) {
        var current = $(this).next();
        var grandparent = $(this).parent().parent();
        grandparent.find(".sub-menu:visible").not(current).hide();
        current.show();
        e.stopPropagation();
    });
    $(".dropdown-menu > li a:not(.trigger)").hover(function () {
        var root = $(this).closest('.dropdown');
        root.find('.left-caret').toggleClass('right-caret left-caret');
        root.find('.sub-menu:visible').hide();
    });
    /*$(".dropdown-menu > li div:not(.trigger)").hover(function () {
        var root = $(this).closest('.dropdown');
        root.find('.left-caret').toggleClass('right-caret left-caret');
        root.find('.sub-menu:visible').hide();
    });*/
}

// usata in : site.js, pubblica.js
// serve a settare i valori id e nome della categoria scelta
function impostaCategoria(idBoxCategoria, menu, callBack) {
    $(idBoxCategoria + ' li a').on('click', function (event) {
        $($(this).data('id')).val($(this).data('value'));
        $($(this).data('name')).val($(this).text());
        $(this).parents('.menuCategoria').find('.categoriaSelezionata').text($(this).text());
        $(menu).toggle();
        // chiude tutti i menu quando li nasconde
        //$(this).parents('.dropdown-menu').toggle();
        if (callBack != undefined)
            window[callBack].apply();
    });
}

// inizializza controllo per le checkbox multiple
function setAllCheckbox(checkboxAll, subCheckbox, force) {
    if (force) {
        if ($(checkboxAll).is(':checked')) {
            $(subCheckbox + ':not(:checked)').prop('checked', true);
        }
    }

    // sul click al checkbox tutti
    $(checkboxAll).change(function () {
        if ($(this).is(':checked')) {
            $(subCheckbox + ':not(:checked)').prop('checked', true);
        } else {
            $(subCheckbox + ':checked').prop('checked', false);
        }
    });

    // sul click degli altri checkbox
    $(subCheckbox).change(function () {
        if (!$(this).is(':checked')) {
            $(checkboxAll).prop('checked', false);
        } else if ($(subCheckbox + ':not(:checked)').length <= 0) {
            $(checkboxAll).prop('checked', true);
        }
    });
}

function pauseSubmit(form) {
    var button = $(form).find('input[type="submit"]');
    setTimeout(function () {
        button.removeAttr('disabled');
    }, 1);
}

// blocca il pulsante fino alla fine dell'esecuzione lato server
function attendiInvio(pulsante) {
    var self = $(pulsante);

    //if (self.closest('form').valid()) {
    if (CLICK_PULSANTE > 0) {
        CLICK_PULSANTE++;
        //alert('Your form has been already submited. wait please');
        return false;
    }
    else {
        CLICK_PULSANTE++;
        $.loader({
            className: "blue-with-image-2",
            content: ''
        });
    }
    //};
}

function reinvioEmailRegistrazione()
{
    $.ajax({
        type: "GET",
        url: "/Utente/ReinvioEmailRegistrazione",
        dataType: "json",
        success: function (data) {
            alert("Invio effettuato. Controlla anche nella tua casella di spam.");
        }
    });
}