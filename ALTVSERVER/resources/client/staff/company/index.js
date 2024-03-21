initSearch('#pesquisa', '.pesquisaitem');

let modal;
function addEdit(id, name, posX, posY, posZ, weekRentValue) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Empresa` : `Editar Empresa ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" class="form-control" maxlength="50"/>` +
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
      '<label>Aluguel Semanal</label>' +
      `<input value="${weekRentValue}" id="weekRentValue" type="number" class="form-control"/>` +
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

          const weekRentValue = this.$content.find('#weekRentValue').val();
          if (!weekRentValue) {
            $.alert('Aluguel Semanal é obrigatório.');
            return false;
          }

          alt.emit('save',
            id,
            name,
            Number(posX),
            Number(posY),
            Number(posZ),
            parseIntExt(weekRentValue));
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
  addEdit('', '', '', '', '', '');
});

function loaded(companies) {
  $('#tbody-companies').html(companies);
}

function goto(id) {
  alt.emit('goto', id);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function removeOwner(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('removeOwner', id);
}

function edit(id) {
  const company = JSON.parse($(`#json${id}`).val());
  addEdit(id, company.Name, company.PosX, company.PosY, company.PosZ, company.WeekRentValue);
}

function getPosition() {
  alt.emit('getPosition');
}

function setPosition(x, y, z) {
  $('#posX').val(x);
  $('#posY').val(y);
  $('#posZ').val(z);
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
  alt.on('setPosition', setPosition);
}