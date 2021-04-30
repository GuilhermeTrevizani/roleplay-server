import * as alt from 'alt';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { showCursor } from '/helpers/cursor.js';

let view;
let oldData = {};
let barbearia = 0;

alt.on('character:Edit', handleEdit);
alt.on('character:Sync', handleSync);

async function handleEdit(_oldData, _barbearia) {
    oldData = _oldData;
    barbearia = _barbearia;

    view = new alt.WebView('http://resource/charcreator/html/index.html');
    view.on('load', handleReadyDone);
    view.on('character:Done', handleDone);
    view.on('character:Cancel', handleCancel);
    view.on('character:Sync', handleSync);
    view.focus();

    showCursor(true);
    if (barbearia == 0)
        native.setEntityHeading(alt.Player.local.scriptID, 169.24);
    createPedEditCamera();
    setFov(50);
    setZPos(0.6);
}

function closeView() {
    if (view)
        view.destroy();

    oldData = null;
    view = null;
    showCursor(false);
    native.freezeEntityPosition(alt.Player.local.scriptID, false);
    destroyPedEditCamera();
}

function handleDone(newData) {
    alt.emitServer('ConfirmarPersonalizacao', JSON.stringify(newData), barbearia, true);
    closeView();
}

function handleCancel() {
    alt.emitServer('ConfirmarPersonalizacao', JSON.stringify(oldData), barbearia, false);
    closeView();
}

function handleReadyDone() {
    view.emit('character:SetData', oldData, barbearia);
}

async function handleSync(data, setarRoupas = true) {
    native.clearPedBloodDamage(alt.Player.local.scriptID);
    native.clearPedDecorations(alt.Player.local.scriptID);
    native.setPedHeadBlendData(alt.Player.local.scriptID, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);

    native.setPedHeadBlendData(alt.Player.local.scriptID, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
    native.setPedHeadBlendData(
        alt.Player.local.scriptID,
        data.faceFather,
        data.faceMother,
        0,
        data.skinFather,
        data.skinMother,
        0,
        parseFloat(data.faceMix),
        parseFloat(data.skinMix),
        0,
        false
    );

    // Facial Features
    for (let i = 0; i < data.structure.length; i++) {
        const value = data.structure[i];
        native.setPedFaceFeature(alt.Player.local.scriptID, i, value);
    }

    // Overlay Features - NO COLORS
    for (let i = 0; i < data.opacityOverlays.length; i++) {
        const overlay = data.opacityOverlays[i];
        native.setPedHeadOverlay(alt.Player.local.scriptID, overlay.id, overlay.value, parseFloat(overlay.opacity));
    }

    // Hair
    if (data.hairOverlay != null) {
        const collection = native.getHashKey(data.hairOverlay.collection);
        const overlay = native.getHashKey(data.hairOverlay.overlay);
        native.addPedDecorationFromHashes(alt.Player.local.scriptID, collection, overlay);
    }
    
    native.setPedComponentVariation(alt.Player.local.scriptID, 2, data.hair, 0, 0);
    native.setPedHairColor(alt.Player.local.scriptID, data.hairColor1, data.hairColor2);

    // Facial Hair
    native.setPedHeadOverlay(alt.Player.local.scriptID, 1, data.facialHair, data.facialHairOpacity);
    native.setPedHeadOverlayColor(alt.Player.local.scriptID, 1, 1, data.facialHairColor1, data.facialHairColor1);

    // Eyebrows
    native.setPedHeadOverlay(alt.Player.local.scriptID, 2, data.eyebrows, 1);
    native.setPedHeadOverlayColor(alt.Player.local.scriptID, 2, 1, data.eyebrowsColor1, data.eyebrowsColor1);

    // Decor
    for (let i = 0; i < data.colorOverlays.length; i++) {
        const overlay = data.colorOverlays[i];
        const color2 = overlay.color2 ? overlay.color2 : overlay.color1;
        native.setPedHeadOverlay(alt.Player.local.scriptID, overlay.id, overlay.value, parseFloat(overlay.opacity));
        native.setPedHeadOverlayColor(alt.Player.local.scriptID, overlay.id, 1, overlay.color1, color2);
    }

    // Eyes
    native.setPedEyeColor(alt.Player.local.scriptID, data.eyes);

    if (!barbearia && setarRoupas) { 
        if (data.sex === 0) {
            native.setPedComponentVariation(alt.Player.local.scriptID, 3, 15, 0, 0); // arms
            native.setPedComponentVariation(alt.Player.local.scriptID, 4, 14, 0, 0); // pants
            native.setPedComponentVariation(alt.Player.local.scriptID, 6, 35, 0, 0); // shoes
            native.setPedComponentVariation(alt.Player.local.scriptID, 8, 15, 0, 0); // shirt
            native.setPedComponentVariation(alt.Player.local.scriptID, 11, 15, 0, 0); // torso
        } else {
            native.setPedComponentVariation(alt.Player.local.scriptID, 3, 15, 0, 0); // arms
            native.setPedComponentVariation(alt.Player.local.scriptID, 4, 14, 0, 0); // pants
            native.setPedComponentVariation(alt.Player.local.scriptID, 6, 34, 0, 0); // shoes
            native.setPedComponentVariation(alt.Player.local.scriptID, 8, 15, 0, 0); // shirt
            native.setPedComponentVariation(alt.Player.local.scriptID, 11, 91, 0, 0); // torso
        }
    }
}