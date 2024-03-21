initSearch('#pesquisa', '.pesquisaitem');

let modal;

function addEdit(id, livery, color1R, color1G, color1B, color2R, color2G, color2B) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Veículo` : `Editar Veículo ${id}`,
    content: '<form action="">' +
      (id == '' ?
        '<div class="form-group">' +
        '<label>Tipo</label>' +
        `<select id="type" class="form-control">
            <option value='1'>Facção</option>
            <option value='2'>Emprego</option>
            <option value='3'>Personagem (Benefício)</option>
        </select>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>ID (Facção, Emprego ou Personagem)</label>' +
        `<input id="typeId" type="text" class="form-control"/>` +
        '</div>' +
        '<div class="form-group">' +
        '<label>Modelo</label>' +
        `<input id="model" type="text" class="form-control"/>` +
        '</div>'
        :
        '') +
      '<div class="form-group">' +
      '<label>Livery</label>' +
      `<input value="${livery}" id="livery" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Cor Primária</label><br/>' +
      `<input type="color" id="color1" value="#000000" />` +
      '</div>' +
      '<button type="button" id="btn-copyColor" class="btn btn-dark">Copiar Cor</button><br/><br/>' +
      '<div class="form-group">' +
      '<label>Cor Secundária</label><br/>' +
      `<input type="color" id="color2" value="#000000" />` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const type = this.$content.find('#type').val();
          const typeId = this.$content.find('#typeId').val();
          const model = this.$content.find('#model').val();

          if (id == 0) {
            if (!type) {
              $.alert('Tipo é obrigatório.');
              return false;
            }

            if (!typeId) {
              $.alert('ID é obrigatório.');
              return false;
            }

            if (!model) {
              $.alert('Modelo é obrigatório.');
              return false;
            }
          }

          const livery = this.$content.find('#livery').val();
          const rgb1 = hexToRgb(this.$content.find('#color1').val());
          const rgb2 = hexToRgb(this.$content.find('#color2').val());

          alt.emit('save',
            id,
            model,
            Number(type),
            typeId,
            Number(livery),
            Number(rgb1.r),
            Number(rgb1.g),
            Number(rgb1.b),
            Number(rgb2.r),
            Number(rgb2.g),
            Number(rgb2.b));
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#type').focus();

      this.$content.find('#color1').val(rgbToHex(color1R, color1G, color1B));
      this.$content.find('#color2').val(rgbToHex(color2R, color2G, color2B));

      $('#btn-copyColor').click(() => {
        this.$content.find('#color2').val(this.$content.find('#color1').val());
      });

      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', 1, 0, 0, 0, 0, 0, 0);
});

function loaded(vehicles) {
  $('#tbody-vehicles').html(vehicles);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const veh = JSON.parse($(`#json${id}`).val());
  addEdit(id, veh.Livery, veh.Color1R, veh.Color1G, veh.Color1B, veh.Color2R, veh.Color2G, veh.Color2B);
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