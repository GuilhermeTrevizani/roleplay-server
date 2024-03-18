function abrirUCP(htmlComandos, htmlMinhasInformacoes, htmlConfiguracoes) {
  $('#tab-comandos').html(htmlComandos);
  $('#tab-minhasinformacoes').html(htmlMinhasInformacoes);
  $('#tab-configuracoes').html(htmlConfiguracoes);

  $("#pesquisa").on('input', function () {
    filtrarComandos();
  });

  $("#sel-categoria").on('change', function () {
    filtrarComandos();
  });
}

function gravar() {
  alt.emit('gravar',
    $('#chk-timestamp').is(":checked"),
    $('#chk-dl').is(":checked"),
    $('#chk-anuncios').is(":checked"),
    $('#chk-pm').is(":checked"),
    $('#chk-chatfaccao').is(":checked"),
    $('#chk-chatstaff').is(":checked"),
    parseInt($('#sel-tipofontechat').val()),
    parseInt($('#tamanhofontechat').val()),
    parseInt($('#linhaschat').val()),
    $('#chk-staff').is(":checked"));
}

function filtrarComandos() {
  var pesquisa = removerAcentos($("#pesquisa").val());
  $.each($(".pesquisaitem"), function (index, element) {
    $(element).show();

    if ($('#sel-categoria').val() != 'Todas' && $(element).data('categoria') != $('#sel-categoria').val()) {
      $(element).hide();
    } else if (pesquisa != "") {
      if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
        $(element).hide();
    }
  });
}

if ('alt' in window) {
  alt.on('abrirUCP', abrirUCP);
}