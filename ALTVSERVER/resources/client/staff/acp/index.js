$('#input-logDataInicial').mask('00/00/0000 00:00:00');
$('#input-logDataFinal').mask('00/00/0000 00:00:00');

let flags;
function updateFlags(_flags) {
  flags = _flags;
}

function updateBans(bans) {
  if (bans.length == 0) {
    $('#tbody-bans').html('<tr><td class="text-center" colspan="8">Não há banimentos ativos.</td></tr>');
  } else {
    $('#tbody-bans').html('');
    bans.forEach(x => {
      const options = flags.includes(1) ?
        `<button onclick="unban(this, ${x.Id}, true)" class="btn btn-dark btn-sm mar-all">Desbanir Total</button> ${(x.User != '' ? `<button onclick="unban(this, ${x.Id}, false)" class="btn btn-dark btn-sm mar-all">Desbanir Usuário</button>` : '')}`
        : '';
      $('#tbody-bans').append(`<tr>
                <td>${x.Id}</td>
                <td>${x.Character}</td>
                <td>${x.User}</td>
                <td>${x.Date}</td>
                <td>${x.ExpirationDate}</td>
                <td>${x.UserStaff}</td>
                <td>${x.Reason}</td>
                <td class="text-center">${options}</td>
            </tr>`);
    });
  }
}

function updateSOS(sos) {
  if (sos.length == 0) {
    $('#tbody-sos').html('<tr><td class="text-center" colspan="5">Não há pedidos de ajuda ativos.</td></tr>');
  } else {
    $('#tbody-sos').html('');
    sos.forEach(x => {
      $('#tbody-sos').append(`<tr>
                <td>${x.CharacterSessionId}</td>
                <td>${x.Date}</td>
                <td>${x.CharacterName}</td>
                <td>${x.UserName}</td>
                <td>${x.Message}</td>
            </tr>`);
    });
  }
}

function closeView() {
  alt.emit('closeView');
}

function unban(button, id, total) {
  $(button).LoadingOverlay('show');
  alt.emit('unban', id, total);
}

function updateLogTypes(logTypes) {
  logTypes.forEach(x => {
    $('#select-logTipo').append(`<option value="${x.Id}">${x.Name}</option>`);
  });
}

$('#btn-logPesquisar').click(() => {
  $('#btn-logPesquisar').LoadingOverlay('show');

  alt.emit('searchLogs',
    $('#input-logDataInicial').val(), $('#input-logDataFinal').val(),
    parseInt($('#select-logTipo').val()),
    $('#input-logPersonagemOrigem').val(), $('#input-logPersonagemDestino').val(),
    $('#input-logDescricao').val());
});

$('#btn-pesquisarusuario').click(() => {
  const search = $('#input-pesquisarusuario').val();
  if (search == '') {
    $('#input-pesquisarusuario').focus();
    $.alert('Informe algo para ser pesquisado.');
    return;
  }

  $('#btn-pesquisarusuario').LoadingOverlay('show');
  alt.emit('searchUser', search);
});

$('#btn-pesquisarpersonagem').click(() => {
  const search = $('#input-pesquisarpersonagem').val();
  if (search == '') {
    $('#input-pesquisarpersonagem').focus();
    $.alert('Informe algo para ser pesquisado.');
    return;
  }

  $('#btn-pesquisarpersonagem').LoadingOverlay('show');
  alt.emit('searchCharacter', search);
});

function updateLogs(logs) {
  $('#btn-logPesquisar').LoadingOverlay('hide');
  if (logs.length == 0) {
    $('#tbody-logs').html('<tr><td class="text-center" colspan="13">Não há logs com os filtros informados.</td></tr>');
  } else {
    $('#tbody-logs').html('');
    logs.forEach(x => {
      $('#tbody-logs').append(`<tr>
                <td>${x.Type}</td>
                <td>${x.Date}</td>
                <td>${x.Description}</td>
                <td>${x.OriginCharacterName}</td>
                <td>${x.OriginIp}</td>
                <td>${x.OriginHardwareIdHash}</td>
                <td>${x.OriginHardwareIdExHash}</td>
                <td>${x.TargetCharacterName}</td>
                <td>${x.TargetIp}</td>
                <td>${x.TargetHardwareIdHash}</td>
                <td>${x.TargetHardwareIdExHash}</td>
            </tr>`);
    });
  }
}

function updateUser(html) {
  $('#div-pesquisarusuario').html(html);
  $('#btn-pesquisarusuario').LoadingOverlay('hide');
  $('#select-staff').select2();
  $('#select-flags').select2();
}

function save() {
  alt.emit('saveUser',
    parseInt($('#input-userId').val()),
    parseInt($('#select-staff').val()),
    JSON.stringify($('#select-flags').val()));
}

function updateCharacter(html) {
  $('#div-pesquisarpersonagem').html(html);
  $('#btn-pesquisarpersonagem').LoadingOverlay('hide');
}

function banirPersonagem(id) {
  $.confirm({
    title: `Banir Personagem`,
    content:
      '<form action="">' +
      '<div class="form-group">' +
      '<label>Dias (0 para permanente)</label>' +
      `<input id="days" type="number" class="form-control"/> <br/>` +
      '<label>Motivo</label>' +
      '<input id="reason" type="text" class="form-control"/>' +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Confirmar',
        btnClass: 'btn-green',
        action: function () {
          const days = this.$content.find('#days').val();
          if (!days) {
            $.alert('Os dias não foram informados.');
            return false;
          }

          const reason = this.$content.find('#reason').val();
          if (!reason) {
            $.alert('O motivo não foi informado.');
            return false;
          }

          alt.emit('banCharacter', id, parseInt(days), reason);
        }
      },
      cancel: {
        text: 'Cancelar',
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
}

function ckAvalationRemoveCharacter(id) {
  alt.emit('ckAvalationRemoveCharacter', id);
}

function ckAvalationCharacter(id) {
  alt.emit('ckAvalationCharacter', id);
}

function nameChangeStatusCharacter(id) {
  alt.emit('nameChangeStatusCharacter', id);
}

function removeForumNameChange(id) {
  alt.emit('removeForumNameChange', id);
}

function removeJailCharacter(id) {
  alt.emit('removeJailCharacter', id);
}

function ckCharacter(id) {
  $.confirm({
    title: `CK no Personagem`,
    content:
      '<form action="">' +
      '<div class="form-group">' +
      '<label>Motivo</label>' +
      '<input id="reason" type="text" class="form-control" maxlength="255"/>' +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Confirmar',
        btnClass: 'btn-green',
        action: function () {
          const reason = this.$content.find('#reason').val();
          if (!reason) {
            $.alert('O motivo não foi informado.');
            return false;
          }

          alt.emit('ckCharacter', id, reason);
        }
      },
      cancel: {
        text: 'Cancelar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#reason').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

function updateStaff(staff) {
  if (staff.length == 0) {
    $('#tbody-staff').html('<tr><td class="text-center" colspan="7">Não há membros na staff.</td></tr>');
  } else {
    $('#tbody-staff').html('');
    staff.forEach(x => {
      $('#tbody-staff').append(`<tr>
                <td>${x.Staff}</td>
                <td>${x.Id}</td>
                <td>${x.Name}</td>
                <td>${x.HelpRequestsAnswersQuantity}</td>
                <td>${x.CharacterApplicationsQuantity}</td>
                <td>${x.StaffDutyTime}</td>
                <td>${x.ConnectedTime}</td>
            </tr>`);
    });
  }
}

function mostrarMensagem(message) {
  $.alert(message);
}

if ('alt' in window) {
  alt.on('updateFlags', updateFlags);
  alt.on('updateBans', updateBans);
  alt.on('updateSOS', updateSOS);
  alt.on('updateLogTypes', updateLogTypes);
  alt.on('updateLogs', updateLogs);
  alt.on('updateUser', updateUser);
  alt.on('updateCharacter', updateCharacter);
  alt.on('mostrarMensagem', mostrarMensagem);
  alt.on('updateStaff', updateStaff);
}