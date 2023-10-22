$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(() => {
    $("#pesquisa").on('input', function () {
        var pesquisa = removerAcentos($("#pesquisa").val());
        $.each($(".searchMember"), function (index, element) {
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
        title: 'Adicionar Membro',
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>ID do Personagem</label>' +
        `<input id="id" type="number" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const id = this.$content.find('#id').val();
                    if (!id) {
                        $.alert('ID do Personagem é obrigatório.');
                        return false;
                    }

                    alt.emit('invite', parseInt(id));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            $('#id').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
});

function loaded(members) {
    $('#tbody-factionmembers').html(members);
}

function editMember(id) {
    const info = JSON.parse($(`#jsonMember${id}`).val());
    const infoFlags = JSON.parse(info.FactionFlagsJSON);

    modal = $.confirm({
        title: `Editar ${info.Name}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>ID do Personagem</label>' +
        `<input value="${id}" id="id" type="number" readonly class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Rank</label>' +
        `<select id="rank" class="form-control"></select>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Flags</label>' +
        `<select id="flags" class="form-control" multiple></select>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Distintivo</label>' +
        `<input value="${info.Badge}" id="badge" type="number" ${!government ? 'readonly' : ''} class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const rank = this.$content.find('#rank').val();
                    if (!rank) {
                        $.alert('Rank é obrigatório.');
                        return false;
                    }

                    alt.emit('save', 
                        parseInt(id), 
                        parseInt(rank), 
                        parseInt($('#badge').val()), 
                        JSON.stringify($('#flags').val()));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            ranks.forEach((x) => {
                $('#rank').append(`<option value='${x.Id}'>${x.Name}</option>`);
            });

            flags.forEach((x) => {
                $('#flags').append(`<option value='${x.Id}'>${x.Name}</option>`);
            });

            $('#rank').val(info.FactionRankId).change();
            $('#flags').val(infoFlags).change();

            $('#rank').focus();

            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

function removeMember(id, name) {
    modal = $.confirm({
        title: `Expulsar ${name}`,
        content: `Confirmar expulsar ${name} da facção?`,
        buttons: {
            formSubmit: {
                text: 'Confirmar',
                btnClass: 'btn-green',
                action: function () {
                    alt.emit('remove', 
                        parseInt(id));
                    return false;
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
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

let ranks;
let government;
let flags;

function init(factionName, _ranks, _flags, _government) {
    $('#span-factionName').html(factionName);
    ranks = _ranks;
    flags = _flags;
    government = _government;

    if (government) {
        $('#th-badge').show();
        $('#th-status').show();
    }
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarMensagem', mostrarMensagem);
    alt.on('init', init);
}