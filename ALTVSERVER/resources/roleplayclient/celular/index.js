$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function abrirCelular(celular, contatos) {
    $('#titulo').html(`Celular • Número: ${celular}`);
    $('#nome').val('');
    $('#celular').val('');
    $('#tbody-contatos').html('');

    let xContatos = JSON.parse(contatos);
    if (xContatos.length == 0) {
        $('#tbody-contatos').html('<tr><td class="text-center" colspan="3">O celular não possui contatos.</td></tr>');
    } else {
        xContatos.forEach(function(p) {
            $('#tbody-contatos').append(`<tr class="pesquisacon"><td>${p.Nome}</td> <td>${p.Celular}</td><td class="text-center"><button class="btn btn-xs btn-dark" type="button" onclick="enviarLocalizacaoContato(${p.Celular})">Enviar Localização</button> <button class="btn btn-xs btn-primary" type="button" onclick="ligarContato(${p.Celular})">Ligar</button> <button class="btn btn-xs btn-danger" type="button" onclick="removerContato(${p.Celular})">Excluir</button></td></tr>`);
        });
    }

    $("#pesquisa").on('input', function () {
        var pesquisa = removerAcentos($("#pesquisa").val());
        $.each($(".pesquisacon"), function (index, element) {
            $(element).show();

            if (pesquisa != "") {
                if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                    $(element).hide();
            }
        });
    });
}

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

function adicionarContato() {
    alt.emit("adicionarContato", $('#nome').val(), isNaN($('#celular').val()) ? 0 : parseInt($('#celular').val()));
}

function removerContato(celular) {
    alt.emit("removerContato", celular);
}

function ligarContato(celular) {
    alt.emit("ligarContato", celular);
}

function enviarLocalizacaoContato(celular) {
    alt.emit("enviarLocalizacaoContato", celular);
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window)
    alt.on('abrirCelular', abrirCelular);