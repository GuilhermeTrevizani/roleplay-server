$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function showHTML(html) {
    $('#basehtml').html(html);

    $("#pesquisa").on('input', function () {
        var pesquisa = removerAcentos($("#pesquisa").val());
        $.each($(".pesquisaitem"), function (index, element) {
            $(element).show();

            if (pesquisa != "") {
                if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                    $(element).hide();
            }
        });
    });
}

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window)
    alt.on('showHTML', showHTML);