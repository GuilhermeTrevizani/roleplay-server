function requestDiscordToken() {
    alt.emit('requestDiscordToken');
}

function mostrarErro(erro) {
    $('#erro').html(erro);
    $('.login-box-body').css('display', 'block');
}

if('alt' in window)
    alt.on('mostrarErro', mostrarErro);