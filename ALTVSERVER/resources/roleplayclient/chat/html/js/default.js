// Settings
var _MAX_MESSAGES_ON_CHAT = 10;
var _HIDE_INPUT_BAR_ON_BLUR = true;
var _MAX_INPUT_HISTORIES = 5;

// Data
var chatActive = false;
var inputActive = false;
var timeStampActive = true;

// Input History
var inputHistory = [];
var inputHistoryCurrent;
var inputHistoryCache;

// Elements
var chatBox = $('.chat-box');
var chatMessagesList = $('.chat-box .chat-messages-list');
var chatInputBar = $('.chat-box .chat-input-bar');
var chatInputBarLength = $('.chat-box .chat-input-bar-length');
var chatNewMessagesWarning = $('.chat-box .chat-new-messages-warning');

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

// Initiation
$(document).ready(() => {
    const messagesListHeight = _MAX_MESSAGES_ON_CHAT * 22;
    const chatBoxHeight = messagesListHeight + 50;

    chatBox.css('height', chatBoxHeight + 'px');
    chatMessagesList.css('height', messagesListHeight + 'px');

    alt.emit('chat:onLoaded');
});

$(document).keydown(function(e) {
    if (e.which == 32 && !inputActive) {
        return false;
    }
});

if (_HIDE_INPUT_BAR_ON_BLUR) $(chatInputBar).focusout(() => inputActive && activateInput(false));
chatMessagesList.bind('mousewheel DOMMouseScroll', (e) => e.preventDefault());
chatInputBar.bind('propertychange change click keyup input paste', () => inputActive && setInputBarLengthCounterCurrent(chatInputBar.val().length));

function clearMessages() {
    chatMessagesList.html('');
}

// Functions - Actions
function pushMessage(text, color = '#FFFFFF', icon = "") {
    if (text.length < 1) return;

    let style = `color:${color};`

    if (icon != "")
        text = `<i class="fi-${icon}" style="padding:0 2px 0 2px"></i> ${text}`;

    let data = new Date().toTimeString().replace(/.*(\d{2}:\d{2}:\d{2}).*/, "$1");

    chatMessagesList.append(`<div class="chat-message stroke" style="${style}"><span class='timestamp' style='display:${(timeStampActive ? 'inline' : 'none')};'>[${data}] </span>${text}</div>`);

    // Check if player's chat is scrolled all the way to the bottom. If true, then scroll down for new message to appear,
    // if false, inform player about new message(s).
    (getScrolledUpMessagesAmount() >= 4) ? toggleWarningText(true) : scrollMessagesList('bottom');
}

function scrollMessagesList(direction) {
    const pixels = 22 * 5;

    switch (direction) {
        case 'up':
            chatMessagesList.stop().animate({ scrollTop: `-=${pixels}px` }, 250);
            break;
        case 'down':
            chatMessagesList.stop().animate({ scrollTop: `+=${pixels}px` }, 250);
            break;
        case 'bottom':
            chatMessagesList.stop().animate({ scrollTop: chatMessagesList.prop('scrollHeight') }, 250);
            break;
    }

    setTimeout(() => {
        if (getScrolledUpMessagesAmount() == 0) toggleWarningText(false);
    }, 250);
}

function activateChat(state) {
    chatActive = state;
    (state) ? chatBox.removeClass('hide') : chatBox.addClass('hide');

    alt.emit('chat:onChatStateChange', state);
}

function activateTimeStamp(state) {
    timeStampActive = state;
    (state) ? $('.timestamp').css('display', 'inline') : $('.timestamp').css('display', 'none');
}

function activateInput(state) {
    inputActive = state;

    // Restart Input Bar Length Counter
    setInputBarLengthCounterCurrent(0);

    // Restart Input History
    inputHistoryCache = '';
    inputHistoryCurrent = inputHistory.length;

    switch (state) {
        case true:
            chatInputBarLength.removeClass('hide');
            chatInputBar.removeClass('hide');
            chatInputBar.focus();
            break;
        case false:
            chatInputBarLength.addClass('hide');
            chatInputBar.addClass('hide');
            chatInputBar.blur();
            break;
    }

    alt.emit('chat:onInputStateChange', state);
}

function sendInput() {
    const length = chatInputBar.val().length;
    if (length > 0) {
        alt.emit('chat:onInput', chatInputBar.val());
        addInputToHistory(chatInputBar.val());
    }
    activateInput(false);
    chatInputBar.val('');
    scrollMessagesList('bottom');
}

function addInputToHistory(input) {
    // If history list have max amount of inputs, start deleting them from the beginning
    if (inputHistory.length >= _MAX_INPUT_HISTORIES) inputHistory.shift();

    // Add input to history list
    inputHistory.push(input);
}

function shiftHistoryUp() {
    let current = inputHistoryCurrent;
    if (inputHistoryCurrent == inputHistory.length) inputHistoryCache = chatInputBar.val();

    if (current > 0) {
        inputHistoryCurrent--;
        chatInputBar.val(inputHistory[inputHistoryCurrent]);
    }
}

function shiftHistoryDown() {
    if (inputHistoryCurrent == inputHistory.length) return;
    if (inputHistoryCurrent < inputHistory.length - 1) {
        inputHistoryCurrent++;
        chatInputBar.val(inputHistory[inputHistoryCurrent]);
    } else {
        inputHistoryCurrent = inputHistory.length;
        chatInputBar.val(inputHistoryCache);
    }
}

function toggleWarningText(state) {
    switch (state) {
        case true:
            chatNewMessagesWarning.removeClass('hide');
            break;
        case false:
            chatNewMessagesWarning.addClass('hide');
            break;
    }
}

function setInputBarLengthCounterCurrent(amount) {
    chatInputBarLength.html(`<i class="fi-pencil" style="padding-right:2px"></i> ${amount}/150`);
}

function getScrolledUpMessagesAmount() {
    const amount = Math.round((chatMessagesList.prop('scrollHeight') - chatMessagesList.scrollTop() - _MAX_MESSAGES_ON_CHAT * 22) / 22);
    return (amount > 0) ? amount : 0;
}

function notify(text, type) {
    if (type == 'success')
        toastr.success(text);
    else if (type == 'danger')
        toastr.error(text);
    else if (type == 'info')
        toastr.info(text);
}

function toggleTelaPreta() {
    $('body').removeClass('telacinza');
    $('body').removeClass('telalaranja');
    $('body').removeClass('telaverde');
    ($('body').hasClass('telapreta')) ? $('body').removeClass('telapreta') : $('body').addClass('telapreta');
}

function toggleTelaCinza() {
    $('body').removeClass('telapreta');
    $('body').removeClass('telalaranja');
    $('body').removeClass('telaverde');
    ($('body').hasClass('telacinza')) ? $('body').removeClass('telacinza') : $('body').addClass('telacinza');
}

function toggleTelaLaranja() {
    $('body').removeClass('telacinza');
    $('body').removeClass('telapreta');
    $('body').removeClass('telaverde');
    ($('body').hasClass('telalaranja')) ? $('body').removeClass('telalaranja') : $('body').addClass('telalaranja');
}

function toggleTelaVerde() {
    $('body').removeClass('telacinza');
    $('body').removeClass('telalaranja');
    $('body').removeClass('telapreta');
    ($('body').hasClass('telaverde')) ? $('body').removeClass('telaverde') : $('body').addClass('telaverde');
}

function exibirConfirmacao(titulo, descricao, funcao) {
    $.confirm({
        title: titulo,
        content: descricao,
        buttons: {
            confirm: {
                text: 'Sim', 
                btnClass: 'btn-green',
                action: () => {
                    alt.emit('ConfirmarExibirConfirmacao', funcao);
                }
            },
            cancel: { 
                text: 'Não', 
                btnClass: 'btn-red' ,
                action: () => {
                    alt.emit('CancelarExibirConfirmacao', funcao);
                }
            } 
        }
    });
}

// alt:V - Callbacks
alt.on('chat:clearMessages', clearMessages);
alt.on('chat:pushMessage', pushMessage);
alt.on('chat:activateChat', activateChat);
alt.on('chat:activateInput', activateInput);
alt.on('chat:sendInput', sendInput);
alt.on('chat:scrollMessagesList', scrollMessagesList);
alt.on('chat:addInputToHistory', addInputToHistory);
alt.on('chat:shiftHistoryUp', shiftHistoryUp);
alt.on('chat:shiftHistoryDown', shiftHistoryDown);
alt.on('chat:notify', notify);
alt.on('chat:activateTimeStamp', activateTimeStamp);
alt.on('chat:toggleTelaPreta', toggleTelaPreta);
alt.on('chat:exibirConfirmacao', exibirConfirmacao);
alt.on('chat:toggleTelaVerde', toggleTelaVerde);
alt.on('chat:toggleTelaLaranja', toggleTelaLaranja);
alt.on('chat:toggleTelaCinza', toggleTelaCinza);