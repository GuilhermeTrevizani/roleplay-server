import * as alt from 'alt';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.mjs';

let sexo = 0;
let tipoFaccao = 0;

// 0 - Criação do Personagem
// 1 - Loja de Roupas
// 2 - Uniforme
let tipo = 0;

alt.onServer('AbrirLojaRoupas', (sexo, tipo, tipoFaccao) => {
    native.displayHud(false);
    native.displayRadar(false);
    activateChat(false);
    alt.emit('character:EditClothes', sexo, tipo, tipoFaccao);
});

alt.on('character:EditClothes', handleEdit);
alt.onServer('Character:CloseClothes', handleClose);

function handleEdit(_sexo, _tipo = 0, _tipoFaccao = 0) {
    sexo = _sexo;
    tipo = _tipo;
    tipoFaccao = _tipoFaccao;

    setView(new alt.WebView('http://resource/clothes/html/index.html'));
    view.on('load', handleReadyDone);
    view.on('character:Done', handleDone);
    view.on('character:Cancel', handleCancel);
    view.on('character:SetClothes', handleSetClothes);
    view.on('character:SetProps', handleSetProps);
    view.on('character:GetClothMaxTexture', (component, drawable) => {
        view.emit('character:SetMaxTexture', native.getNumberOfPedTextureVariations(alt.Player.local, component, drawable));
    });
    view.on('character:GetPropMaxTexture', (component, drawable) => {
        view.emit('character:SetMaxTexture', native.getNumberOfPedPropTextureVariations(alt.Player.local, component, drawable));
    });
    view.focus();

    native.freezeEntityPosition(alt.Player.local, true);
    toggleView(true, false);
    if (_tipo == 0)
        native.setEntityHeading(alt.Player.local, 169.24);
    createPedEditCamera(false);
    setFov(50);
    setZPos(0.6);
}

function handleClose() {
    closeView();
    native.freezeEntityPosition(alt.Player.local, false);
    destroyPedEditCamera();
}

function handleDone(newRoupas, newAcessorios) {
    if (tipo == 0)
        handleClose();

    alt.emitServer('ConfirmarLojaRoupas', JSON.stringify(newRoupas), JSON.stringify(newAcessorios), tipo, true);
}

function handleCancel() {
    if (tipo == 0)
        handleClose();

    alt.emitServer('ConfirmarLojaRoupas', '', '', tipo, false);
}

function handleSetClothes(component, drawable, texture, dlc) {
    alt.emitServer('SetClothes', component, drawable, texture, dlc);
}

function handleSetProps(component, drawable, texture, dlc) {
    alt.emitServer('SetProps', component, drawable, texture, dlc);
}

function handleReadyDone() {
    view.emit('character:SetData', sexo, tipo, tipoFaccao);
}

alt.onServer('Character:ShowMessage', (message) => {
    view.emit('character:ShowMessage', message);
});