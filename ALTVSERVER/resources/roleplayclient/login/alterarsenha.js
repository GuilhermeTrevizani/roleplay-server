function alterar() {
    if (!$('#senhaAntiga').val() || !$('#novaSenha').val() || !$('#novaSenha2').val()) {
        mostrarErro("Verifique se todos os campos foram preenchidos corretamente.");
        return;
    }

    $('#btn-alterar').LoadingOverlay('show');
    alt.emit("alterar", $('#senhaAntiga').val(), $('#novaSenha').val(), $('#novaSenha2').val());
}

function voltar() {
    alt.emit("voltar");
}

function mostrarErro(erro) {
    $('#sucesso').css('display', 'none');
    $('#btn-alterar').LoadingOverlay('hide');
    $('#senhaAntiga').val('');
    $('#novaSenha').val('');
    $('#novaSenha2').val('');
    $.alert(erro);
}

function mostrarSucesso(sucesso) {
    $('#erro').css('display', 'none');
    $('#btn-alterar').LoadingOverlay('hide');
    $('#senhaAntiga').val('');
    $('#novaSenha').val('');
    $('#novaSenha2').val('');
    $.alert(sucesso);
}

if('alt' in window) {
    alt.on('mostrarErro', mostrarErro);
    alt.on('mostrarSucesso', mostrarSucesso);
}