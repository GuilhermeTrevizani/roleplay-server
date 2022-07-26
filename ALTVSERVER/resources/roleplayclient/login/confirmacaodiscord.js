function showHTML(usuario, token) {
    $('#usuario').html(usuario);
    $('#token').html(token);
}

$(document).ready(() => {
    $('#btn-verificarconfirmacao').click(() => {
        $('#btn-verificarconfirmacao').LoadingOverlay('show');
        alt.emit('verificarConfirmacao', $('#token').val());
    });
});

function mostrarErro(erro) {
    $('#btn-verificarconfirmacao').LoadingOverlay('hide');
    $.alert(erro);
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarErro', mostrarErro);
}