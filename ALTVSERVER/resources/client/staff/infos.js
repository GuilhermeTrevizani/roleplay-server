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

$('#btn-add').click(() => {
    modal = $.confirm({
        title: `Adicionar Informação`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Dias</label>' +
        `<input id="days" type="number" class="form-control" />` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Mensagem</label>' +
        `<input id="message" type="text maxlength="300" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const days = this.$content.find('#days').val();
                    if (!days) {
                        $.alert('Dias é obrigatório.');
                        return false;
                    }

                    const message = this.$content.find('#message').val();
                    if (!message) {
                        $.alert('Mensagem é obrigatória.');
                        return false;
                    }

                    alt.emit('save',
                        parseInt(days),
                        message
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
            $('#days').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
});

function loaded(infos) {
    $('#tbody-infos').html(infos);
}

function goto(id) {
    alt.emit('goto', id);
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
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