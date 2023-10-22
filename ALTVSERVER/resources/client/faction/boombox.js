function loaded(title, url, volume) {
    $('#title').html(title);
    $('#input-url').val(url);
    $('#input-volume').val(volume);
}

$('#btn-confirm').click(() => {
    alt.emit('confirm', $('#input-url').val(), Number($('#input-volume').val()));
});

$('#btn-turnOff').click(() => {
    alt.emit('turnOff');
});

function mostrarMensagem(message) {
    if (message)
        $.alert(message);
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarMensagem', mostrarMensagem);
}