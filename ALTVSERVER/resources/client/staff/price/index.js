let searchTimeout;
initSearch('#pesquisa', '.pesquisaitem', searchTimeout);

let types;
function loadTypes(_types) {
  types = _types;
}

let modal;
function addEdit(id, type, name, value) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Preço` : `Editar Preço ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Tipo</label>' +
      `<select id="type" class="form-control"></select>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" class="form-control" maxlength="25"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Valor</label>' +
      `<input value="${value}" id="value" type="number" min="1" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const name = this.$content.find('#name').val();
          if (!name) {
            $.alert('Nome é obrigatório.');
            return false;
          }

          const type = this.$content.find('#type').val();
          if (!type) {
            $.alert('Tipo é obrigatório.');
            return false;
          }

          const value = this.$content.find('#value').val();
          if (!value) {
            $.alert('Valor é obrigatório.');
            return false;
          }

          alt.emit('save',
            id,
            Number(type),
            name,
            parseFloatExt(value));
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
  addEdit('', 0, '', 0);
});

function loaded(prices) {
  $('#tbody-prices').html(prices);

  if ($('#pesquisa').val() != '')
    $('#pesquisa').trigger('input');
}

function excluir(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function editar(id) {
  const price = JSON.parse($(`#json${id}`).val());
  addEdit(id, price.Type, price.Name, price.Value);
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
}