let modal;
function addEdit(id, color, blipType, blipColor) {
  modal = $.confirm({
    title: `Editar Empresa ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Cor</label>' +
      `<input value="#${color}" id="color" type="color" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Tipo do Blip</label>' +
      `<input value="${blipType}" id="blipType" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Cor do Blip</label>' +
      `<input value="${blipColor}" id="blipColor" type="number" class="form-control"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const color = this.$content.find('#color').val().replace('#', '');
          if (!color) {
            $.alert('Cor é obrigatória.');
            return false;
          }

          const blipType = this.$content.find('#blipType').val();
          if (!blipType) {
            $.alert('Tipo do Blip é obrigatório.');
            return false;
          }

          const blipColor = this.$content.find('#blipColor').val();
          if (!blipColor) {
            $.alert('Cor do Blip é obrigatória.');
            return false;
          }

          alt.emit('save',
            id,
            color,
            parseIntExt(blipType),
            parseIntExt(blipColor)
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
      $('#color').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

function loaded(companies) {
  $('#tbody-companies').html(companies);
}

function edit(id) {
  const company = JSON.parse($(`#json${id}`).val());
  addEdit(id, company.Color, company.BlipType, company.BlipColor);
}

function employees(id) {
  alt.emit('employees', id);
}

function openClose(id) {
  alt.emit('openClose', id);
}

function announce(id) {
  modal = $.confirm({
    title: 'Fazer Anúncio',
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Mensagem</label>' +
      `<input id="message" type="text" class="form-control" maxlength="200"/>` +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const message = this.$content.find('#message').val();
          if (!message) {
            $.alert('Mensagem é obrigatória.');
            return false;
          }

          alt.emit('announce', id, message);
          return false;
        }
      },
      cancel: {
        text: 'Fechar',
        btnClass: 'btn-red'
      }
    },
    onContentReady: function () {
      $('#message').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

function mostrarMensagem(message, fechar) {
  if (message)
    $.alert(message);

  if (fechar && modal) {
    modal.close();
    modal = null;
  }
}

if ('alt' in window) {
  alt.on('loaded', loaded);
  alt.on('mostrarMensagem', mostrarMensagem);
}