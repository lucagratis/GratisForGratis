$(function () {
    function suggerimentoAttivazioneAnnuncio(id) {
        $.ajax({
            type: 'POST',
            url: '/Home/SuggerimentoAttivazioneAnnuncio',
            dataType: "json",
            data: {
                id: decodeURI(id)
            },
            success: function (data) {
                alert(data);
            }
        });
    }
});