function pintar() {
  const rgb1 = hexToRgb($('#color1').val());
  const rgb2 = hexToRgb($('#color2').val());

  alt.emit("pintar",
    Number(rgb1.r),
    Number(rgb1.g),
    Number(rgb1.b),
    Number(rgb2.r),
    Number(rgb2.g),
    Number(rgb2.b)
  );
}

function copyColor() {
  $('#color2').val($('#color1').val());
}