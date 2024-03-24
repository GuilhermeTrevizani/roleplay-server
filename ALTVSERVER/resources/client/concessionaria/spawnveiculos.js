function abrirSpawnVeiculos(titulo, itens) {
  $('#titulo').text(titulo);
  $('#tbody-veiculos').html('');

  itens.forEach(function (p) {
    $("#tbody-veiculos").append(`<tr>
            <td>${p.Id}</td> 
            <td>${p.Model}</td> 
            <td>${p.Plate}</td> 
            <td class='text-center'>${p.Spawn}</td> 
            <td class='text-center'>${p.Seized}</td> 
            <td class='text-center'>${p.Dismantled}</td> 
            <td class="text-center">
                <button class="btn btn-xs btn-primary" type="button" onclick="spawnVehicle('${p.Id}')">Spawnar</button>
            </td>
        </tr>`);
  });
}

function spawnVehicle(item) {
  alt.emit("spawnarVeiculo", item);
}

if ('alt' in window)
  alt.on('abrirSpawnVeiculos', abrirSpawnVeiculos);