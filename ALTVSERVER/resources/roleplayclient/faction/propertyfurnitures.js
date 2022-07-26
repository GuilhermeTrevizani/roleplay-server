$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(() => {
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
});

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

$('#btn-buy').click(() => {
    alt.emit('buy');
});

function showHTML(propertyId, furnitures) {
    $('#h3-title').html(`Propriedade Nº ${propertyId}`);
    $('#tbody-furnitures').html(furnitures);
}

function edit(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('edit', id);        
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function closeView() {
    alt.emit('closeView');
}

function mostrarMensagem(mensagem, fechar) {
    $.alert(mensagem);

    if (fechar && modal) {
        modal.close();
        modal = null;
    }
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarMensagem', mostrarMensagem);
}