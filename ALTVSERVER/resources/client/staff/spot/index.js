initSearch('#pesquisa', '.pesquisaitem');

let types;
function loadTypes(_types) {
  types = _types;
}

let modal;
function addEdit(id, type, posX, posY, posZ, auxiliarPosX, auxiliarPosY, auxiliarPosZ) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Ponto` : `Editar Ponto ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Tipo</label>' +
      `<select id="type" class="form-control"></select>` +
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
      '<button type="button" onclick="getAuxiliarPosition()" class="btn btn-sm btn-dark">Obter Posição</button><br/>' +
      '<div class="form-group">' +
      '<label>Posição Auxiliar X</label>' +
      `<input value="${auxiliarPosX}" id="auxiliarPosX" type="text" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Posição Auxiliar Y</label>' +
      `<input value="${auxiliarPosY}" id="auxiliarPosY" type="text" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Posição Auxiliar Z</label>' +
      `<input value="${auxiliarPosZ}" id="auxiliarPosZ" type="text" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const type = this.$content.find('#type').val();
          if (!type) {
            $.alert('Tipo é obrigatório.');
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

          const auxiliarPosX = this.$content.find('#auxiliarPosX').val();
          const auxiliarPosY = this.$content.find('#auxiliarPosY').val();
          const auxiliarPosZ = this.$content.find('#auxiliarPosZ').val();

          alt.emit('save',
            id,
            parseInt(type),
            Number(posX),
            Number(posY),
            Number(posZ),
            Number(auxiliarPosX),
            Number(auxiliarPosY),
            Number(auxiliarPosZ));
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      types.forEach((x) => {
        $('#type').append(`<option value='${x.Id}'>${x.Name}</option>`);
      });

      $('#type').val(type).change();
      $('#type').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', 0, '', '', '', '', '', '');
});

function loaded(spots) {
  $('#tbody-spots').html(spots);
}

function ir(id) {
  alt.emit('goto', id);
}

function excluir(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function editar(id) {
  const spot = JSON.parse($(`#json${id}`).val());
  addEdit(id, spot.Type, spot.PosX, spot.PosY, spot.PosZ, spot.AuxiliarPosX, spot.AuxiliarPosY, spot.AuxiliarPosZ);
}

function getPosition() {
  alt.emit('getPosition');
}

function setPosition(x, y, z) {
  $('#posX').val(x);
  $('#posY').val(y);
  $('#posZ').val(z);
}

function getAuxiliarPosition() {
  alt.emit('getAuxiliarPosition');
}

function setAuxiliarPosition(x, y, z) {
  $('#auxiliarPosX').val(x);
  $('#auxiliarPosY').val(y);
  $('#auxiliarPosZ').val(z);
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
  alt.on('loadTypes', loadTypes);
  alt.on('setPosition', setPosition);
  alt.on('setAuxiliarPosition', setAuxiliarPosition);
}