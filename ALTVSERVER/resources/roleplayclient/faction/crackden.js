if('alt' in window) {
    alt.on('loaded', (itemsHtml) => {
        $('#tbody-items').html(itemsHtml);
    });

    alt.on('mostrarMensagem', (message, sucesso) => {
        if (sucesso)
            toastr.success(message);
        else
            $.alert(message);
    });
}

$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$('#btn-showSells').click(() => {
    alt.emit('showSells');
});

function sell(item) {
    $.confirm({
        title: `Vender Droga`,
        content: '<form action="">' +
        '<div class="form-group">' +
        '<label>Quantidade</label>' +
        `<input id="quantity" type="number" class="form-control"/>` +
        '</div>' +
        '</form>',
        buttons: {
            formSubmit: {
                text: 'Gravar',
                btnClass: 'btn-green',
                action: function () {
                    const quantity = this.$content.find('#quantity').val();
                    if (!quantity) {
                        $.alert('Quantidade é obrigatória.');
                        return false;
                    }
                    
                    alt.emit('sellItem', parseInt(item), parseInt(quantity));
                }
            },
            cancel: { 
                text: 'Fechar', 
                btnClass: 'btn-red' 
            } 
        },
        onContentReady: function () {
            $('#quantity').focus();
            var jc = this;
            this.$content.find('form').on('submit', function (e) {
                e.preventDefault();
                jc.$$formSubmit.trigger('click');
            });
        }
    });
}

function closeView() {
    alt.emit('closeView');
}