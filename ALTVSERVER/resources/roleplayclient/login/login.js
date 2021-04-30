$(document).ready(function () {
    $("#password").keyup(function( event ) {
        event.preventDefault();
        if (event.keyCode === 13) 
            $("#btn-entrar").click();
    });
});

function entrar() {
    if (!$('#user').val() || !$('#password').val()) {
        mostrarErro("Verifique se todos os campos foram preenchidos corretamente.");
        return;
    }

    $('#btn-entrar').LoadingOverlay('show');
    alt.emit('entrarUsuario', $('#user').val(), $('#password').val());
}

function registrar() {
    alt.emit('registrarUsuario');
}

function esqueciMinhaSenha() {
    alt.emit('esqueciMinhaSenha');
}

function showHTML(usuario) {
    $('#user').val(usuario);
    if (usuario != "")
    {
        $('#password').focus();
        $('#btn-registrar').hide();
    }
}

function mostrarErro(erro) {
    $('#btn-entrar').LoadingOverlay('hide');
    if (erro != "") {
        $('#password').val('');
        $('#erro').html(erro);
        $('#erro').css('display', 'block');
    }
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarErro', mostrarErro);
}