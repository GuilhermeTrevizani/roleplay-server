import * as alt from 'alt-client'
import { showCursor } from '/helpers/cursor.js';

let chatActive = false;
let inputActive = false;
let scrollActive = false;
let canInput = false;

let webview = new alt.WebView('http://resource/chat/html/index.html');
webview.focus();

webview.on('chat:onInputStateChange', state => {
    inputActive = state;

    alt.Player.local.setMeta('chatting', state);
    alt.emitServer('Chatting', state);
    showCursor(state);
    alt.toggleGameControls(!state);
});

webview.on('chat:onChatStateChange', state => {
    chatActive = state;
});

webview.on('chat:onInput', (text) => {
    alt.emitServer('OnPlayerChat', text);
});

alt.onServer('chat:sendMessage', (text, color, icon) => {
    push(text, color, icon);
});

alt.onServer('chat:activateChat', state => {
    activateChat(state);
});

alt.onServer('chat:activateTimeStamp', state => {
    webview.emit('chat:activateTimeStamp', state);
});

alt.onServer('chat:notify', (text, type) => {
    webview.emit('chat:notify', text, type);
});

alt.on('chat:notify', (text, type) => {
    webview.emit('chat:notify', text, type);
});

alt.onServer('chat:toggleTelaPreta', () => {
    webview.emit('chat:toggleTelaPreta');
});

alt.onServer('chat:toggleTelaCinza', () => {
    webview.emit('chat:toggleTelaCinza');
});

alt.onServer('chat:toggleTelaLaranja', () => {
    webview.emit('chat:toggleTelaLaranja');
});

alt.onServer('chat:toggleTelaVerde', () => {
    webview.emit('chat:toggleTelaVerde');
});

alt.onServer('chat:ExibirConfirmacao', (titulo, descricao, funcao) => {
    toggleView(true);
    webview.emit('chat:exibirConfirmacao', titulo, descricao, funcao);
});

alt.onServer('chat:clearMessages', clearMessages);

webview.on('ConfirmarExibirConfirmacao', (funcao) => {
    toggleView(false);
    alt.emitServer(funcao);
});

webview.on('CancelarExibirConfirmacao', () => {
    toggleView(false);
});

function toggleView(show) {
    showCursor(show);
    alt.toggleGameControls(!show);
    toggleInput(!show);
}

export function clearMessages() {
    webview.emit('chat:clearMessages');
}

export function push(text, color = 'white', gradient = false, icon = false) {
    webview.emit('chat:pushMessage', text, color, gradient, icon);
}

export function activateChat(state) {
    webview.emit('chat:activateChat', state);
}

export function toggleInput(state) {
    canInput = state;
}

function activateInput(state) {
    webview.emit('chat:activateInput', state);
}

alt.on('keyup', key => {
    if (alt.isMenuOpen())
        return;

    // Keys working only when chat is not active
    if (!chatActive) {
        switch (key) {
        }
    }

    // Keys working only when chat is active
    if (chatActive) {
        switch (key) {
            // PageUp
            case 33: return scrollMessagesList('up');
            // PageDown
            case 34: return scrollMessagesList('down');
        }
    }

    // Keys working only when chat is active and input is not active
    if (chatActive && !inputActive && canInput) {
        switch (key) {
            // KeyT
            case 84: return activateInput(true);
        }
    }

    // Keys working only when chat is active and input is active
    if (chatActive && inputActive) {
        switch (key) {
            // Enter
            case 13: return sendInput();
            // ArrowUp
            case 38: return shiftHistoryUp();
            // ArrowDown
            case 40: return shiftHistoryDown();
            // Esc
            case 27: return activateInput(false);
        }
    }
});

function scrollMessagesList(direction) {
    if (scrollActive) return;
    scrollActive = true;
    alt.setTimeout(() => scrollActive = false, 250 + 5);
    webview.emit('chat:scrollMessagesList', direction);
}
function sendInput() {
    webview.emit('chat:sendInput');
}

function shiftHistoryUp() {
    webview.emit('chat:shiftHistoryUp');
}

function shiftHistoryDown() {
    webview.emit('chat:shiftHistoryDown');
}