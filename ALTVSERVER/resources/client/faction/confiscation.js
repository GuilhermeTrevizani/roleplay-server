let items = [];

$('#btn-confirm').click(() => {
  const characterName = $('#input-characterName').val();
  if (!characterName) {
    $.alert('Nome do personagem é obrigatório');
    return;
  }

  $(`#btn-confirm`).LoadingOverlay('show');
  alt.emit('save',
    $('#input-characterName').val(),
    JSON.stringify(items));
});

$('#btn-addItem').click(() => {
  const id = parseInt($('#select-item').val());
  const inventoryItem = inventoryItems.find(x => x.Id == id);
  const name = inventoryItem.Name.substring(inventoryItem.Name.indexOf("x") + 1);

  const quantity = parseInt($('#input-quantity').val());
  if (isNaN(quantity) || quantity <= 0 || quantity > inventoryItem.Quantity) {
    $.alert(`Quantidade deve ser entre 1 e ${inventoryItem.Quantity}.`);
    return;
  }

  let item = items.find(x => x.Id == id);
  if (item == null) {
    item = {
      Id: id,
      Quantity: quantity,
      Name: name,
    };
    items.push(item);
  } else {
    item.Quantity = quantity;
  }

  $('#span-items').html('');
  items.forEach((x) => {
    $('#span-items').append(`<span>${x.Quantity}x ${x.Name}</span><br/>`);
  });
});

$('#select-item').change(() => {
  $('#input-quantity').val('');
});

let inventoryItems = [];
function loaded(items) {
  inventoryItems = items;
  items.forEach((x) => {
    $('#select-item').append(`<option value='${x.Id}'>${x.Name}</option>`);
  });
}

function mostrarMensagem(message) {
  $(`#btn-confirm`).LoadingOverlay('hide');
  $.alert(message);
}

if ('alt' in window) {
  alt.on('loaded', loaded);
  alt.on('mostrarMensagem', mostrarMensagem);
}