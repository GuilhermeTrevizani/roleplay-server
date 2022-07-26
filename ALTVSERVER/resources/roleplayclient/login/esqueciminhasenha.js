$(document).ready(() => {
    $('#btn-enviartoken').click(() => {
        $('#btn-enviartoken').LoadingOverlay('show');
        alt.emit('confirmar', $('#email').val());
    });
});

function voltarLogin() {
    alt.emit("voltarLogin");
}