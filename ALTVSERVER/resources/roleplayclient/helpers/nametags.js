import alt from 'alt-client';
import * as native from 'natives';

let drawDistance = 20;
let interval;

alt.onServer('nametags:Config', handleConfig);

function handleConfig(showNametags) {
    if (!showNametags) {
        if (interval) {
            alt.clearInterval(interval);
            interval = null;
        }
        return;
    }

    interval = alt.setInterval(drawNametags, 1);
}

async function drawNametags() {
    if (alt.Player.local.getMeta('STOP_DRAWS'))
        return;

    const players = [...alt.Player.all];
    players.forEach(player => {
        drawPlayerNametag(player);
    });
}

async function drawPlayerNametag(player) {
    if (player.scriptID === alt.Player.local.scriptID) 
        return;

    let name = player.getSyncedMeta('nametag');
    if (!name) 
        return;

    if (!native.isEntityOnScreen(player.scriptID))
        return;

    let dist = distance2d(alt.Player.local.pos, player.pos);
    if (dist > drawDistance)
        return;

    if (!native.hasEntityClearLosToEntity(alt.Player.local.scriptID, player.scriptID, 17))
        return;

    name = `~w~${name}`;
    let ferido = parseInt(player.getSyncedMeta('ferido'));
    if (ferido == 1)
        name = `(( Esse jogador está gravemente ferido. ))\n${name}`;
    else if (ferido == 2)
        name = `(( Esse jogador perdeu a consciência. ))\n${name}`;

    name += ` [${player.getSyncedMeta('id')}]`;
    if (player.isTalking)
        name += `~y~*`;
    else if (player.getSyncedMeta('chatting'))
        name += `~r~*`;

    let pos = {...player.pos};
    pos.z += player.vehicle ? 1.40 : 1.30;

    native.setDrawOrigin(pos.x, pos.y, pos.z, 0);
    native.beginTextCommandDisplayText('STRING');
    native.setTextFont(4);
    native.setTextScale(0.4, 0.4);
    native.setTextProportional(true);
    native.setTextCentre(true);
    native.setTextColour(255, 106, 77, 255);
    native.setTextOutline();
    native.addTextComponentSubstringPlayerName(name);
    native.endTextCommandDisplayText(0, 0);
    native.clearDrawOrigin();
}

function distance2d(vector1, vector2) {
    return Math.sqrt(Math.pow(vector1.x - vector2.x, 2) + Math.pow(vector1.y - vector2.y, 2) + Math.pow(vector1.z - vector2.z, 2));
}