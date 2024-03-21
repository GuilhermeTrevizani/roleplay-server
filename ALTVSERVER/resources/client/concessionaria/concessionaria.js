initSearch('#pesquisa', '.pesquisaveh');

function comprarVeiculo() {
  const rgb1 = hexToRgb($('#color1').val());
  const rgb2 = hexToRgb($('#color2').val());

  alt.emit("confirmarCompra",
    $("#veiculo").val(),
    Number(rgb1.r),
    Number(rgb1.g),
    Number(rgb1.b),
    Number(rgb2.r),
    Number(rgb2.g),
    Number(rgb2.b)
  );
}

function showHTML(titulo, x) {
  $('#titulo').html(titulo);
  let vehs = JSON.parse(x);
  vehs.forEach(function (p) {
    $("#tbody-veiculos").append(`<tr class="pesquisaveh"><td>${p.Exibicao}</td><td>${p.Nome}</td><td>${p.Restricao}</td><td>${p.Preco}</td></tr>`);
  });
}

function copyColor() {
  $('#color2').val($('#color1').val());
}

function copy() {
  var copyText = document.getElementById("link");
  copyText.select();
  document.execCommand("copy");
}

if ('alt' in window)
  alt.on('showHTML', showHTML);