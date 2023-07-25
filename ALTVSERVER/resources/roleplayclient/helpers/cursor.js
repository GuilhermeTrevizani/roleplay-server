import * as alt from 'alt-client';
import * as native from 'natives';
import { toggleInput } from '/chat/index.mjs';

let cursorCount = 0;
export var view;

export async function setView(_view) {
    view = _view;
}

export async function showCursor(value) {
    if (value) {
        cursorCount += 1;
        alt.showCursor(true);
        return;
    }

    for (let i = 0; i < cursorCount; i++) {
        try {
            alt.showCursor(false);
        } catch (e) {}
    }

    cursorCount = 0;
}

export async function toggleView(show, toggleGameControls = true) {
    if (toggleGameControls)
        alt.toggleGameControls(!show);

    showCursor(show);
    toggleInput(!show);
}

export async function closeView() {
    if (view?.isVisible ?? false) {
        view?.destroy();
        view = null;
        toggleView(false);
    }
}
alt.onServer('Server:CloseView', closeView);

export function getAddress(pos) {
    let [discard, street, street2] = native.getStreetNameAtCoord(pos.x, pos.y, pos.z, 0, 0);
    let zoneName = native.getFilenameForAudioConversation(native.getNameOfZone(pos.x, pos.y, pos.z));
    let streetName = native.getStreetNameFromHashKey(street);

    let streetName2 = native.getStreetNameFromHashKey(street2);
    if (streetName2 != '') {
        streetName += ` / ${streetName2}`;
    }

    return [zoneName , streetName];
}

export function getRightCoordsZ(x, y, z, tries = 0) {
    return new Promise(resolve => {
        if (tries == 0) 
            native.setFocusPosAndVel(x, y, z, 0, 0, 0);

        if (tries == 40) { // 40 tries * 25ms = 1 second
            native.clearFocus();
            return resolve(new alt.Vector3(x, y, 0));
        }

        setTimeout(() => {
            if (!native.hasCollisionLoadedAroundEntity(alt.Player.local.scriptID)) 
                return resolve(getRightCoordsZ(x, y, z, ++tries));

            const newZ = native.getGroundZFor3dCoord(x, y, z, 0, false, false)[1];
            if (!newZ) 
                return resolve(getRightCoordsZ(x, y, z + 100, ++tries));

            native.clearFocus();
            resolve(new alt.Vector3(x, y, newZ));
         }, 25);
    });
}