$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function abrirSpawnVeiculos(titulo, itens) {
    $('#titulo').text(titulo);
    $('#tbody-veiculos').html('');

    itens.forEach(function(p) {
	    $("#tbody-veiculos").append(`<tr>
            <td>${p.Id}</td> 
            <td>${p.Model}</td> 
            <td>${p.Plate}</td> 
            <td class='text-center'>${p.Spawn}</td> 
            <td class='text-center'>${p.Seized}</td> 
            <td class='text-center'>${p.Dismantled}</td> 
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