
function resetarSenha() {
    if (!$('#usuario').val() && !$('#email').val()) {
        mostrarErro("Verifique se os campos foram preenchidos corretamente.");
        return;
    }
    
    $('#btn-resetarsenha').LoadingOverlay('show');
    alt.emit('confirmar', $('#usuario').val(), $('#email').val());
}

function voltarLogin() {
    alt.emit("voltarLogin");
}

function mostrarSucesso(sucesso) {
    $('#usuario').val('');
    $('#email').val('');
    $('#btn-resetarsenha').LoadingOverlay('hide');
    if (sucesso != "") {
        $('#sucesso').html(sucesso);
        $('#sucesso').css('display', 'block');
    }
}

if('alt' in window)
    alt.on('mostrarSucesso', mostrarSucesso);