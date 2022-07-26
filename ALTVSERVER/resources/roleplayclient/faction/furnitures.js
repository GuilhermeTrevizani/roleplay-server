$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function showHTML(propertyId, html) {
    $('#h3-title').html(`Comprar Mobílias para Propriedade Nº ${propertyId}`);
    $('#div-body').html(html);

    $("#pesquisa").on('input', function () {
        filtrarComandos();
    });

    $("#sel-category").on('change', function () {
        filtrarComandos();
    });
}

function filtrarComandos() {
    var pesquisa = removerAcentos($("#pesquisa").val());
    $.each($(".pesquisaitem"), function (index, element) {
        $(element).show();

        if ($('#sel-category').val() != 'Todas' && $(element).data('category') != $('#sel-category').val()) {
            $(element).hide();
        } else if (pesquisa != "") {
            if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                $(element).hide();
        }
    });
}

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

function closeView() {
    alt.emit('closeView');
}

function buy(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('buy', id);        
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
}