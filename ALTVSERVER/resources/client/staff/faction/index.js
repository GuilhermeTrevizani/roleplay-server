initSearch('#pesquisa', '.pesquisaitem');

let types;
function loadTypes(_types) {
  types = _types;
}

let modal;
function addEdit(id, name, type, color, slots, chatColor) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Facção` : `Editar Facção ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" maxlength="50" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Tipo</label>' +
      `<select id="type" class="form-control"></select>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Cor</label>' +
      `<input value="${color}" id="color" type="text" maxlength="6" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Slots</label>' +
      `<input value="${slots}" id="slots" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Cor do Chat</label>' +
      `<input value="${chatColor}" id="chatColor" type="text" maxlength="6" class="form-control"/>` +
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

          const color = this.$content.find('#color').val();
          if (!color) {
            $.alert('Cor é obrigatória.');
            return false;
          }

          const slots = this.$content.find('#slots').val();
          if (!slots) {
            $.alert('Slots é obrigatório.');
            return false;
          }

          const chatColor = this.$content.find('#chatColor').val();
          if (!chatColor) {
            $.alert('Cor do Chat é obrigatória.');
            return false;
          }


          alt.emit('save',
            id,
            name,
            parseInt(type),
            color,
            parseInt(slots),
            chatColor);
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
  addEdit('', '', 0, '', '', '');
});

function loaded(factions) {
  $('#tbody-factions').html(factions);
}

function editar(id) {
  const faction = JSON.parse($(`#json${id}`).val());
  addEdit(id, faction.Name, faction.Type, faction.Color, faction.Slots, faction.ChatColor);
}

function ranks(id) {
  alt.emit('ranks', id);
}

function members(id) {
  alt.emit('members', id);
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