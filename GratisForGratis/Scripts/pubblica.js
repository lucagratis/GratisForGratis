$(document).ready(function () {
    //attiva menu
    $('menu ul .publication').addClass('active');

    impostaCategoria('#menuCategoriaPubblicazione', '#categoriePubblicazione');
    $('#dropdownMenuPubblicazione').click(function () {
        $('#categoriePubblicazione').toggle();
    });
    // attiva upload custom delle foto
    $('#file').uploadifive({
        'uploadScript': '/Pubblica/UploadFotoOggetto',
        // Put your options here
        'fileObjName': 'file',
        'auto': true,
        'multi': true,
        //'uploadLimit': 4,
        'fileSizeLimit': '20MB',
        'queueID': 'listaFileAggiunti',
        'queueSizeLimit': 4,
        'buttonText': 'Carica foto',
        'buttonClass': 'btnUpload',
        'fileTypeExts': '*.gif; *.jpg; *.png; *.jpeg; *.tiff; *.bmp;',
        'itemTemplate': '<div class="uploadifive-queue-item"><span class="filename"></span> | <span class="fileinfo" style></span><span class="chiudi" style="display:none;"></span></div>',
        'formData': {
            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        'onProgress'   : function(file, e) {
            if (e.lengthComputable) {
                var percent = Math.round((e.loaded / e.total) * 100);
            }
            file.queueItem.find('.fileinfo').html(' - ' + percent + '%');
            file.queueItem.find('.progress-bar').css('width', percent + '%');
        },
        'onAddQueueItem': function (file) {
            $boxUploadFatti = $('#listaFileAggiunti');
            if ($boxUploadFatti.find('.title').length <= 0)
                $boxUploadFatti.prepend('<h4 class="title">Upload</h4>');
        },
        'onUploadComplete': function (file, data) {
            $fileCreato = JSON.parse(data);
            var idCodificato = encodeURI($fileCreato.responseText.replace('.jpg',''));
            $foto = $('<input id="foto' + idCodificato + '" name="Foto" type="hidden" value="' + file.name + '" />')
            //$('#Foto').val($('#Foto').val() + file.name + ';');
            // setto il click per l'annullo sugli upload completati
            $('#listaFileAggiunti').append($foto);
            file.queueItem.find('.fileinfo').html(' - Completato');
            file.queueItem.find('.chiudi').css('display','inline');
            file.queueItem.find('.chiudi').click(function (event) {
                event.preventDefault();
                //alert($fileCreato.responseText);
                annullaUploadFoto(idCodificato, this);
            });
            // tolgo il titolo della lista
            if ($('#listaFileAggiunti .uploadifive-queue-item').length <= 0)
                $('#listaFileAggiunti').html('');
        },
        'onFallback'   : function() {
            alert('Oops!  You have to use the non-HTML5 file uploader.');
        },
        'onError': function (errorType) {
            alert('Errore: ' + errorType);
        }
    });

    // inizializza scelta colore oggetto
    $('#Colore').ColorPicker();
    
    setAllCheckbox('input[name="Tutti"]', '#pubblicazione .day', true);
});

function annullaUploadFoto(nome, linkAnnullo) {
    //$file = $(linkAnnullo).parents('.uploadifive-queue-item');
    //if ($file.length > 0) {
        //var id = $file.attr('id');
        //var nome = $(linkAnnullo).prev('.filename').text();
        //var indice = id.replace('uploadifive-file-file-', '');
        //alert(indice);
        $.ajax({
            type: 'POST',
            url: '/Pubblica/AnnullaUploadFoto',
            dataType: "json",
            data: {
                nome: decodeURI(nome)
            },
            success: function (data) {
                $(linkAnnullo).parents('.uploadifive-queue-item').remove();
                //alert($('#foto' + nome).length);
                $('#foto' + nome).remove();
                if ($('#listaFileAggiunti').find('.uploadifive-queue-item').length <= 0)
                    $('#listaFileAggiunti').html('');
            }
        });
    //}
}