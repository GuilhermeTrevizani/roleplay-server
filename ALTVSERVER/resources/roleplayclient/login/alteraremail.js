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
    if (erro != "")  {
        $('#erro').html(erro);
        $('#erro').css('display', 'block');
    }
}

if('alt' in window)
    alt.on('mostrarErro', mostrarErro);