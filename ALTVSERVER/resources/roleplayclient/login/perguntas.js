let perguntas;

function showHTML(x) {
    perguntas = JSON.parse(x);
    $("#perguntas").html('');
    perguntas.forEach(function(p) {
        $("#perguntas").append(`<p class='mar-top'>${p.Nome}</p>`);
        p.Respostas.forEach(function(r) {
            $("#perguntas").append(`<div class='radio'><input type='radio' class='magic-radio' id='resposta${r.Codigo}' name='pergunta${p.Codigo}' value='${r.Codigo}'><label for='resposta${r.Codigo}'>${r.Nome}</label></div>`);
        });
    });
}

function confirmar() {
    let temSemResposta = false;

    perguntas.forEach(function(p) {
        p.RespostaSelecionada = parseInt($(`input[name='pergunta${p.Codigo}']:checked`).val());
        if (isNaN(p.RespostaSelecionada))
            temSemResposta = true;
    });

    if (temSemResposta) {
        mostrarErro("Verifique se todas as perguntas foram respondidas.", false);
        return;
    }

    $('#btn-confirmar').LoadingOverlay('show');
    alt.emit('confirmar', JSON.stringify(perguntas));
}

function copiar() {
    var copyText = document.getElementById("link");
    copyText.select();
    document.execCommand("copy");
}

function voltarLogin() {
    alt.emit("voltarLogin");
}

function mostrarErro(erro, limpar = true) {
    $('#btn-confirmar').LoadingOverlay('hide');
    if (erro != "") {
        $('#erro').html(erro);
        $('#erro').css('display', 'block');
    }

    if (limpar)
        $('input').prop('checked', false);
}

if('alt' in window) {
    alt.on('showHTML', showHTML);
    alt.on('mostrarErro', mostrarErro);
}