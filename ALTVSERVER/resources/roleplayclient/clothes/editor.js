import * as alt from 'alt';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { showCursor } from '/helpers/cursor.js';

let view;
let oldRoupas = [];
let oldAcessorios = [];
let sexo = 0;
let slots = 1;
let roupa = 1;
let tipo = 0;
let tipoFaccao = 0;

alt.on('character:EditClothes', handleEdit);
alt.on('character:SyncClothes', handleSync);

function handleEdit(_oldRoupas, _oldAcessorios, _sexo, _slots = 1, _roupa = 1, _tipo = 0, _tipoFaccao = 0) {
    oldRoupas = _oldRoupas;
    oldAcessorios = _oldAcessorios;
    sexo = _sexo;
    slots = _slots;
    roupa = _roupa;
    tipo = _tipo;
    tipoFaccao = _tipoFaccao;

    view = new alt.WebView('http://resource/clothes/html/index.html');
    view.on('load', handleReadyDone);
    view.on('character:Done', handleDone);
    view.on('character:Cancel', handleCancel);
    view.on('character:SyncClothes', handleSync);
    view.on('log', (txt) => {
        alt.log(txt);
    });
    view.focus();

    showCursor(true);
    if (_tipo == 0)
        native.setEntityHeading(alt.Player.local.scriptID, 169.24);
    createPedEditCamera(false);
    setFov(50);
    setZPos(0.6);
}

function closeView() {
    if (view)
        view.destroy();

    view = null;
    showCursor(false);
    native.freezeEntityPosition(alt.Player.local.scriptID, false);
    destroyPedEditCamera();
}

function handleDone(newRoupas, newAcessorios, newRoupa) {
    alt.emitServer('ConfirmarLojaRoupas', JSON.stringify(newRoupas), JSON.stringify(newAcessorios), newRoupa, tipo, true);
    closeView();
}

function handleCancel() {
    alt.emitServer('ConfirmarLojaRoupas', JSON.stringify(oldRoupas), JSON.stringify(oldAcessorios), roupa, tipo, false);
    closeView();
}

function handleReadyDone() {
    view.emit('character:SetData', oldRoupas, oldAcessorios, sexo, slots, roupa, tipo, tipoFaccao);
}

function handleSync(dataRoupas, dataAcessorios) {
    for (let i = 0; i < dataRoupas.length; i++) {
        let value = dataRoupas[i];
        native.setPedComponentVariation(alt.Player.local.scriptID, value.Slot, value.Drawable, value.Texture, 0);
    }

    for (let i = 0; i < dataAcessorios.length; i++) {
        let value = dataAcessorios[i];
        if (value.Drawable != -1)
            native.setPedPropIndex(alt.Player.local.scriptID, value.Slot, value.Drawable, value.Texture, true);
        else
            native.clearPedProp(alt.Player.local.scriptID, value.Slot);
    }
}