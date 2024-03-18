$(window).keyup((e) => {
  if (e.which === 27)
    closeView();
});

function comprarConveniencia(x) {
  const itens = JSON.parse(x);
  itens.forEach(function (p) {
    $("#tbody-itens").append(`<tr>
            <td>${p.Nome}</td> 
            <td>${p.Preco}</td> 
            <td class="text-center">
                <button class="btn btn-xs btn-primary" type="button" onclick="confirmarCompra('${p.Nome}')">Comprar</button>
            </td>
        </tr>`);
  });
}

function confirmarCompra(item) {
  alt.emit("confirmarCompra", item);
}

function closeView() {
  alt.emit('closeView');
}

if ('alt' in window)
  alt.on('comprarConveniencia', comprarConveniencia);