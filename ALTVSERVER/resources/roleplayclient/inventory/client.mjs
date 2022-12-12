import * as alt from 'alt-client';
import * as native from 'natives';
import { view, setView, toggleView, closeView, getRightCoordsZ } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.mjs';

const player = alt.Player.local;
const lixeiras = [ 'prop_cs_dumpster_01a', 'prop_snow_dumpster_01', 'prop_bin_01a', 'prop_bin_02a', 'prop_bin_03a',
    'prop_bin_04a', 'prop_bin_05a', 'prop_bin_06a', 'prop_bin_07a', 'prop_bin_07b', 'prop_bin_07c',
    'prop_bin_07d', 'prop_bin_08a', 'prop_bin_08open', 'prop_bin_09a', 'prop_bin_10a',
    'prop_bin_10b', 'prop_bin_11a', 'prop_bin_11b', 'prop_bin_12a', 'prop_bin_13a',
    'prop_bin_14a', 'prop_bin_14b', 'prop_bin_beach_01a', 'prop_bin_beach_01d',
    'prop_bin_delpiero', 'prop_bin_delpiero_b', 'prop_dumpster_01a', 'prop_dumpster_02a', 'prop_dumpster_02b',
    'prop_dumpster_3a', 'prop_dumpster_3step', 'prop_dumpster_4a', 'prop_dumpster_4b', 'prop_recyclebin_01a',
    'prop_recyclebin_02_c', 'prop_recyclebin_02_d', 'prop_recyclebin_02a', 'prop_recyclebin_02b',
    'prop_recyclebin_03_a', 'prop_recyclebin_04_a', 'prop_recyclebin_04_b', 'prop_recyclebin_05_a'
];

alt.onServer('ActivateCurrentHUD', activateCurrentHUD);
function activateCurrentHUD() {
    if (!player.getMeta('f7')) {
        activateChat(true);
        native.displayHud(true);
        native.displayRadar(true);
    }
}

alt.onServer('Inventory:Show', (update, title, items, rightTitle, rightItems, type) => {
    if (update) {
        if (view?.url == 'http://resource/inventory/index.html')
            view.emit('loaded', title, JSON.parse(items), rightTitle, JSON.parse(rightItems), type, true);

        return;
    }

    closeView();
    activateChat(false);
    native.displayHud(false);
    native.displayRadar(false);
    setView(new alt.WebView('http://resource/inventory/index.html'));
    view.on('load', () => {
        view.emit('loaded', title, JSON.parse(items), rightTitle, JSON.parse(rightItems), type, false);
    });
    view.on('closeView', () => {
        activateCurrentHUD();
        closeView(); 
    });
    view.on('moveItem', (id, slot) => {
        alt.emitServer('MoveItem', id, slot);
    });
    view.on('giveItem', (id, quantity, codigoTarget) => {
        alt.emitServer('GiveItem', id, quantity, codigoTarget);
    });
    view.on('dropItem', (id, quantity) => {
        alt.emitServer('DropItem', id, quantity);
    });
    view.on('getItem', (id, quantity) => {
        alt.emitServer('GetItem', id, quantity);
    });
    view.on('robItem', (id, quantity) => {
        alt.emitServer('RobItem', id, quantity);
    });
    view.on('discardItem', (id, quantity) => {
        let temLixeiraPerto = false;
        lixeiras.forEach(bomb => {
            if (temLixeiraPerto) 
                return;
                    
            const object = native.getClosestObjectOfType(player.pos.x, player.pos.y, player.pos.z, 2.0, alt.hash(bomb), false, false, false);
            temLixeiraPerto = object != 0;
        });

        if (!temLixeiraPerto) {
            view.emit('mostrarErro', 'Você não está próximo de uma lixeira.');
            return;
        }

        alt.emitServer('DiscardItem', id, quantity);
    });
    view.on('storeItem', (id, quantity) => {
        alt.emitServer('StoreItem', id, quantity);
    });
    view.on('moveRightItem', (id, slot) => {
        alt.emitServer('MoveRightItem', id, slot);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Inventory:MoveItem', (id, newSlot, oldSlot) => {
    if (view?.url == 'http://resource/inventory/index.html')
        view.emit('moveItem', id, newSlot, oldSlot);
});

function clearDropObject() {
    alt.off('keydown', dropObjectKeyDown);
    closeView();
    if (dropObjectTick)
        alt.clearEveryTick(dropObjectTick);
    dropObjectTick = null;
    if (dropObject)
        dropObject.destroy();
    dropObject = null;
    native.freezeEntityPosition(player, false);
}

function dropObjectKeyDown(key) {
    if (key == 8) { // BACKSPACE
        let name = 'CancelDropItem';
        if (dropObjectType == 1)
            name = 'CancelDropBarrier';
        else if (dropObjectType == 2)
            name = 'CancelDropFurniture';
        alt.emitServer(name);
        clearDropObject();
    } else if (key == 13) { // ENTER
        let name = 'ConfirmDropItem';
        if (dropObjectType == 1)
            name = 'ConfirmDropBarrier';
        else if (dropObjectType == 2)
            name = 'ConfirmDropFurniture';
        alt.emitServer(name, dropObject.pos, dropObject.rot);
        clearDropObject();
    }
}

const rotSum = 0.1;
function setDropObjectRotation(type) {
    let rot = { ... dropObject.rot };

    if (type == 1)
        rot.x += rotSum;
    else if (type == 2)
        rot.x -= rotSum;
    else if (type == 3)
        rot.y += rotSum;
    else if (type == 4)
        rot.y -= rotSum;
    else if (type == 5)
        rot.z += rotSum;
    else if (type == 6)
        rot.z -= rotSum;

    dropObject.rot = new alt.Vector3(rot.x, rot.y, rot.z);
}

const posSum = 0.05;
function setDropObjectPosition(type) {
    let pos = { ... dropObject.pos };

    if (type == 1)
        pos.x += posSum;
    else if (type == 2)
        pos.x -= posSum;
    else if (type == 3)
        pos.y += posSum;
    else if (type == 4)
        pos.y -= posSum;
    else if (type == 5)
        pos.z += posSum;
    else if (type == 6)
        pos.z -= posSum;

    if (player.pos.distanceTo(pos) <= 2.5 && pos.z >= dropObjectMinZ)
        dropObject.pos = new alt.Vector3(pos.x, pos.y, pos.z);
}

let dropObjectTick;
let dropObject;
let dropObjectMinZ;
let dropObjectType;
alt.onServer('DropObject', (model, type) => {
    dropObjectType = type;
    clearDropObject();
    activateCurrentHUD();
    alt.on('keydown', dropObjectKeyDown);

    getRightCoordsZ(player.pos.x, player.pos.y, player.pos.z).then((minPos) => {
        native.freezeEntityPosition(player, true);
    
        setView(new alt.WebView('http://resource/inventory/dropitem.html', true));

        dropObjectMinZ = minPos.z;
        dropObject = new alt.Object(model, new alt.Vector3(player.pos.x, player.pos.y, minPos.z), alt.Vector3.zero, false, false);
        dropObject.toggleCollision(false, false);
    
        dropObjectTick = alt.everyTick(() => {
            if (native.isControlPressed(0, 189)) { // LEFT
                if (alt.isKeyDown(16))
                    setDropObjectRotation(1);
                else
                    setDropObjectPosition(1);
            } else if (native.isControlPressed(0, 190)) { // RIGHT 
                if (alt.isKeyDown(16))
                    setDropObjectRotation(2);
                else
                    setDropObjectPosition(2);
            } else if (native.isControlPressed(0, 188)) { // UP
                if (alt.isKeyDown(16))
                    setDropObjectRotation(3);
                else
                    setDropObjectPosition(3);
            } else if (native.isControlPressed(0, 187)) { // DOWN
                if (alt.isKeyDown(16))
                    setDropObjectRotation(4);
                else
                    setDropObjectPosition(4);
            } else if (native.isControlPressed(0, 208)) { // PAGE UP
                if (alt.isKeyDown(16))
                    setDropObjectRotation(5);
                else
                    setDropObjectPosition(5);
            } else if (native.isControlPressed(0, 207)) { // PAGE DOWN
                if (alt.isKeyDown(16))
                    setDropObjectRotation(6);
                else
                    setDropObjectPosition(6);
            }
        });
    });
});