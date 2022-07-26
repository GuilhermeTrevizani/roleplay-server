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
function addEdit(id, name, posX, posY, posZ, type, color) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Blip` : `Editar Blip ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Nome</label>' +
        `<input value="${name}" id="name" type="text" class="form-control" maxlength="50"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Tipo</label>' +
        `<input value="${type}" id="type" type="number" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Cor</label>' +
        `<input value="${color}" id="color" type="number" class="form-control"/>` +
        '</div>' +
        '<button type="button" onclick="getPosition()" class="btn btn-sm btn-dark">Obter Posição</button><br/>' +
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
                    
                    const type = this.$content.find('#type').val();
                    if (!type) {
                        $.alert('Tipo é obrigatório.');
                        return false;
                    }
                    
                    const color = this.$content.find('#color').val();
                    if (!color) {
                        $.alert('Cor é obrigatória.');
                        return false;
                    }

                    alt.emit('save', 
                        id, 
                        name, 
                        Number(posX), 
                        Number(posY),  
                        Number(posZ),  
                        parseInt(type), 
                        parseInt(color));
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
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '', '', '', '');
});

function loaded(blips) {
    $('#tbody-blips').html(blips);
}

function ir(id) {
    alt.emit('goto', id);
}

function excluir(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function editar(id) {
    const blip = JSON.parse($(`#json${id}`).val());
    addEdit(id, blip.Name, blip.PosX, blip.PosY, blip.PosZ, blip.Type, blip.Color);
}

function getPosition() {
    alt.emit('getPosition');
}

function setPosition(x, y, z) {
    $('#posX').val(x);
    $('#posY').val(y);
    $('#posZ').val(z);
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