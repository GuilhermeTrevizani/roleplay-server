initSearch('#pesquisa', '.pesquisaitem');

let modal;
function addEdit(id, item, value) {
  modal = $.confirm({
    title: id == 0 ? `Adicionar Item` : `Editar Item ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Item</label>' +
      `<input value="${item}" id="item" type="text" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Valor</label>' +
      `<input value="${value}" id="value" type="numner" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const item = this.$content.find('#item').val();
          if (!item) {
            $.alert('Item é obrigatório.');
            return false;
          }

          const value = this.$content.find('#value').val();
          if (!value) {
            $.alert('Valor é obrigatório.');
            return false;
          }

          alt.emit('save',
            id,
            item,
            parseInt(value)
          );
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#item').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', '', '');
});

function loaded(crackDensItems) {
  $('#tbody-crackdensitems').html(crackDensItems);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const crackDenItem = JSON.parse($(`#json${id}`).val());
  addEdit(id, crackDenItem.ItemCategory, crackDenItem.Value);
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