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

let modal;
$('#btn-add').click(() => {
    modal = $.confirm({
        title: 'Adicionar Funcionário',
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

                    alt.emit('invite',
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

const CompanyFlag = {
    InviteCharacter: 1,
    EditCharacter: 2,
    RemoveCharacter: 3
};

function loaded(characters, _flags, _isOwner) {
    $('#tbody-characters').html(characters);

    if (_flags.includes(CompanyFlag.InviteCharacter) || _isOwner) {
        $('#col-addMember').show();
        $('#col-searchMembers').attr('class', 'col-md-10');
    } else {
        $('#col-addMember').hide();
        $('#col-searchMembers').attr('class', 'col-md-12');
    }

    if (_flags.includes(CompanyFlag.RemoveCharacter) || _isOwner) {
        $('.removeMember').show();
    } else {
        $('.removeMember').hide();
    }

    if (_flags.includes(CompanyFlag.EditCharacter) || _isOwner) {
        $('.editMember').show();
    } else {
        $('.editMember').hide();
    }

    if (_flags.includes(CompanyFlag.RemoveCharacter)
        || _flags.includes(CompanyFlag.EditCharacter) 
        || _isOwner) {
        $('.tdOptions').show();
    } else {
        $('.tdOptions').hide();
    }
}

let companyFlags;
function loadCompany(companyId, companyName, _companyFlags) {
    $('#h3-title').html(`${companyName} [${companyId}]`);
    companyFlags = _companyFlags;
}

function edit(id) {
    const info = JSON.parse($(`#jsonMember${id}`).val());
    const infoFlags = JSON.parse(info.FlagsJSON);

    modal = $.confirm({
        title: `Editar ${info.Name}`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>ID do Personagem</label>' +
        `<input value="${id}" id="id" type="number" readonly class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Flags</label>' +
        `<select id="flags" class="form-control" multiple></select>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    alt.emit('save', 
                        id, 
                        JSON.stringify($('#flags').val())
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
            companyFlags.forEach((x) => {
                $('#flags').append(`<option value='${x.Id}'>${x.Name}</option>`);
            });

            $('#flags').val(infoFlags).change();
            $('#flags').focus();

            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

function remove(button, id) {
    $(button).LoadingOverlay('show');
    alt.emit('remove', id);        
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
    alt.on('loadCompany', loadCompany);
}