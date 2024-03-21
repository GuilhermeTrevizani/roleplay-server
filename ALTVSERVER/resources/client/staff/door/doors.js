initSearch('#pesquisa', '.pesquisaitem');

let modal;
function addEdit(id, name, hash, posX, posY, posZ, factionId, companyId, locked) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Porta` : `Editar Porta ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" class="form-control" maxlength="50"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Hash</label>' +
      `<input value="${hash}" id="hash" type="text" class="form-control"/>` +
      '</div>' +
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
      '<label>Facção</label>' +
      `<input value="${(factionId ? factionId : '')}" id="factionId" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Empresa</label>' +
      `<input value="${(companyId ? companyId : '')}" id="companyId" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      `<input ${locked ? 'checked' : ''} value="true" id="locked" type="checkbox" class="form-control"/>` +
      '<label for="locked">Trancada</label>' +
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

          const hash = this.$content.find('#hash').val();
          if (!hash) {
            $.alert('Hash é obrigatória.');
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

          const factionId = this.$content.find('#factionId').val();
          const companyId = this.$content.find('#companyId').val();

          alt.emit('save',
            id,
            name,
            Number(hash),
            Number(posX),
            Number(posY),
            Number(posZ),
            factionId,
            companyId,
            this.$content.find('#locked').is(":checked")
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
      $('#name').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit('', '', '', '', '', '', '', '', false);
});

function loaded(doors) {
  $('#tbody-doors').html(doors);
}

function goto(id) {
  alt.emit('goto', id);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const door = JSON.parse($(`#json${id}`).val());
  addEdit(id, door.Name, door.Hash, door.PosX, door.PosY, door.PosZ, door.FactionId, door.CompanyId, door.Locked);
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