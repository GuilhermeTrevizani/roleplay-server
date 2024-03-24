initSearch('#pesquisa', '.pesquisaitem');

let modal;
function addEdit(id, factionId, posX, posY, posZ, dimension) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Arsenal` : `Editar Arsenal ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>ID da Facção</label>' +
      `<input value="${factionId}" id="factionId" type="text" class="form-control"/>` +
      '</div>' +
      '<button type="button" onclick="getPosition()" class="btn btn-sm btn-dark">Obter Posição</button><br/>' +
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
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const factionId = this.$content.find('#factionId').val();
          if (!factionId) {
            $.alert('ID da Facção é obrigatório.');
            return false;
          }

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
            $.alert('Dimensão é obrigatória.');
            return false;
          }

          alt.emit('save', id,
            factionId,
            Number(posX),
            Number(posY),
            Number(posZ),
            parseInt(dimension));
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#factionId').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', '', '', '', '', '', '', '');
});

function loaded(factionsArmories) {
  $('#tbody-factionsstorages').html(factionsArmories);
}

function goto(id) {
  alt.emit('goto', id);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const factionStorage = JSON.parse($(`#json${id}`).val());
  addEdit(id, factionStorage.FactionId, factionStorage.PosX, factionStorage.PosY, factionStorage.PosZ, factionStorage.Dimension);
}

function editWeapons(id) {
  alt.emit('editWeapons', id);
}

function mostrarMensagem(mensagem, fechar) {
  $.alert(mensagem);

  if (fechar && modal) {
    modal.close();
    modal = null;
  }
}

function getPosition() {
  alt.emit('getPosition');
}

function setPosition(x, y, z, dimension) {
  $('#posX').val(x);
  $('#posY').val(y);
  $('#posZ').val(z);
  $('#dimension').val(dimension);
}

if ('alt' in window) {
  alt.on('loaded', loaded);
  alt.on('mostrarMensagem', mostrarMensagem);
  alt.on('setPosition', setPosition);
}