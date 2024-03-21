initSearch('#pesquisa', '.pesquisaitem');

let modal;
function addEdit(id, posX, posY, posZ, dimension, onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Boca de Fumo` : `Editar Boca de Fumo ${id}`,
    content: '<form action="">' +
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
      '<div class="form-group">' +
      '<label>Dimensão</label>' +
      `<input value="${dimension}" id="dimension" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Policiais Online</label>' +
      `<input value="${onlinePoliceOfficers}" id="onlinePoliceOfficers" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Quantidade Limite Cooldown</label>' +
      `<input value="${cooldownQuantityLimit}" id="cooldownQuantityLimit" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Horas Cooldown</label>' +
      `<input value="${cooldownHours}" id="cooldownHours" type="number" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
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

          const dimension = this.$content.find('#dimension').val();
          if (!dimension) {
            $.alert('Dimensão é obrigatório.');
            return false;
          }

          const onlinePoliceOfficers = this.$content.find('#onlinePoliceOfficers').val();
          if (!onlinePoliceOfficers) {
            $.alert('Policiais Online é obrigatório.');
            return false;
          }

          const cooldownQuantityLimit = this.$content.find('#cooldownQuantityLimit').val();
          if (!cooldownQuantityLimit) {
            $.alert('Quantidade Limite Cooldown é obrigatória.');
            return false;
          }

          const cooldownHours = this.$content.find('#cooldownHours').val();
          if (!cooldownHours) {
            $.alert('Horas Cooldown é obrigatória.');
            return false;
          }

          alt.emit('save', id,
            Number(posX),
            Number(posY),
            Number(posZ),
            parseInt(dimension),
            parseInt(onlinePoliceOfficers),
            parseInt(cooldownQuantityLimit),
            parseInt(cooldownHours));
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#posX').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', '', '', '', '', '', '', '', '', '');
});

function loaded(crackDens) {
  $('#tbody-crackdens').html(crackDens);
}

function goto(id) {
  alt.emit('goto', id);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const crackDen = JSON.parse($(`#json${id}`).val());
  addEdit(id, crackDen.PosX, crackDen.PosY, crackDen.PosZ, crackDen.Dimension,
    crackDen.OnlinePoliceOfficers, crackDen.CooldownQuantityLimit, crackDen.CooldownHours);
}

function editItems(id) {
  alt.emit('editItems', id);
}

function revokeCooldown(id) {
  alt.emit('revokeCooldown', id);
}

function mostrarMensagem(mensagem, fechar) {
  $.alert(mensagem);

  if (fechar && modal) {
    modal.close();
    modal = null;
  }
}

if ('alt' in window) {
  alt.on('loaded', loaded);
  alt.on('mostrarMensagem', mostrarMensagem);
}