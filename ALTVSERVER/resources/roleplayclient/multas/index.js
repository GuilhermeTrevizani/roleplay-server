$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function abrirMultas(x) {
    $('#tbody-multas').html('');

    let multas = JSON.parse(x);
    multas.forEach(function(p) {
	    $("#tbody-multas").append(`<tr><td>${p.Codigo}</td> <td>${p.Data}</td> <td>${p.Valor}</td> <td>${p.Motivo}</td> <td class="text-center"><button class="btn btn-xs btn-primary" type="button" onclick="pagarMulta(${p.Codigo})">Pagar</button></td></tr>`);
    });

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

function pagarMulta(codigo) {
    alt.emit("pagarMulta", codigo);
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window) 
    alt.on('abrirMultas', abrirMultas);