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
function addEdit(id, category, name, model, value) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Mobília` : `Editar Mobília ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Categoria</label>' +
        `<input value="${category}" id="category" type="text" class="form-control" maxlength="50"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Nome</label>' +
        `<input value="${name}" id="name" type="text" class="form-control" maxlength="50"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Modelo</label>' +
        `<input value="${model}" id="model" type="text" class="form-control" maxlength="50"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Valor</label>' +
        `<input value="${value}" id="value" type="number" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const category = this.$content.find('#category').val();
                    if (!category) {
                        $.alert('Categoria é obrigatória.');
                        return false;
                    }

                    const name = this.$content.find('#name').val();
                    if (!name) {
                        $.alert('Nome é obrigatório.');
                        return false;
                    }

                    const model = this.$content.find('#model').val();
                    if (!model) {
                        $.alert('Modelo é obrigatório.');
                        return false;
                    }

                    const value = this.$content.find('#value').val();
                    if (!value) {
                        $.alert('Valor é obrigatório.');
                        return false;
                    }

                    alt.emit('save', 
                        id, 
                        category, 
                        name, 
                        model, 
                        Number(value));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            $('#category').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

$('#btn-add').click(() => {
    addEdit(0, '', '', '', '');
});

function loaded(furnitures) {
    $('#tbody-furnitures').html(furnitures);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
}

function edit(id) {
    const furniture = JSON.parse($(`#json${id}`).val());
    addEdit(id, furniture.Category, furniture.Name, furniture.Model, furniture.Value);
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