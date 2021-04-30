function registrar() {
    if (!$('#user').val() || !$('#email').val() || !$('#password').val() || !$('#password2').val()) {
        mostrarErro("Verifique se todos os campos foram preenchidos corretamente.");
        return;
    }

    $('#btn-registrar').LoadingOverlay('show');
    alt.emit("registrarUsuario", $('#user').val(), $('#email').val(), $('#password').val(), $('#password2').val());
}

function voltarLogin() {
    alt.emit("voltarLogin");
}

function mostrarErro(erro) {
    $('#btn-registrar').LoadingOverlay('hide');
    if (erro != "")  {
        $('#erro').html(erro);
        $('#erro').css('display', 'block');
    }
}

if('alt' in window)
    alt.on('mostrarErro', mostrarErro);