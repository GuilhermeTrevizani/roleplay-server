
function showHTML(usuario, email) {
    $('#titulo').html(`Confirmação do Registro de ${usuario}`);
    $('#email').val(email);
}

function reenviarEmail() {
    if (!$('#email').val()) {
        mostrarErro("Verifique se o e-mail foi preenchido corretamente.");
        return;
    }

    $('#btn-reenviaremail').LoadingOverlay('show');
    alt.emit('enviarEmail', $('#email').val());
}

function validarToken() {
    if (!$('#token').val()) {
        mostrarErro("Verifique se o token de confirmação foi preenchido corretamente.");
        return;
    }

    $('#btn-validartoken').LoadingOverlay('show');
    alt.emit('validarToken', $('#token').val());
}

function mostrarErro(erro) {
    $('#btn-reenviaremail').LoadingOverlay('hide');
    $('#btn-validartoken').LoadingOverlay('hide');
    $('#token').val('');
    $.alert(erro);
}

function mostrarSucesso(sucesso) {
    $('#btn-reenviaremail').LoadingOverlay('hide');
    $('#btn-validartoken').LoadingOverlay('hide');
    $('#token').val('');
    $.alert(sucesso);
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarErro', mostrarErro);
    alt.on('mostrarSucesso', mostrarSucesso);
}