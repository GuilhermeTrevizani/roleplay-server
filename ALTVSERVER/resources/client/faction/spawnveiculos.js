$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function abrirSpawnVeiculos(faccao, x) {
    $('#titulo').text(faccao + ' • Spawn de Veículos');
    $('#tbody-veiculos').html('');

    let itens = JSON.parse(x);
    itens.forEach(function(p) {
	    $("#tbody-veiculos").append(`<tr>
            <td>${p.Id}</td> 
            <td>${p.Model}</td> 
            <td>${p.Name}</td> 
            <td>${p.Plate}</td> 
            <td>${p.LiveryName}</td> 
            <td>${p.InChargeCharacterName}</td> 
            <td class="text-center">
                <button class="btn btn-xs btn-primary" type="button" onclick="spawnarVeiculo(${p.Id})">Spawnar</button>
            </td>
        </tr>`);
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