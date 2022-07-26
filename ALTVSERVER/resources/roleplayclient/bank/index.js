$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

$(document).ready(() => {
    $('#btn-deposit').click(() => {
        $.confirm({
            title: `Depositar`,
            content:
            '<form action="">' +
            '<div class="form-group">' +
            '<label>Valor</label>' +
            `<input id="value" type="number" class="form-control"/>` +
            '</div>' +
            '</form>',
            buttons: {
                formSubmit: {
                    text: 'Depositar',
                    btnClass: 'btn-green',
                    action: function () {
                        var value = this.$content.find('#value').val();
                        if (!value) {
                            $.alert('O valor não foi informado.');
                            return false;
                        }
    
                        alt.emit('deposit', parseInt(value));
                    }
                },
                cancel: { 
                    text: 'Cancelar', 
                    btnClass: 'btn-red' 
                } 
            },
            onContentReady: function () {
                $('#value').focus();
                var jc = this;
                this.$content.find('form').on('submit', function (e) {
                    e.preventDefault();
                    jc.$$formSubmit.trigger('click');
                });
            }
        });
    });
    
    $('#btn-withdraw').click(() => {
        $.confirm({
            title: `Sacar`,
            content:
            '<form action="">' +
            '<div class="form-group">' +
            '<label>Valor</label>' +
            `<input id="value" type="number" class="form-control"/>` +
            '</div>' +
            '</form>',
            buttons: {
                formSubmit: {
                    text: 'Sacar',
                    btnClass: 'btn-green',
                    action: function () {
                        var value = this.$content.find('#value').val();
                        if (!value) {
                            $.alert('O valor não foi informado.');
                            return false;
                        }
    
                        alt.emit('withdraw', parseInt(value));
                    }
                },
                cancel: { 
                    text: 'Cancelar', 
                    btnClass: 'btn-red' 
                } 
            },
            onContentReady: function () {
                $('#value').focus();
                var jc = this;
                this.$content.find('form').on('submit', function (e) {
                    e.preventDefault();
                    jc.$$formSubmit.trigger('click');
                });
            }
        });
    });
    
    $('#btn-transfer').click(() => {
        $.confirm({
            title: `Transferir`,
            content:
            '<form action="">' +
            '<div class="form-group">' +
            '<label>Conta Bancária</label>' +
            `<input id="targetId" type="number" class="form-control"/>` +
            '<label>Valor</label>' +
            `<input id="value" type="number" class="form-control"/>` +
            '<label>Descrição</label>' +
            `<input id="description" type="text" class="form-control"/>` +
            '</div>' +
            '</form>',
            buttons: {
                formSubmit: {
                    text: 'Transferir',
                    btnClass: 'btn-green',
                    action: function () {
                        let targetId = this.$content.find('#targetId').val();
                        if (!targetId) {
                            $.alert('A conta bancária não foi informada.');
                            return false;
                        }

                        let value = this.$content.find('#value').val();
                        if (!value) {
                            $.alert('O valor não foi informado.');
                            return false;
                        }

                        let description = this.$content.find('#description').val();
                        if (!description) {
                            $.alert('A descrição não foi informada.');
                            return false;
                        }
    
                        alt.emit('transfer', parseInt(targetId), parseInt(value), description);
                    }
                },
                cancel: { 
                    text: 'Cancelar', 
                    btnClass: 'btn-red' 
                } 
            },
            onContentReady: function () {
                $('#targetId').focus();
                var jc = this;
                this.$content.find('form').on('submit', function (e) {
                    e.preventDefault();
                    jc.$$formSubmit.trigger('click');
                });
            }
        });
    });
    
    $('#btn-savings').click(() => {
        let title = savings == 0 ? 'Depositar na Poupança' : 'Sacar da Poupança';
        $.confirm({
            title: title,
            content: `Confirma ${title.toLowerCase()}?`,
            buttons: {
                confirm: {
                    text: 'Sim', 
                    btnClass: 'btn-green',
                    action: () => {
                        alt.emit(savings == 0 ? 'savingsDeposit' : 'savingsWithdraw');
                    }
                },
                cancel: { 
                    text: 'Não', 
                    btnClass: 'btn-red' ,
                    action: () => {
                    }
                } 
            }
        });
    });
});

let atm = false;
let savings = 0;
function loaded(_atm, id, name, accountAmount, _savings, tickets, transactions) {
    atm = _atm;
    $('#accountId').html(id);
    $('#name').html(name);
    update(accountAmount, _savings, tickets, transactions);
}

function update(accountAmount, _savings, tickets, transactions) {
    savings = _savings;

    if (atm) {
        $('#btn-deposit').hide();
        $('#btn-transfer').hide();
        $('#btn-savings').hide();
    } else {
        $('#btn-savings').html(savings == 0 ? 'Depositar Poupança' : 'Sacar Poupança');
    }

    $('#accountAmount').html(`$${accountAmount.toLocaleString("pt-BR")}`);
    $('#savings').html(`$${savings.toLocaleString("pt-BR")}`);

    if (accountAmount < 0)
        $('#accountAmount').attr('class', 'text-danger');
    else if (accountAmount > 0)
        $('#accountAmount').attr('class', 'text-success');
    else
        $('#accountAmount').attr('class', 'text-dark');

    $('#tbody-tickets').html('');

    if (tickets.length == 0) {
        $('#badge-tickets').hide();
        $('#tbody-tickets').html('<tr><td class="text-center" colspan="5">Você não possui multas pendentes.</td></tr>');
    } else {
        $('#badge-tickets').show();
        $('#badge-tickets').html(tickets.length);

        tickets.forEach((p) => {
            $('#tbody-tickets').append(`<tr>
                <td>${p.Id}</td> 
                <td>${p.Date}</td> 
                <td>${p.Value}</td> 
                <td>${p.Reason}</td> 
                <td class="text-center">
                    <button class="btn btn-sm btn-dark" type="button" onclick="policeTicketPayment(${p.Id})">Pagar</button>
                </td>
            </tr>`);
        });
    }

    if (transactions.length == 0) {
        $('#tbody-transactions').html('<tr><td class="text-center" colspan="4">Sua conta bancária não possui movimentações.</td></tr>');
    } else {
        $('#tbody-transactions').html('');
        transactions.forEach((p) => {
            $('#tbody-transactions').append(`<tr>
                <td>${p.Id}</td> 
                <td>${p.Date}</td> 
                <td class="text-${(p.Type == 1 ? 'success' : 'danger')}">${(p.Type == 1 ? '+' : '-')} ${p.Value.toLocaleString("pt-BR")}</td> 
                <td>${p.Description}</td>
            </tr>`);
        });
    }
}

function closeView() {
    alt.emit('closeView');
}

function policeTicketPayment(id) {
    alt.emit("policeTicketPayment", id);
}

function bankTransferConfirm(targetId, value, description, name) {
    $.confirm({
        title: 'Transferir',
        content: `Confirma transferir ${value} para ${name}?`,
        buttons: {
            confirm: {
                text: 'Sim', 
                btnClass: 'btn-green',
                action: () => {
                    alt.emit('transfer', targetId, value, description, true);
                }
            },
            cancel: { 
                text: 'Não', 
                btnClass: 'btn-red' ,
                action: () => {
                }
            } 
        }
    });
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('update', update);
    alt.on('bankTransferConfirm', bankTransferConfirm);
}