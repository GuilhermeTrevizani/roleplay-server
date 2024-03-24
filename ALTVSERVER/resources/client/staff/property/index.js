initSearch('#pesquisa', '.pesquisaitem');

let interiors;
function loadInteriors(_interiors) {
  interiors = _interiors;
}

let modal;
function addEdit(id, interior, value, dimension, posX, posY, posZ) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Propriedade` : `Editar Propriedade ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Interior</label>' +
      `<select id="interior" class="form-control"></select>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Valor</label>' +
      `<input value="${value}" id="value" type="number" class="form-control"/>` +
      '</div>' +
      '<button type="button" onclick="getPosition()" class="btn btn-sm btn-dark">Obter Posição</button><br/>' +
      '<div class="form-group">' +
      '<label>Dimensão</label>' +
      `<input value="${dimension}" id="dimension" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Posição Entrada X</label>' +
      `<input value="${posX}" id="posX" type="text" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Posição Entrada Y</label>' +
      `<input value="${posY}" id="posY" type="text" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Posição Entrada Z</label>' +
      `<input value="${posZ}" id="posZ" type="text" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const interior = this.$content.find('#interior').val();
          if (!interior) {
            $.alert('Interior é obrigatório.');
            return false;
          }

          const value = this.$content.find('#value').val();
          if (!value) {
            $.alert('Valor é obrigatório.');
            return false;
          }

          const dimension = this.$content.find('#dimension').val();
          if (!dimension) {
            $.alert('Dimensão é obrigatória.');
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

          alt.emit('save',
            id,
            parseInt(interior),
            parseInt(value),
            parseInt(dimension),
            Number(posX),
            Number(posY),
            Number(posZ));
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      interiors.forEach((x) => {
        $('#interior').append(`<option value='${x.Id}'>${x.Name}</option>`);
      });

      $('#interior').val(interior).change();
      $('#interior').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', 0, '', 0, '', '', '');
});

function loaded(properties) {
  $('#tbody-properties').html(properties);
}

function ir(id) {
  alt.emit('goto', id);
}

function excluir(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function editar(id) {
  const property = JSON.parse($(`#json${id}`).val());
  addEdit(id, property.Interior, property.Value, property.Dimension, property.EntrancePosX, property.EntrancePosY, property.EntrancePosZ);
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
  alt.on('loadInteriors', loadInteriors);
  alt.on('setPosition', setPosition);
}