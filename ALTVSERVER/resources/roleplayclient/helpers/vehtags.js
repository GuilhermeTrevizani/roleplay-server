import alt from 'alt-client';
import * as native from 'natives';
import * as Constants from '/helpers/constants.js';

let drawDistance = 10;
let interval;

alt.onServer('dl:Config', handleConfig);

function handleConfig(showTags) {
    if (interval) {
        alt.clearInterval(interval);
        interval = null;
    }

    if (!showTags)
        return;

    interval = alt.setInterval(drawVehciletags, 0);
}

async function drawVehciletags() {
    if (alt.Player.local.getMeta('f7'))
        return;

    const vehicles = [...alt.Vehicle.streamedIn];
    vehicles.forEach(vehicle => {
        drawVehicleNametag(vehicle);
    });
}

async function drawVehicleNametag(vehicle) {
    const placa = vehicle.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_PLATE);
    if (!placa) 
        return;

    const id = vehicle.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_ID);
    if (!id) 
        return;

    const modelo = vehicle.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_MODEL);
    if (!modelo) 
        return;

    const dist = alt.Player.local.pos.distanceTo(vehicle.pos);
    if (dist > drawDistance)
        return;

    if (!native.hasEntityClearLosToEntity(alt.Player.local, vehicle, 17))
        return;

    let name = `~w~ID: ~s~${id}\n`;
    name += `~w~Modelo: ~s~${modelo}\n`;
    name += `~w~Placa: ~s~${placa}\n`;
    name += `~w~Motor: ~s~${native.getVehicleEngineHealth(vehicle.scriptID).toFixed(0)}`;

    const vector = native.getEntityVelocity(vehicle);
    const frameTime = native.getFrameTime();

    alt.Utils.drawText3dThisFrame(
        name,
        new alt.Vector3(vehicle.pos.x + vector.x * frameTime, vehicle.pos.y + vector.y * frameTime, vehicle.pos.z + vector.z * frameTime),
        4,
        0.4,
        new alt.RGBA(174, 106, 178, 255),
        true,
        true
    );
}