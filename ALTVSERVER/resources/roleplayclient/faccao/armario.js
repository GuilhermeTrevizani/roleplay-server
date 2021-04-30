$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

let isGoverno = false;

function abrirArmario(faccao, armas, componentes, governo, policia, precoComponente) {
    $('#titulo').text(`${faccao} • Armário`);
    $('#precocomponente').html(precoComponente);

    isGoverno = governo;
    if (!isGoverno) 
        $('#btn-devolveritens').hide();
    else
        $('#th-preco').hide();

    if (!policia)
        $('#btn-equiparcolete').hide();

    atualizarArmario(armas, componentes);
}

function atualizarArmario(armas, componentes) {
    $('#tbody-armas').html('');
    $('#tbody-componentes').html('');
    
    let xArmas = JSON.parse(armas);
    if (xArmas.length == 0)
        $('#tbody-armas').html('<tr><td colspan="6" class="text-center">O armário não possui armas.</td></tr>');

    xArmas.forEach(function(p) {
	    $('#tbody-armas').append(`<tr><td>${p.Arma}</td> <td>${p.Municao}</td> <td>${p.Estoque}</td> <td>${p.Rank}</td> ${(!isGoverno ? `<td>${p.Preco}</td>` : '')} <td class="text-center"><button class="btn btn-xs btn-primary" onclick="equiparItem(${p.Item})">Equipar</button></td></tr>`);
    });

    let xComponentes = JSON.parse(componentes);
    if (xComponentes.length == 0)
        $('#li-componentes').hide();
    else
        $('#li-componentes').show();

    xComponentes.forEach(function(p) {
	    $('#tbody-componentes').append(`<tr><td>${p.Arma}</td> <td>${p.Componente}</td> <td class="text-center"><button class="btn btn-xs btn-primay" onclick="equiparComponente(${p.ItemArma},${p.ItemComponente})">Equipar</button></td></tr>`);
    });
}

function equiparItem(item) {
    alt.emit('equiparItem', parseInt(item));
}

function equiparComponente(arma, componente) {
    alt.emit('equiparComponente', parseInt(arma), parseInt(componente));
}

function closeView() {
    alt.emit('closeView');
}

function devolverItens() {
    alt.emit('devolverItens');
}

function equiparColete() {
    alt.emit('equiparColete');
}

if('alt' in window) {
    alt.on('abrirArmario', abrirArmario);
    alt.on('atualizarArmario', atualizarArmario);
}