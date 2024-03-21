initSearch('#pesquisa', '.searchRank');

let modal;
function addEdit(id, name, salary) {
  modal = $.confirm({
    title: id == '' ? `Adicionar Rank` : `Editar Rank ${id}`,
    content: '<form action="">' +
      '<div class="form-group">' +
      '<label>Nome</label>' +
      `<input value="${name}" id="name" type="text" maxlength="50" class="form-control"/>` +
      '</div>' +
      '<div class="form-group">' +
      '<label>Salário</label>' +
      `<input value="${salary}" id="salary" type="number" class="form-control"/>` +
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

          const salary = this.$content.find('#salary').val();
          if (!salary) {
            $.alert('Salário é obrigatório.');
            return false;
          }

          alt.emit('save',
            id,
            name,
            parseInt(salary));
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
  addEdit('', '', 0, '', '', '');
});

$('#btn-order').click(() => {
  let ranks = [];
  $('tr').filter('[data-id]').each((i, x) => {
    ranks.push({
      Id: parseInt($(x).data('id')),
      Position: i + 1,
    })
  });
  alt.emit('order', JSON.stringify(ranks));
});

function loaded(ranks) {
  $('#tbody-factionranks').html(ranks);

  $("#table-factionranks").tableDnD({
    onDragClass: "myDragClass"
  });
}

function editRank(id) {
  const rank = JSON.parse($(`#jsonRank${id}`).val());
  addEdit(id, rank.Name, rank.Salary);
}

function removeRank(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('remove', id);
}

function mostrarMensagem(mensagem, fechar) {
  $.alert(mensagem);

  if (fechar && modal) {
    modal.close();
    modal = null;
  }
}

function init(factionName) {
  $('#span-factionName').html(factionName);
}

if ('alt' in window) {
  alt.on('loaded', loaded);
  alt.on('mostrarMensagem', mostrarMensagem);
  alt.on('init', init);
}