$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(function () {
    $('#btn-pesquisarpropriedade').click(function() {
        if ($('#input-pesquisarpropriedade').val() == '') {
            $('#div-pesquisarpropriedade').html("<div class='alert alert-danger'>A pesquisa não foi informada corretamente.</div>");
            return;
        }

        $('#btn-pesquisarpropriedade').LoadingOverlay('show');
        alt.emit('pesquisarPropriedade', $('#input-pesquisarpropriedade').val());
    });

    $('#btn-pesquisarveiculo').click(function() {
        if ($('#input-pesquisarveiculo').val() == '') {
            $('#div-pesquisarveiculo').html("<div class='alert alert-danger'>A pesquisa não foi informada corretamente.</div>");
            return;
        }

        $('#btn-pesquisarveiculo').LoadingOverlay('show');
        alt.emit('pesquisarVeiculo', $('#input-pesquisarveiculo').val());
    });

    $('#btn-pesquisarpessoa').click(function() {
        if ($('#input-pesquisarpessoa').val() == '') {
            $('#div-pesquisarpessoa').html("<div class='alert alert-danger'>A pesquisa não foi informada corretamente.</div>");
            return;
        }

        $('#btn-pesquisarpessoa').LoadingOverlay('show');
        alt.emit('pesquisarPessoa', $('#input-pesquisarpessoa').val());
    });
});

function abrirMDC(tipoFaccao, nomeFaccao, ligacoes911, apb, bolo, relatoriosPendentes) {
    if (tipoFaccao == 4) { // Governo
        $('#li-pesquisarpessoa').hide();
        $('#li-911').hide();
        $('#li-apb').hide();
        $('#li-bolo').hide();
    } else if (tipoFaccao == 2) { // Médica
        $('#li-pesquisarpessoa').hide();
        $('#li-pesquisarveiculo').hide();
        $('#li-pesquisarpropriedade').hide();
        $('#li-apb').hide();
        $('#li-bolo').hide();
        $('#li-relatoriospendentes').hide();
    }

    $('#titulo').html(`${nomeFaccao} • Mobile Data Computer`);
    $('#tab-911').html(ligacoes911);
    $('#tab-apb').html(apb);
    $('#tab-bolo').html(bolo);
    $('#tab-relatoriospendentes').html(relatoriosPendentes);
}

function atualizarMDC(botao, div, html) {
    (botao != '') ? $(`#${botao}`).LoadingOverlay('hide') : $.LoadingOverlay('hide');
    $(`#${div}`).html(html);
}

function rastrear911(codigo) {
    alt.emit('rastrear911', codigo);
}

function rastrearVeiculo(codigo) {
    alt.emit('rastrearVeiculo', codigo);
}

function adicionarBOLO(tipo, codigo, pesquisa) {
    $.confirm({
        title: 'Adicionar BOLO/APB',
        content: '' +
        '<form action="" class="formName">' +
        '<div class="form-group">' +
        '<label>Informe o motivo:</label>' +
        '<textarea type="text" class="name form-control" rows="5" required spellcheck="false" autofocus></textarea>' +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Confirmar',
                btnClass: 'btn-green',
                action: function () {
                    var motivo = this.$content.find('.name').val();
                    if(!motivo){
                        $.alert('O motivo não foi informado.');
                        return false;
                    }

                    alt.emit('adicionarBOLO', tipo, codigo, motivo, pesquisa);
                }
            },
            cancel: { 
                text: 'Cancelar', 
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

function removerBOLO(codigo, pesquisa) {
    $.confirm({
        title: `Remover BOLO/APB #${codigo}`,
        content: `Confirma a remoção do BOLO/APB #${codigo}?`,
        buttons: {
            confirm: {
                text: 'Sim', 
                btnClass: 'btn-green',
                action: () => {
                    alt.emit('removerBOLO', codigo, pesquisa);
                }
            },
            cancel: { 
                text: 'Não', 
                btnClass: 'btn-red' 
            } 
        }
    });
}

function multar(codigo, nome) {
    $.confirm({
        title: `Multar ${nome}`,
        content: '' +
        '<form action="" class="formName">' +
        '<div class="form-group">' +
        '<label>Informe o valor:</label>' +
        '<input type="text" class="valor form-control" required spellcheck="false" autofocus>' +
        '<label>Informe o motivo:</label>' +
        '<textarea type="text" class="name form-control" rows="5" required spellcheck="false"></textarea>' +
        '<label>Informe a descrição:</label>' +
        '<textarea type="text" class="descricao form-control" rows="5" required spellcheck="false"></textarea>' +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Confirmar',
                btnClass: 'btn-green',
                action: function () {
                    var valor =  parseInt(this.$content.find('.valor').val());
                    if(isNaN(valor) || valor == 0){
                        $.alert('O valor não foi informado corretamente.');
                        return false;
                    }
                    
                    var motivo = this.$content.find('.name').val();
                    if(!motivo){
                        $.alert('O motivo não foi informado.');
                        return false;
                    }

                    var descricao = this.$content.find('.descricao').val();
                    if(!descricao){
                        $.alert('A descrição não foi informada.');
                        return false;
                    }

                    alt.emit('multar', codigo, nome, valor, motivo, descricao);
                }
            },
            cancel: { 
                text: 'Cancelar', 
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

function revogarLicenca(codigo, nome) {
    $.confirm({
        title: `Revogar licença de motorista ${nome}`,
        content: `Confirma revogar a licença de motorista de ${nome}}?`,
        buttons: {
            confirm: {
                text: 'Sim', 
                btnClass: 'btn-green',
                action: () => {
                    alt.emit('revogarLicenca', codigo, pesquisa);
                }
            },
            cancel: { 
                text: 'Não', 
                btnClass: 'btn-red' 
            } 
        }
    });
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window) {
    alt.on('abrirMDC', abrirMDC);
    alt.on('atualizarMDC', atualizarMDC);
}