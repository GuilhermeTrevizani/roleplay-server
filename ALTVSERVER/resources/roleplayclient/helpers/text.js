import * as alt from 'alt';
import * as native from 'natives';

let textActions = [];
let textDraws = [];

alt.setInterval(() => {
    if (alt.Player.local.getMeta('STOP_DRAWS'))
        return;
        
    let closeTextDraws = textDraws.filter(x => alt.Player.local.pos.distance(x.pos) < x.range);
    for (let x of closeTextDraws) {
        drawText3d(
            x.nome,
            x.pos.x,
            x.pos.y,
            x.pos.z,
            x.size, 
            x.font,
            x.color.r, 
            x.color.g, 
            x.color.b,
            x.color.a,
            true,
            false
        );
    }
}, 1);

alt.onServer('textDraw:create', (codigo, nome, pos, range, size, font, color, dimension) => {
    var x = textDraws.findIndex(x => x.codigo === codigo);
    if (x !== -1)
        return;

    textDraws.push({
        codigo,
        nome,
        pos,
        range,
        size,
        font,
        color,
        dimension
    });
});

alt.onServer('textDraw:remove', (codigo) => {
    var x = textDraws.findIndex(x => x.codigo === codigo);
    if (x === -1)
        return;

    textDraws.splice(x, 1);
});

alt.onServer('text:playerAction', (player, msg) => {
    var x = textActions.findIndex(x => x.player === player);
    if (x !== -1) {
        alt.clearTimeout(textActions[x].timeoutAction);
        alt.clearInterval(textActions[x].intervalAction);
        textActions.splice(x, 1);
    }

    let intervalAction = alt.setInterval(() => {
        let pos = {...player.pos};
        pos.z += player.vehicle ? 1.55 : 1.45;

        drawText3d(
            msg,
            pos.x,
            pos.y,
            pos.z,
            0.35,
            4,
            194, 
            162, 
            218,
            255,
            true,
            false
        );
    }, 0);
    let timeoutAction = alt.setTimeout(() => {
        alt.clearInterval(intervalAction);
    }, 7000);
    textActions.push({
        player,
        intervalAction,
        timeoutAction
    });
});

alt.onServer('text:Animated', (text, duration) => {
    let pos = alt.Player.local.pos;
    let alpha = 255;
    const interval = alt.setInterval(() => {
        if (alpha <= 0) alpha = 0;
        alt.nextTick(() => {
            drawText3d(
                text,
                pos.x,
                pos.y,
                pos.z + 1,
                0.5,
                4,
                255,
                255,
                255,
                alpha,
                true,
                false
            );
        });
        pos.z += 0.0075;
        alpha -= 3;
    }, 0);
    alt.setTimeout(() => {
        alt.clearInterval(interval);
    }, duration);
});

export async function drawText3d(
    msg,
    x,
    y,
    z,
    scale,
    fontType,
    r,
    g,
    b,
    a,
    useOutline = true,
    useDropShadow = true
) {
    native.setDrawOrigin(x, y, z, 0);
    native.beginTextCommandDisplayText('STRING');
    native.addTextComponentSubstringPlayerName(msg);
    native.setTextFont(fontType);
    native.setTextScale(1, scale);
    native.setTextWrap(0.0, 1.0);
    native.setTextCentre(true);
    native.setTextColour(r, g, b, a);

    if (useOutline) native.setTextOutline();

    if (useDropShadow) native.setTextDropShadow();

    native.endTextCommandDisplayText(0, 0);
    native.clearDrawOrigin();
}

export async function drawText2d(
    msg,
    x,
    y,
    scale,
    fontType,
    r,
    g,
    b,
    a,
    useOutline = true,
    useDropShadow = true,
    align = 0
) {
    native.beginTextCommandDisplayText('STRING');
    native.addTextComponentSubstringPlayerName(msg);
    native.setTextFont(fontType);
    native.setTextScale(1, scale);
    native.setTextWrap(0.0, 1.0);
    native.setTextCentre(true);
    native.setTextColour(r, g, b, a);
    native.setTextJustification(align);

    if (useOutline) 
        native.setTextOutline();

    if (useDropShadow) 
        native.setTextDropShadow();

    native.endTextCommandDisplayText(x, y);
}