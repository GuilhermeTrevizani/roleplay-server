import * as alt from 'alt';
import * as native from 'natives';

let markers = [];

alt.setInterval(() => {
    let closeMarkers = markers.filter(x => alt.Player.local.pos.distance(x.pos) < x.range);
    for (let x of closeMarkers) {
        native.drawMarker(x.type, 
            x.pos.x, x.pos.y, x.pos.z, 
            x.dir.x, x.dir.y, x.dir.z, 
            x.rot.x, x.rot.y, x.rot.z, 
            x.scale.x, x.scale.y, x.scale.z,
            x.rgba.r, x.rgba.g, x.rgba.b, x.rgba.a,
            x.bobUpAndDown,
            x.faceCamera,
            x.p19,
            x.rotate,
            x.textureDict,
            x.textureName,
            x.drawOnEnts);
    }
}, 1);

alt.onServer('marker:create', createMarker);
alt.onServer('marker:remove', removeMarker);

export async function createMarker(codigo, type, pos, dir, rot, scale, rgba, range,
    bobUpAndDown = true, faceCamera = true, p19 = 2, rotate = true, textureDict = null, textureName = null, drawOnEnts = false) {

    markers.push({
        codigo,
        type,
        pos,
        dir,
        rot,
        scale,
        rgba,
        range,
        bobUpAndDown,
        faceCamera,
        p19,
        rotate,
        textureDict,
        textureName,
        drawOnEnts
    });
}

export async function removeMarker(codigo) {
    var x = markers.findIndex(x => x.codigo === codigo);
    if (x === -1)
        return;

    markers.splice(x, 1);
}