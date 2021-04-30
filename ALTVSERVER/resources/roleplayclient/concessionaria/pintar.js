let r1 = 0, g1 = 0, b1 = 0;
let r2 = 0, g2 = 0, b2 = 0;
let cor1 = '#000000';

$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function pintar() {
    alt.emit("pintar", r1, g1, b1, r2, g2, b2);
}

function updateCor1(picker) {
    r1 = parseInt(picker.rgb[0]);
    g1 = parseInt(picker.rgb[1]);
    b1 = parseInt(picker.rgb[2]);
    cor1 = picker.toHEXString();
}

function updateCor2(picker) {
    r2 = parseInt(picker.rgb[0]);
    g2 = parseInt(picker.rgb[1]);
    b2 = parseInt(picker.rgb[2]);
}

function copiarCorPrimaria() {
    r2 = r1;
    g2 = g1;
    b2 = b1;
    document.querySelector('#corsecundaria').jscolor.fromString(cor1)
}

function closeView() {
    alt.emit('closeView');
}