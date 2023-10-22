$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function closeView() {
    alt.emit('closeView');
}

function removerAcentos(s) {
    return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}

function componentToHex(c) {
    var hex = c.toString(16);
    return hex.length == 1 ? "0" + hex : hex;
}
  
function rgbToHex(r, g, b) {
    return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
}

function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

if (typeof toastr !== 'undefined')
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "3500",
        "hideDuration": "3500",
        "timeOut": "3500",
        "extendedTimeOut": "3500",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

function parseIntExt(valor) {
    if (isNaN(parseInt(valor)) || valor === undefined || valor === null || valor === '')
        return 0;

    return parseInt(valor);
}

function initSearch(inputId, className, timeout) {
    $(inputId).on('input', function () {
        if (timeout) {
            clearTimeout(timeout);
            timeout = null;
        }

        timeout = setTimeout(() => {
            const search = removerAcentos($(inputId).val());
            $.each($(className), (index, element) => {
                $(element).show();
    
                if (search != "") {
                    if (!removerAcentos($(element).html().toLowerCase()).includes(search.toLowerCase()))
                        $(element).hide();
                }

            });
            timeout = null;
        }, 500);
    });
}

function parseFloatExt(valor) {
    if (isNaN(parseFloat(valor)) || valor === undefined || valor === null || valor === '')
        return 0;

    return parseFloat(valor);
}