$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(() => {
    $("#pesquisa").on('input', function () {
        var pesquisa = removerAcentos($("#pesquisa").val());
        $.each($(".pesquisaitem"), function (index, element) {
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

let modal;
function addEdit(id, weapon, ammo, quantity, tintIndex, components) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Arma` : `Editar Arma ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Arma</label>' +
        `<input value="${weapon}" id="weapon" type="text" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Munição</label>' +
        `<input value="${ammo}" id="ammo" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Estoque</label>' +
        `<input value="${quantity}" id="quantity" type="numner" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Pintura</label>' +
        `<input value="${tintIndex}" id="tintIndex" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Componentes</label>' +
        `<select id="components" multiple class="form-control"></select>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const weapon = this.$content.find('#weapon').val();
                    if (!weapon) {
                        $.alert('Arma é obrigatória.');
                        return false;
                    }

                    const ammo = this.$content.find('#ammo').val();
                    if (!ammo) {
                        $.alert('Munição é obrigatória.');
                        return false;
                    }

                    const quantity = this.$content.find('#quantity').val();
                    if (!quantity) {
                        $.alert('Estoque é obrigatório.');
                        return false;
                    }

                    const tintIndex = this.$content.find('#tintIndex').val();
                    if (!tintIndex) {
                        $.alert('Pintura é obrigatória.');
                        return false;
                    }
                    
                    alt.emit('save', 
                        id,
                        weapon, 
                        parseInt(ammo), 
                        parseInt(quantity),  
                        parseInt(tintIndex), 
                        JSON.stringify($('#components').val()));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            components.forEach((x) => {
                $('#components').append(`<option value='${x}' selected>${x}</option>`);
            });

            $('#components').select2({
                tags: true,
                modal: this,
            });
            $('#weapon').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '', '', []);
});

function loaded(factionsArmoriesWeapons) {
    $('#tbody-factionsarmoriesweapons').html(factionsArmoriesWeapons);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function edit(id) {
    const factionArmoryWeapon = JSON.parse($(`#json${id}`).val());
    addEdit(id, factionArmoryWeapon.Weapon, factionArmoryWeapon.Ammo, factionArmoryWeapon.Quantity,
        factionArmoryWeapon.TintIndex, factionArmoryWeapon.Components);
}

function closeView() {
    alt.emit('closeView');
}

function mostrarMensagem(mensagem, fechar) {
    $.alert(mensagem);

    if (fechar && modal) {
        modal.close();
        modal = null;
    }
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarMensagem', mostrarMensagem);
}