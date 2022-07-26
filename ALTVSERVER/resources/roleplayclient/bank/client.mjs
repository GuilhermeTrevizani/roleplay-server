
import * as alt from 'alt';
import * as native from 'natives';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

const atms = [ 'prop_atm_01', 'prop_atm_02', 'prop_atm_03', 'prop_fleeca_atm' ];

alt.onServer('ATMCheck', () => {
    let temAtmPerto = false;
    atms.forEach(atm => {
        if (temAtmPerto) 
            return;

        let object = native.getClosestObjectOfType(alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z, 
            1.0, alt.hash(atm), false, false, false);
        temAtmPerto = object != 0;
    });

    alt.emitServer('ATMUse', temAtmPerto);
});

alt.onServer('BankShow', (update, atm, id, name, accountAmount, savings, tickets, transactions) => {
    if (update) {
        view.emit('update', accountAmount, savings, JSON.parse(tickets), JSON.parse(transactions));
        return;
    }

    setView(new alt.WebView('http://resource/bank/index.html'));
    view.on('load', () => {
        view.emit('loaded', atm, id, name, accountAmount, savings, JSON.parse(tickets), JSON.parse(transactions));
    });
    view.on('closeView', closeView);
    view.on('deposit', (value) => {
        alt.emitServer('BankDeposit', value);
    });
    view.on('withdraw', (value) => {
        alt.emitServer('BankWithdraw', value);
    });
    view.on('transfer', (targetId, value, description, confirm) => {
        alt.emitServer('BankTransfer', targetId, value, description, confirm);
    });
    view.on('savingsDeposit', () => {
        alt.emitServer('BankSavingsDeposit');
    });
    view.on('savingsWithdraw', () => {
        alt.emitServer('BankSavingsWithdraw');
    });
    view.on('policeTicketPayment', (id) => {
        alt.emitServer('BankPoliceTicketPayment', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('BankTransferConfirm', (targetId, value, description, name) => {
    view.emit('bankTransferConfirm', targetId, value, description, name)
});