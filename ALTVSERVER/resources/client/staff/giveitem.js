$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$('#btn-confirm').click(() => {
    const targetId = $('#input-targetId').val();
    if (!targetId) {
        $.alert('ID do Jogador é obrigatório.');
        return;
    }

    $(`#btn-confirm`).LoadingOverlay('show');
    alt.emit('giveItem', 
        parseInt($('#sel-category').val()),
        $('#input-type').val(),
        $('#input-extra').val(),
        parseInt($('#input-quantity').val()),
        parseInt(targetId)
    );
});

function loaded(categories) {
    categories.forEach((x) => {
        $('#sel-category').append(`<option value='${x.ID}'>${x.Name}</option>`);
    });

    $("#sel-category").on('change', () => {
        const category = categories.find(x => x.ID == parseInt($('#sel-category').val()));

        $('#input-quantity').val(1);
        if(category.IsStack)
            $('#col-quantity').show();
        else
            $('#col-quantity').hide();

        if (category.HasType)
            $('#col-type').show();
        else
            $('#col-type').hide();

        if (category.Extra) {
            $('#input-extra').val(category.Extra);
            $('#col-extra').show();
        } else {
            $('#input-extra').val('');
            $('#col-extra').hide();
        }
    });

    $('#sel-category').val(1).change();
}

function closeView() {
    alt.emit('closeView');
}

function mostrarMensagem(message) {
    $(`#btn-confirm`).LoadingOverlay('hide');
    $.alert(message);
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarMensagem', mostrarMensagem);
}