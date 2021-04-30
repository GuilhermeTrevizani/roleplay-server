let r1 = 0, g1 = 0, b1 = 0;
let r2 = 0, g2 = 0, b2 = 0;
let cor1 = '#000000';

$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(function () {
    $("#pesquisa").on('input', function () {
        var pesquisa = removerAcentos($("#pesquisa").val());
        $.each($(".pesquisaveh"), function (index, element) {
            $(element).show();

            if (pesquisa != "") {
                if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                    $(element).hide();
            }
        });
    });
});

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

function comprarVeiculo() {
    alt.emit("confirmarCompra", $("#veiculo").val(), r1, g1, b1, r2, g2, b2);
}

function updateCor1(picker) {
    r1 = parseInt(picker.rgb[0]);
    g1 = parseInt(picker.rgb[1]);
    b1 = parseInt(picker.rgb[2]);
    cor1 = picker.toHEXString();
}

function updateCor2(picker) {
    r2 = parseInt(picker.rgb[0]);
    g2 = parseInt(picker.rgb[1]);
    b2 = parseInt(picker.rgb[2]);
}

function showHTML(titulo, x) {
    $('#titulo').html(titulo);
    let vehs = JSON.parse(x);
    vehs.forEach(function(p) {
	    $("#tbody-veiculos").append(`<tr class="pesquisaveh"><td>${p.Exibicao}</td><td>${p.Nome}</td><td>${p.Restricao}</td><td>${p.Preco}</td></tr>`);
    });
}

function copiarCorPrimaria() {
    r2 = r1;
    g2 = g1;
    b2 = b1;
    document.querySelector('#corsecundaria').jscolor.fromString(cor1);
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window)
    alt.on('showHTML', showHTML);