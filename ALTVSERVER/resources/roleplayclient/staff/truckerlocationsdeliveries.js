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
function addEdit(id, posX, posY, posZ) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Entrega` : `Editar Entrega ${id}`,
        content: '<form action="">' +
        '<button type="button" onclick="getPosition()" class="btn btn-sm btn-dark">Obter Posição</button><br/>' +
        '<div class="form-group">' +
        '<label>Posição X</label>' +
        `<input value="${posX}" id="posX" type="numner" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Posição Y</label>' +
        `<input value="${posY}" id="posY" type="numner" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Posição Z</label>' +
        `<input value="${posZ}" id="posZ" type="numner" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
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
                    
                    alt.emit('save', 
                        id,
                        Number(posX), 
                        Number(posY),  
                        Number(posZ)
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
            $('#posX').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

function getPosition() {
    alt.emit('getPosition');
}

function setPosition(x, y, z) {
    $('#posX').val(x);
    $('#posY').val(y);
    $('#posZ').val(z);
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '');
});

function loaded(truckerLocationsDeliveries) {
    $('#tbody-truckerlocationsdeliveries').html(truckerLocationsDeliveries);
}

function goto(id) {
    alt.emit('goto', id);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function edit(id) {
    const truckerLocationDelivery = JSON.parse($(`#json${id}`).val());
    addEdit(id, truckerLocationDelivery.PosX, truckerLocationDelivery.PosY, truckerLocationDelivery.PosZ);
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
    alt.on('setPosition', setPosition);
}