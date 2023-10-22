let faction;
let government;

if('alt' in window) {
    alt.on('init', (_faction, _government) => {
        faction = _faction;
        government = _government;

        $('#title').text(`${faction.Name} • Arsenal`);
        
        if (government) 
            $('#btn-giveBack').show();
        else
            $('#th-price').show();

        if (faction.Type == 1)
            $('#btn-equipArmor').show();
    });

    alt.on('loaded', (weaponsHtml) => {
        $('#tbody-weapons').html(weaponsHtml);
    });

    alt.on('mostrarMensagem', (mensagem) => {
        $.alert(mensagem);
    });
}

$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function equip(item) {
    alt.emit('equipWeapon', parseInt(item));
}

function closeView() {
    alt.emit('closeView');
}

$('#btn-giveBack').click(() => {
    alt.emit('giveBack');
});

$('#btn-equipArmor').click(() => {
    alt.emit('equipArmor');
});