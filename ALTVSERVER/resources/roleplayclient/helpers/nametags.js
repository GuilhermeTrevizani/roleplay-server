import alt from 'alt-client';
import * as native from 'natives';
import { drawText3d } from './text';

let drawDistance = 10;
let interval;

alt.onServer('nametags:Config', handleConfig);

function handleConfig(showNametags) {
    if (interval) {
        alt.clearInterval(interval);
        interval = null;
    }

    if (!showNametags)
        return;

    interval = alt.setInterval(drawNametags, 0);
}

async function drawNametags() {
    if (alt.Player.local.getMeta('f7'))
        return;

    const players = [...alt.Player.streamedIn];
    players.forEach(player => {
        drawPlayerNametag(player);
    });
}

async function drawPlayerNametag(player) {
    if (!player.valid)
        return;

    if (player === alt.Player.local) 
        return;

    let name = player.getSyncedMeta('nametag');
    if (!name) 
        return;

    const distance = alt.Player.local.pos.distanceTo(player.pos);
    if (distance > drawDistance)
        return;

    if (!native.hasEntityClearLosToEntity(alt.Player.local, player, 17))
        return;

    name = `~${player.getSyncedMeta("Damaged") ? 'r' : 'w'}~${name}`;
    const ferido = parseInt(player.getSyncedMeta('ferido'));
    if (ferido == 1 || ferido == 2)
        name = `(( Este jogador está gravemente ferido. ))\n${name}`;
    else if (ferido >= 3)
        name = `(( Este jogador está morto. ))\n${name}`;

    if (player.hasSyncedMeta('GameUnfocused'))
        name += `~r~*`;
    else if (player.getSyncedMeta('chatting'))
        name += `~y~*`;
    else if (player.isTalking)
        name += `~b~*`;

    let pos = {...player.pos};
    pos.z += player.vehicle ? 1.40 : 1.30;

    const entity = player.vehicle ? player.vehicle : player;
    const vector = native.getEntityVelocity(entity);
    const frameTime = native.getFrameTime();

    drawText3d(name,
        pos.x + vector.x * frameTime, 
        pos.y + vector.y * frameTime, 
        pos.z + vector.z * frameTime,
        0.4,
        4,
        255, 106, 77, 255
    );
    
    const action = player.getSyncedMeta('TextAction');
    if (action) {
        pos.z += 0.15;
        drawText3d(
            action,
            pos.x + vector.x * frameTime, 
            pos.y + vector.y * frameTime, 
            pos.z + vector.z * frameTime,
            0.35,
            4,
            194, 
            162, 
            218,
            255,
            true,
            false
        );
    }
}