function alterar() {
    if (!$('#email').val()) {
        mostrarErro("Verifique se todos os campos foram preenchidos corretamente.");
        return;
    }

    $('#btn-alterar').LoadingOverlay('show');
    alt.emit("alterar", $('#email').val());
}

function voltar() {
    alt.emit("voltar");
}

function mostrarErro(erro) {
    $('#btn-alterar').LoadingOverlay('hide');
    $.alert(erro);
}

if('alt' in window)
    alt.on('mostrarErro', mostrarErro);