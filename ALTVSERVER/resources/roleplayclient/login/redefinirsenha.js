$(document).ready(() => {
    $('#btn-validartoken').click(() => {
        $('#btn-validartoken').LoadingOverlay('show');
        alt.emit('confirmar', $('#token').val());
    });
});

function voltarLogin() {
    alt.emit("voltarLogin");
}

function showHTML(email) {
    $('#email').val(email);
}

function mostrarSucesso(sucesso) {
    $('#btn-validartoken').LoadingOverlay('hide');
    $('#token').val('');
    $.confirm({
        title: 'Redefinir Senha',
        content: sucesso,
        buttons: {
            confirm: {
                text: 'OK',
                action: function () {
                    voltarLogin();
                }
            },
        }
    });
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarSucesso', mostrarSucesso);
}