import * as alt from 'alt';
import * as native from 'natives';
import { drawText2d } from '/helpers/text.js';

const fov_max = 80.0;
const fov_min = 10.0; // max zoom level (smaller fov is more zoom)
const zoomspeed = 2.0; // camera zoom speed
const speed_lr = 3.0; // speed by which the camera pans left-right
const speed_ud = 3.0; // speed by which the camera pans up-down
const toggle_vision = 25; // control id to toggle vision mode. Default: INPUT_AIM (Right mouse btn)
const toggle_light = 24; // left mouse btn

let scaleform;
let helicam = false;
let fov = (fov_max + fov_min) * 0.5;
let vision_state = 0; // 0 is normal, 1 is nightmode, 2 is thermal vision

let lightStatus = false;
var lights = [];

let cam;
let player = alt.Player.local;

let spotlightTick;

alt.on('keyup', (key) => {
    if (key == 113 && player.vehicle) // F2
        alt.emitServer('HelicamToggle');
});

alt.onServer('Helicam:Toggle', helicamToggle);
function helicamToggle(desativar = false) {
    if (helicam || desativar) {
        native.clearTimecycleModifier();
        if (scaleform != null || scaleform != 0)
            native.setScaleformMovieAsNoLongerNeeded(scaleform);

        if (cam != null) 
            native.destroyAllCams(true);

         native.renderScriptCams(false, false, 0, true, false, 0);
    
        cam = null;
        helicam = false;
        native.setSeethrough(false);
        native.setNightvision(false);
        vision_state = 0;
        lightStatus = false;
        alt.emitServer('SpotlightRemove');
    } else {
        native.setTimecycleModifier("heliGunCam");
        native.setTimecycleModifierStrength(0.3);

        scaleform = native.requestScaleformMovie("HELI_CAM");
        cam = native.createCamWithParams('DEFAULT_SCRIPTED_FLY_CAMERA', 
            player.pos.x, player.pos.y, player.pos.z,
            0, 0, native.getEntityHeading(player.scriptID),
            60, true, 0);
        native.setCamActive(cam, true);

        native.setCamRot(cam, 0.0, 0.0, native.getEntityHeading(player.vehicle.scriptID), 2);
        native.setCamFov(cam, fov);
        native.renderScriptCams(true, false, 0, true, false, 0);

        native.attachCamToEntity(cam, player.vehicle.scriptID, 0.0, 0.0, -1.5, true);

        native.beginScaleformMovieMethod(scaleform, "SET_CAM_LOGO");
        native.scaleformMovieMethodAddParamInt(1);
        native.endScaleformMovieMethod();

        helicam = true;
    }
}

alt.everyTick(() => {
    let closeLights = lights.filter(x => player.pos.distanceTo(x.position) <= 400);
    for (let light of closeLights) {
        native.drawSpotLight(
            light.position.x,
            light.position.y,
            light.position.z,
            light.direction.x,
            light.direction.y,
            light.direction.z,
            221,
            221,
            221,
            light.distance, 
            light.brightness, 
            light.hardness, 
            light.radius, 
            light.falloff
        );
    }

    if (helicam && player.vehicle) {
        if (cam !== null && native.isCamActive(cam) && native.isCamRendering(cam)) {

        var x = native.getControlNormal(7, 1) * speed_lr;
        var y = native.getControlNormal(7, 2) * speed_ud;
        var zoomIn = native.getControlNormal(2, 40) * zoomspeed;
        var zoomOut = native.getControlNormal(2, 41) * zoomspeed;

        var currentRot = native.getCamRot(cam, 2);

        currentRot = new alt.Vector3(currentRot.x - y, 0, currentRot.z - x);

        native.setCamRot(cam, currentRot.x, currentRot.y, currentRot.z, 2);

        if (zoomIn > 0) {
                var currentFov = native.getCamFov(cam);
                currentFov -= zoomIn;
                if (currentFov < fov_min) 
                    currentFov = fov_min;
                native.setCamFov(cam, currentFov);
            } else if (zoomOut > 0) {
                var currentFov = native.getCamFov(cam);
                currentFov += zoomOut;
                if (currentFov > fov_max) 
                    currentFov = fov_max;
                native.setCamFov(cam, currentFov);
            }
        }

        if (native.isControlJustPressed(0, toggle_vision)) {
            native.playSoundFrontend(
                -1,
                "SELECT",
                "HUD_FRONTEND_DEFAULT_SOUNDSET",
                false
            );
            ChangeVision();
        }

        if (native.isControlJustPressed(0, toggle_light)) {
            native.playSoundFrontend(
                -1,
                "SELECT",
                "HUD_FRONTEND_DEFAULT_SOUNDSET",
                false
            );
            lightStatus = !lightStatus;
            if (!lightStatus)
                alt.emitServer('SpotlightRemove');
        }
    
        const vehicle = pointingAt(cam);
        if (vehicle != null) {
            const veh = alt.Vehicle.streamedIn.find(x => x.scriptID == vehicle);
            if (native.isSphereVisible(veh.pos.x, veh.pos.y, veh.pos.z, 0.0099999998))
                drawText2d(`Modelo: ${veh.getSyncedMeta('modelo')}`, 0.5, 0.93, 0.55, 0, 255, 255, 255, 185);
        }

        native.beginScaleformMovieMethod(scaleform, "SET_ALT_FOV_HEADING");
        native.scaleformMovieMethodAddParamFloat(player.vehicle.pos.z);
        native.scaleformMovieMethodAddParamFloat(native.getCamFov(cam));
        native.scaleformMovieMethodAddParamFloat(native.getCamRot(cam, 2).z);
        native.endScaleformMovieMethod();
        native.drawScaleformMovieFullscreen(scaleform, 255, 255, 255, 255, 1);
    }
});

function ChangeVision() {
    if (vision_state == 0) {
        native.setNightvision(true);
        vision_state = 1;
    } /*else if (vision_state == 1) {
        native.setNightvision(true);
        native.setSeethrough(true);
        vision_state = 2;
    }*/ else {
        native.setSeethrough(false);
        native.setNightvision(false);
        vision_state = 0;
    }
}

alt.setInterval(() => {
    if (lightStatus && spotlightPosition && spotlightDirection)
        alt.emitServer('SpotlightAdd', spotlightPosition, spotlightDirection, 300.0, 1.0, 0.0, 13.0, 1.0, true);
}, 250);

let spotlightPosition = null;
let spotlightDirection = null;

function pointingAt(camera) {
    const distance = 2000;
    spotlightPosition = native.getCamCoord(camera);
    const rotation = native.getCamRot(camera, 2);
    spotlightDirection = GetDirectionFromRotation(rotation);

    const farAway = new alt.Vector3((spotlightPosition.x + (spotlightDirection.x * distance)), (spotlightPosition.y + (spotlightDirection.y * distance)), (spotlightPosition.z + (spotlightDirection.z * distance)));

    const raycast = native.startExpensiveSynchronousShapeTestLosProbe(spotlightPosition.x, spotlightPosition.y, spotlightPosition.z, 
        farAway.x, farAway.y, farAway.z, 
        2, player, 4);

    const result = native.getShapeTestResult(raycast, undefined, undefined, undefined, undefined);

    if (result) {
        const hitEntity = result[4];

        if (hitEntity === null || hitEntity === undefined) 
            return null;
            
        if (hitEntity.scriptID === player.scriptID) 
            return null;

        if (native.getEntityType(hitEntity) == 2) 
            return hitEntity;

        return null;
    }

    return null;
}

function GetDirectionFromRotation(rotation) {
    const z = rotation.z * (Math.PI / 180.0);
    const x = rotation.x * (Math.PI / 180.0);
    const num = Math.abs(Math.cos(x));

    return new alt.Vector3(
        (-Math.sin(z) * num),
        (Math.cos(z) * num),
        Math.sin(x)
    );
}

alt.onServer('Spotlight:Add', (id, position, direction, distance, brightness, hardness, radius, falloff) => {
    const x = lights.findIndex(x => x.id === id);
    if (x === -1) {
        lights.push({id, position, direction, distance, brightness, hardness, radius, falloff });
        return;
    }

    lights[x].position = position;  
    lights[x].direction = direction;  
    lights[x].distance = distance; 
    lights[x].brightness = brightness; 
    lights[x].hardness = hardness; 
    lights[x].radius = radius; 
    lights[x].falloff = falloff; 
});

alt.onServer('Spotlight:Remove', (id) => {
    const x = lights.findIndex(x => x.id === id);
    if (x === -1) 
        return;

    lights.splice(x, 1);
});

alt.onServer('Spotlight:Cancel', () => {
    lightStatus = false;
});

alt.onServer('Spotlight:Toggle', (state) => {
    if (spotlightTick != null)  {
        alt.clearEveryTick(spotlightTick);
        spotlightTick = null;
    }

    if (!state)
        return;

    spotlightTick = alt.everyTick(() => {
        if (!player.vehicle) 
            return;

        const door = native.getEntityBoneIndexByName(player.vehicle, 'door_dside_f');
        const window = native.getEntityBoneIndexByName(player.vehicle, 'windscreen');
        const doorCoords = native.getWorldPositionOfEntityBone(player.vehicle, door);
        const windowCoords = native.getWorldPositionOfEntityBone(player.vehicle, window);

        const forwardVector = native.getEntityForwardVector(player.vehicle);
        const heading = native.getEntityHeading(player.vehicle);

        let newY = player.vehicle.getStreamSyncedMeta("spotlightY") ?? 0;
        let newZ = player.vehicle.getStreamSyncedMeta("spotlightZ") ?? 0;

        if (native.isControlPressed(0, 127)) { // NumPad 8
            newZ += 0.1;
        } else if (native.isControlPressed(0, 126)) { // NumPad 5
            newZ -= 0.1;
        } else if (native.isControlPressed(0, 124)) { // NumPad 4
            newY += (heading >= 180 && heading <= 365 ? 0.1 : -0.1); 
        } else if (native.isControlPressed(0, 125)) { // NumPad 6
            newY += (heading >= 180 && heading <= 365 ? -0.1 : 0.1);
        }

        if (newZ > 0.2)
            newZ = 0.2;

        if (newZ < -0.2)
            newZ = -0.2;

        if (newY > 3)
            newY = 3;
    
        if (newY < -3)
            newY = -3;

       /*     Logica setar heading e pegar qual o forward vector padrão. Daria certo? 
0
90
180
360*/

        alt.emitServer('SetVehicleMeta', player.vehicle, 'spotlightY', newY);
        alt.emitServer('SetVehicleMeta', player.vehicle, 'spotlightZ', newZ);
        alt.emitServer('SpotlightAdd', new alt.Vector3(doorCoords.x, windowCoords.y, doorCoords.z), 
            new alt.Vector3(forwardVector.x, forwardVector.y + newY, forwardVector.z + newZ),
            70.0, 50.0, 4.3, 25.0, 28.6, 
            false);
    });
});