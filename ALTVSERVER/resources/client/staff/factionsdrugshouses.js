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
function addEdit(id, factionId, posX, posY, posZ, dimension) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Drug House` : `Editar Drug House ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>ID da Facção</label>' +
        `<input value="${factionId}" id="factionId" type="number" class="form-control"/>` +
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
        '<label>Dimensão</label>' +
        `<input value="${dimension}" id="dimension" type="number" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const factionId = this.$content.find('#factionId').val();
                    if (!factionId) {
                        $.alert('ID da Facção é obrigatório.');
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

                    const dimension = this.$content.find('#dimension').val();
                    if (!dimension) {
                        $.alert('Dimensão é obrigatório.');
                        return false;
                    }

                    alt.emit('save', id,
                        parseInt(factionId), 
                        Number(posX), 
                        Number(posY),  
                        Number(posZ), 
                        parseInt(dimension));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            $('#factionId').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '', '', '', '', '');
});

function loaded(factionsDrugsHouses) {
    $('#tbody-factionsdrugshouses').html(factionsDrugsHouses);
}

function goto(id) {
    alt.emit('goto', id);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function edit(id) {
    const factionDrugHouse = JSON.parse($(`#json${id}`).val());
    addEdit(id, factionDrugHouse.FactionId, factionDrugHouse.PosX, factionDrugHouse.PosY, factionDrugHouse.PosZ, factionDrugHouse.Dimension);
}

function editItems(id) {
    alt.emit('editItems', id);
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