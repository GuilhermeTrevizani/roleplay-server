let flags;
let faction;
let government;

let playerFactionFlags;
let isLeader = null;
let ranks;

const FactionFlag = {
    InviteMember: 1,
    EditMember: 2,
    RemoveMember: 3
};

function toggleLeader() {
    if (isLeader) {
        $('.leader').show();
        $('#col-searchRanks').attr('class', 'col-md-8');
        $("#table-factionranks").tableDnD();
    } else {
        $('.leader').hide();
        $('#col-searchRanks').attr('class', 'col-md-12');
    }
}

if ('alt' in window) {
    alt.on('init', (_flags, _faction, _government) => {
        flags = _flags;
        faction = _faction;
        government = _government;

        $('#h3-title').html(faction.Name);

        if (government) {
            $('#th-badge').show();
            $('#th-status').show();
        }
    });

    alt.on('updateMembers', (htmlMembers, _playerFactionFlags, _isLeader) => {
        playerFactionFlags = _playerFactionFlags;
        isLeader = _isLeader;

        $('#tbody-factionmembers').html(htmlMembers);

        toggleLeader();

        if (playerFactionFlags.includes(FactionFlag.InviteMember) || isLeader) {
            $('#col-addMember').show();
            $('#col-searchMembers').attr('class', 'col-md-10');
        } else {
            $('#col-addMember').hide();
            $('#col-searchMembers').attr('class', 'col-md-12');
        }

        if (playerFactionFlags.includes(FactionFlag.RemoveMember) || isLeader) {
            $('.removeMember').show();
        } else {
            $('.removeMember').hide();
        }

        if (playerFactionFlags.includes(FactionFlag.EditMember) || isLeader) {
            $('.editMember').show();
        } else {
            $('.editMember').hide();
        }

        if (playerFactionFlags.includes(FactionFlag.RemoveMember)
            || playerFactionFlags.includes(FactionFlag.EditMember) 
            || isLeader) {
            $('.tdOptions').show();
        } else {
            $('.tdOptions').hide();
        }
    });

    alt.on('updateRanks', (htmlRanks, _ranks) => {
        ranks = _ranks;

        $('#tbody-factionranks').html(htmlRanks);

        toggleLeader();
    });

    alt.on('mostrarMensagem', (mensagem, fechar) => {
        $.alert(mensagem);
    
        if (fechar && modal) {
            modal.close();
            modal = null;
        }
    });
}

$(document).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$("#searchMembers").on('input', function () {
    var pesquisa = removerAcentos($("#searchMembers").val());
    $.each($(".searchMember"), function (index, element) {
        $(element).show();

        if (pesquisa != "") {
            if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                $(element).hide();
        }
    });
});

$("#searchRanks").on('input', function () {
    var pesquisa = removerAcentos($("#searchRanks").val());
    $.each($(".searchRank"), function (index, element) {
        $(element).show();

        if (pesquisa != "") {
            if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
                $(element).hide();
        }
    });
});

let modal;
function addEditRank(id, name) {
    modal = $.confirm({
        title: id == 0 ? `Adicionar Rank` : `Editar Rank ${id}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Nome</label>' +
        `<input value="${name}" id="name" type="text" maxlength="50" class="form-control"/>` +
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

                    alt.emit('saveRank', 
                        faction.Id,
                        id, 
                        name);
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

$('#btn-addMember').click(() => {
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

                    alt.emit('inviteMember',
                        faction.Id,
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
            $('#id').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
});

$('#btn-addRank').click(() => {
    addEditRank(0, '');
});

$('#btn-orderRanks').click(() => {
    let arRanks = [];
    $('.searchRank').filter('[data-id]').each((i, x) => {
        arRanks.push({
            Id: parseInt($(x).data('id')),
            Position: i + 1,
        })
    });
    alt.emit('orderRank', faction.Id, JSON.stringify(arRanks));
});

function editRank(id) {
    const rank = JSON.parse($(`#jsonRank${id}`).val());
    addEditRank(id, rank.Name);
}

function removeRank(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('removeRank', faction.Id, id);
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

                    alt.emit('saveMember', 
                        faction.Id,
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
                    alt.emit('removeMember',
                        faction.Id,
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

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}