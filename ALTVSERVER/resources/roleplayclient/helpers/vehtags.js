import alt from 'alt-client';
import * as native from 'natives';

let drawDistance = 20;
let interval;

alt.onServer('dl:Config', handleConfig);

function handleConfig(showTags) {
    if (!showTags) {
        if (interval) {
            alt.clearInterval(interval);
            interval = null;
        }
        return;
    }

    interval = alt.setInterval(drawNametags, 1);
}

async function drawNametags() {
    const vehicles = [...alt.Vehicle.all];
    vehicles.forEach(vehicle => {
        drawVehicleNametag(vehicle);
    });
}

async function drawVehicleNametag(vehicle) {
    let placa = vehicle.getSyncedMeta('placa');
    if (!placa) 
        return;

    let id = vehicle.getSyncedMeta('id');
    if (!id) 
        return;

    let modelo = vehicle.getSyncedMeta('modelo');
    if (!modelo) 
        return;

    if (!native.isEntityOnScreen(vehicle.scriptID))
        return;

    let dist = distance2d(alt.Player.local.pos, vehicle.pos);
    if (dist > drawDistance)
        return;

    if (!native.hasEntityClearLosToEntity(alt.Player.local.scriptID, vehicle.scriptID, 17))
        return;

    let name = `~w~ID: ~s~${id}\n`;
    name += `~w~Modelo: ~s~${modelo}\n`;
    name += `~w~Placa: ~s~${placa}\n`;
    name += `~w~Motor: ~s~${native.getVehicleEngineHealth(vehicle.scriptID).toFixed(2)}`;

    native.setDrawOrigin(vehicle.pos.x, vehicle.pos.y, vehicle.pos.z, 0);
    native.beginTextCommandDisplayText('STRING');
    native.setTextFont(4);
    native.setTextScale(0.4, 0.4);
    native.setTextProportional(true);
    native.setTextCentre(true);
    native.setTextColour(174, 106, 178, 255);
    native.setTextOutline();
    native.addTextComponentSubstringPlayerName(name);
    native.endTextCommandDisplayText(0, 0);
    native.clearDrawOrigin();
}

function distance2d(vector1, vector2) {
    return Math.sqrt(Math.pow(vector1.x - vector2.x, 2) + Math.pow(vector1.y - vector2.y, 2) + Math.pow(vector1.z - vector2.z, 2));
}