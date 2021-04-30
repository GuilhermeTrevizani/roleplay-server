$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function abrirSpawnVeiculos(faccao, x) {
    $('#titulo').text(faccao + ' • Spawn de Veículos');
    $('#tbody-veiculos').html('');

    let itens = JSON.parse(x);
    itens.forEach(function(p) {
	    $("#tbody-veiculos").append(`<tr><td>${p.Codigo}</td> <td>${p.Modelo}</td> <td>${p.NomeExibicao}</td> <td>${p.Placa}</td> <td>${p.Numeracao}</td> <td>${p.Encarregado}</td> <td class="text-center"><button class="btn btn-xs btn-primary" type="button" onclick="spawnarVeiculo(${p.Codigo})">Spawnar</button></td></tr>`);
    });
}

function spawnarVeiculo(item) {
    alt.emit("spawnarVeiculo", parseInt(item));
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window)
    alt.on('abrirSpawnVeiculos', abrirSpawnVeiculos);