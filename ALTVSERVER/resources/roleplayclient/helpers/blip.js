import * as alt from 'alt';
import * as native from 'natives';

let blips = [];

alt.onServer('blip:create', createBlip);
alt.onServer('blip:remove', removeBlip);

export async function createBlip(codigo, sprite, label, color, pos, display, shortRange, scale) {
    const blip = native.addBlipForCoord(pos.x, pos.y, pos.z);
    native.setBlipAsShortRange(blip, shortRange);
    native.setBlipSprite(blip, sprite);
    native.setBlipColour(blip, color);
    native.setBlipScale(blip, scale);
    native.beginTextCommandSetBlipName('STRING');
    native.addTextComponentSubstringPlayerName(label);
    native.endTextCommandSetBlipName(blip);
    native.setBlipDisplay(blip, display);

    blips.push({
        codigo,
        blip
    });

    return blip;
}

export async function removeBlip(codigo) {
    var x = blips.findIndex(x => x.codigo === codigo);
    if (x === -1)
        return;

    native.removeBlip(blips[x].blip);
    blips.splice(x, 1);
}