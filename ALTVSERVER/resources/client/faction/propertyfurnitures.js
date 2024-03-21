initSearch('#pesquisa', '.pesquisaitem');

$('#btn-buy').click(() => {
  alt.emit('buy');
});

function showHTML(propertyId, furnitures) {
  $('#h3-title').html(`Propriedade Nº ${propertyId}`);
  $('#tbody-furnitures').html(furnitures);
}

function edit(button, id) {
  $(button).LoadingOverlay('show');
  alt.emit('edit', id);
}

function remove(button, id) {
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

if ('alt' in window) {
  alt.on('showHTML', showHTML);
  alt.on('mostrarMensagem', mostrarMensagem);
}