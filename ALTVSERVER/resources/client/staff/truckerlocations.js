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
function addEdit(id, name, posX, posY, posZ, deliveryValue, loadWaitTime, unloadWaitTime, allowedVehiclesJSON) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Localização de Caminhoneiro` : `Editar Localização de Caminhoneiro ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Nome</label>' +
        `<input value="${name}" id="name" type="text" class="form-control" maxlength="50"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Posição X</label>' +
        `<input value="${posX}" id="posX" type="text" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Posição Y</label>' +
        `<input value="${posY}" id="posY" type="text" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Posição Z</label>' +
        `<input value="${posZ}" id="posZ" type="text" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Valor por Entrega</label>' +
        `<input value="${deliveryValue}" id="deliveryValue" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Tempo de Espera no Carregamento</label>' +
        `<input value="${loadWaitTime}" id="loadWaitTime" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Tempo de Espera por Entrega</label>' +
        `<input value="${unloadWaitTime}" id="unloadWaitTime" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Veículos Permitidos</label>' +
        `<select id="allowedVehiclesJSON" class="form-control" multiple></select>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const name = this.$content.find('#name').val();
                    if (!name) {
                        $.alert('Nome é obrigatório.');
                        return false;
                    }

                    const posX = this.$content.find('#posX').val();
                    if (!posX) {
                        $.alert('Posição X é obrigatória.');
                        return false;
                    }

                    const posY = this.$content.find('#posY').val();
                    if (!posY) {
                        $.alert('Posição Y é obrigatória.');
                        return false;
                    }

                    const posZ = this.$content.find('#posZ').val();
                    if (!posZ) {
                        $.alert('Posição Z é obrigatória.');
                        return false;
                    }

                    const deliveryValue = this.$content.find('#deliveryValue').val();
                    if (!deliveryValue) {
                        $.alert('Valor por Entrega é obrigatório.');
                        return false;
                    }

                    const loadWaitTime = this.$content.find('#loadWaitTime').val();
                    if (!loadWaitTime) {
                        $.alert('Tempo de Espera no Carregamento é obrigatório.');
                        return false;
                    }

                    const unloadWaitTime = this.$content.find('#unloadWaitTime').val();
                    if (!unloadWaitTime) {
                        $.alert('Tempo de Espera por Entrega é obrigatório.');
                        return false;
                    }

                    if (!$('#allowedVehiclesJSON').val()) {
                        $.alert('Veículos Permitidos é obrigatório.');
                        return false;
                    }

                    alt.emit('save', 
                        id,
                        name,
                        Number(posX), 
                        Number(posY),  
                        Number(posZ), 
                        parseInt(deliveryValue),
                        parseInt(loadWaitTime),
                        parseInt(unloadWaitTime),
                        JSON.stringify($('#allowedVehiclesJSON').val())
                    );
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            $('#name').focus();

            allowedVehiclesJSON.forEach((x) => {
                $('#allowedVehiclesJSON').append(`<option value='${x}'>${x}</option>`);
            });

            $('#allowedVehiclesJSON').select2({ tags: true });

            $('#allowedVehiclesJSON').val(allowedVehiclesJSON).change();

            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '', '', '', '', '', []);
});

function loaded(truckerLocations) {
    $('#tbody-truckerlocations').html(truckerLocations);
}

function goto(id) {
    alt.emit('goto', id);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function edit(id) {
    const truckerLocation = JSON.parse($(`#json${id}`).val());
    addEdit(id, truckerLocation.Name, truckerLocation.PosX, truckerLocation.PosY, truckerLocation.PosZ, 
        truckerLocation.DeliveryValue, truckerLocation.LoadWaitTime,truckerLocation.UnloadWaitTime,
        JSON.parse(truckerLocation.AllowedVehiclesJSON));
}

function editDeliveries(id) {
    alt.emit('editDeliveries', id);
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