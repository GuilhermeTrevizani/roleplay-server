function showHTML(nome, x, slots, alerta) {
  let players = JSON.parse(x);

  if (players.length != slots)
    $('#btn-criarpersonagem').show();

  if (players.length > slots)
    slots = players.length;

  $('#titulo').html(`Personagens de ${nome} (${players.length} de ${slots})`);

  $('#tbody-personagens').html('');
  if (players.length == 0) {
    $('#tbody-personagens').html('<tr><td scope="row" colspan="4" class="text-center">Você não possui personagens.</td></tr>');
  } else {
    players.forEach(function (p) {
      $('#tbody-personagens').append(`<tr>
                <td>${p.Id}</td>
                <td>${p.Name}</td>
                <td class='text-center'>${p.Status}</td>
                <td class='text-center'>${p.Options}</td>
            </tr>`);
    });
  }

  if (alerta)
    $.alert(alerta);
}

function selecionarPersonagem(id, namechange) {
  $(`.btn-selecionarpersonagem${id}`).LoadingOverlay('show');
  alt.emit('selecionarPersonagem', id, namechange);
}

function criarPersonagem() {
  alt.emit('criarPersonagem');
}

function atualizar() {
  $('#btn-atualizar').LoadingOverlay('show');
  alt.emit('atualizar');
}

function punicoesAdministrativas() {
  alt.emit('punicoesAdministrativas');
}

function deletarPersonagem(id) {
  $.confirm({
    title: `Deletar Personagem #${id}`,
    content: `Confirma a exclusão do personagem #${id}? O nome do personagem não poderá ser reutilizado.`,
    buttons: {
      confirm: {
        text: 'Sim',
        btnClass: 'btn-green',
        action: () => {
          alt.emit('deletarPersonagem', id);
        }
      },
      cancel: {
        text: 'Não',
        btnClass: 'btn-red'
      }
    }
  });
}

if ('alt' in window) {
  alt.on('showHTML', showHTML);

  alt.on('mostrarErro', (message, component) => {
    $.alert(message);
    $(component).LoadingOverlay('hide');
  });
}