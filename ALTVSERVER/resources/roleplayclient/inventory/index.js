$(window).keyup((e) => {
    if (e.which === 27) {
        closeView();
    }
});

const InventoryShowType = {
    Default: 1,
    Inspect: 2,
    Property: 3,
    Vehicle: 4,
};

var contextMenuItems = [
    { 
        name: 'Entregar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');

            $.confirm({
                title: `Entregar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '<label>ID do Jogador</label>' +
                '<input id="player" type="number" class="form-control" required/>' +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if(!quantity){
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }

                            var player = this.$content.find('#player').val();
                            if(!player){
                                $.alert('O ID do jogador não foi informado.');
                                return false;
                            }
        
                            alt.emit('giveItem', Number($(target).data('id')), parseInt(quantity), parseInt(player));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
                        btnClass: 'btn-red' 
                    } 
                },
                onContentReady: function () {
                    $('#player').focus();
                    var jc = this;
                    this.$content.find('form').on('submit', function (e) {
                        e.preventDefault();
                        jc.$$formSubmit.trigger('click');
                    });
                }
            });
        }
    },
    { 
        name: 'Dropar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');
            if (itemQuantity == 1) {
                alt.emit('dropItem', Number($(target).data('id')), parseInt(itemQuantity));
                return;
            }

            $.confirm({
                title: `Dropar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if (!quantity) {
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }
        
                            alt.emit('dropItem', Number($(target).data('id')), parseInt(quantity));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
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
    },
    { 
        name: 'Descartar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');
            if (itemQuantity == 1) {
                alt.emit('discardItem', Number($(target).data('id')), parseInt(itemQuantity));
                return;
            }

            $.confirm({
                title: `Descartar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if (!quantity) {
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }
        
                            alt.emit('discardItem', Number($(target).data('id')), parseInt(quantity));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
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
    },
    { 
        name: 'Armazenar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');
            if (itemQuantity == 1) {
                alt.emit('storeItem', Number($(target).data('id')), parseInt(itemQuantity));
                return;
            }

            $.confirm({
                title: `Armazenar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if (!quantity) {
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }
        
                            alt.emit('storeItem', Number($(target).data('id')), parseInt(quantity));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
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
    },
];

var contextRightMenuItems = [
    { 
        name: 'Pegar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');
            if (itemQuantity == 1) {
                alt.emit('getItem', Number($(target).data('id')), parseInt(itemQuantity));
                return;
            }

            $.confirm({
                title: `Pegar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if (!quantity) {
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }
        
                            alt.emit('getItem', Number($(target).data('id')), parseInt(quantity));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
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
    },
];

var contextRobberyMenuItems = [
    { 
        name: 'Pegar', 
        fn: (target) => { 
            const itemQuantity = $(target).data('quantity');
            if (itemQuantity == 1) {
                alt.emit('robItem', Number($(target).data('id')), parseInt(itemQuantity));
                return;
            }

            $.confirm({
                title: `Pegar Item`,
                content: '<hr/>' +
                '<form action="">' +
                $(target).data('original-title') +
                '<hr/><div class="form-group">' +
                '<label>Quantidade</label>' +
                `<input value="${itemQuantity}" id="quantity" type="number" class="form-control" required min="1" max="${itemQuantity}"/> <br/>` +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Confirmar',
                        btnClass: 'btn-green',
                        action: function () {
                            var quantity = this.$content.find('#quantity').val();
                            if (!quantity) {
                                $.alert('A quantidade não foi informada.');
                                return false;
                            }
        
                            alt.emit('robItem', Number($(target).data('id')), parseInt(quantity));
                        }
                    },
                    cancel: { 
                        text: 'Cancelar', 
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
    },
];

function loaded(title, items, rightTitle, rightItems, type, update) {
    $('#title').html(title);
    $('#right-title').html(rightTitle);

    $(`.inv-slot`).removeClass('draggable-droppable--occupied').empty('');

    if (type >= InventoryShowType.Property)
        $(`.inv-right-slot`).removeClass('draggable-droppable--occupied').empty('');
    else
        $(`.inv-right-slot`).empty('');

    items.forEach((x) => {
        createItem(x, 'inv-icon')
    });

    rightItems.forEach((x) => {
        createItem(x, 'inv-right-icon')
    });

    $(".inv-icon, .inv-right-icon").tooltip();

    if (!update) {
        if (type != InventoryShowType.Inspect) {
            const droppable = new Draggable.Droppable(document.querySelectorAll('.inv-item'), {
                draggable: '.inv-icon',
                droppable: '.inv-slot'
            });

            droppable.on('drag:stop', (e) => {
                const id = $(e.source).data('id');
                const slot = $(e.source).parent('.inv-slot').data('slot');
                
                if('alt' in window)
                    alt.emit('moveItem', id, slot);
            });

            if (type == InventoryShowType.Property || type == InventoryShowType.Vehicle) {
                const droppable2 = new Draggable.Droppable(document.querySelectorAll('.inv-right-item'), {
                    draggable: '.inv-right-icon',
                    droppable: '.inv-right-slot'
                });
    
                droppable2.on('drag:stop', (e) => {
                    const id = $(e.source).data('id');
                    const slot = $(e.source).parent('.inv-right-slot').data('slot');
                    
                    if('alt' in window)
                        alt.emit('moveRightItem', id, slot);
                });
            }
        } 
        
        if (type != InventoryShowType.Property && type != InventoryShowType.Vehicle)
            contextMenuItems.splice(3, 1);

        new ContextMenu('.inv-icon', type == InventoryShowType.Inspect ? contextRobberyMenuItems : contextMenuItems);

        new ContextMenu('.inv-right-icon', contextRightMenuItems);
    }
}

function createItem(x, className) {
    let tooltip = `<h4>${x.Name}</h4>Quantidade: <strong>${x.Quantity.toLocaleString("pt-BR")}</strong></br>Peso: <strong>${x.Weight}</strong></br><br/>${x.Extra}`;

    $(`[data-slot="${x.Slot}"]`)
        .addClass('draggable-droppable--occupied')
        .html(`<img data-id="${x.ID}" data-quantity="${x.Quantity}" data-html="true" data-toggle="tooltip" data-placement="bottom" data-original-title="${tooltip}" src="img/${x.Image}.png" class="${className}"/>`);
}

/*loaded('Demetrius Vanwinkle [0] (10,00/30,00 kgs)', [
    {
        ID: 1,
        Name: 'Chave de Veículo',
        Slot: 2,
        Quantity: 1,
        Weight: 1,
        Extra: 'Fechadura: 12387595',
        Image: "1/3415619887",
    },
    {
        ID: 2,
        Name: 'Chave de Propriedade',
        Slot: 4,
        Quantity: 1,
        Weight: 1,
        Extra: 'Fechadura: 123321',
        Image: "1/487013001",
    },
], 'Chão', [
    {
        ID: 123,
        Name: 'Arma',
        Slot: 1001,
        Quantity: 1,
        Weight: 1,
        Extra: '',
        Image: "1/3523564046",
    }
], InventoryShowType.Default, false);*/

function closeView() {
    alt.emit('closeView');
}

function mostrarErro(erro) {
    $.alert(erro);
}

function moveItem(id, newSlot, oldSlot) {
    const item = $(`[data-id="${id}"]`);
    const divSlot = item.parent(`[data-slot="${oldSlot}"]`);
    $(divSlot).empty();
    const childs = divSlot.children('.inv-icon');
    if (childs.length == 0)
        divSlot.removeClass('draggable-droppable--occupied');

    if (newSlot != 0) {
        $(`[data-slot="${newSlot}"]`).addClass('draggable-droppable--occupied').append(item);
        $(item).tooltip();
    }
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarErro', mostrarErro);
    alt.on('moveItem', moveItem);
}