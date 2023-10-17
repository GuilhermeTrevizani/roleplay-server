$(document).ready(() => {
  $("#pesquisa").on('input', function () {
    var pesquisa = removerAcentos($("#pesquisa").val());
    $.each($(".pesquisaitem"), function (index, element) {
      $(element).show();

      if (pesquisa != "") {
        if (!removerAcentos($(element).html().toLowerCase()).includes(pesquisa.toLowerCase()))
          $(element).hide();
      }
    });
  });
});

let modal;
function addEdit(id, display, dictionary, name, flag, duration, vehicle) {
  modal = $.confirm({
    title: id == 0 ? `Adicionar Animação` : `Editar Animação ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Opção</label>' +
      `<input value="${display}" id="display" type="text" class="form-control" maxlength="25"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Dicionário</label>' +
      `<input value="${dictionary}" id="dictionary" type="text" class="form-control" maxlength="100"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" class="form-control" maxlength="100"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Flag</label>' +
      `<input value="${flag}" id="flag" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Duração</label>' +
      `<input value="${duration}" id="duration" type="number" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      `<input ${vehicle ? 'checked' : ''} value="true" id="vehicle" type="checkbox" class="form-control"/>` +
      '<label for="vehicle">Somente em veículos</label>' +
      '</div>' +
      '</form>',
    buttons: {
      formSubmit: {
        text: 'Gravar',
        btnClass: 'btn-green',
        action: function () {
          const display = this.$content.find('#display').val();
          if (!display) {
            $.alert('Opção é obrigatória.');
            return false;
          }

          const dictionary = this.$content.find('#dictionary').val();
          if (!dictionary) {
            $.alert('Dicionário é obrigatório.');
            return false;
          }

          const name = this.$content.find('#name').val();
          if (!name) {
            $.alert('Nome é obrigatório.');
            return false;
          }

          const flag = this.$content.find('#flag').val();
          if (!flag) {
            $.alert('Flag é obrigatória.');
            return false;
          }

          const duration = this.$content.find('#duration').val();
          if (!duration) {
            $.alert('Duração é obrigatória.');
            return false;
          }

          alt.emit('save',
            id,
            display,
            dictionary,
            name,
            parseIntExt(flag),
            parseIntExt(duration),
            this.$content.find('#vehicle').is(":checked")
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
      $('#display').focus();
      var jc = this;
      this.$content.find('form').on('submit', function (e) {
        e.preventDefault();
        jc.$$formSubmit.trigger('click');
      });
    }
  });
}

$('#btn-add').click(() => {
  addEdit(0, '', '', '', '', -1, false);
});

function loaded(animations) {
  $('#tbody-animations').html(animations);
}

function remove(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function edit(id) {
  const animation = JSON.parse($(`#json${id}`).val());
  addEdit(id, animation.Display, animation.Dictionary, animation.Name, animation.Flag, animation.Duration, animation.Vehicle);
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