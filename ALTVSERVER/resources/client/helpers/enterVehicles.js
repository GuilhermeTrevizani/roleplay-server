import * as alt from 'alt-client';
import * as native from 'natives';
import * as Constants from '/helpers/constants.js';

const player = alt.Player.local;
const timeout = 7000;

export async function enterVehicleAsDriver() {
  if (player.vehicle)
    return;

  if (player.getMeta('animation_freeze'))
    return;

  if (player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_INJURED) != 0)
    return;

  let vehicle;
  let lastDistance = 7;
  const vehicles = [...alt.Vehicle.streamedIn];
  vehicles.forEach(veh => {
    var vehiclePosition = veh.pos;
    var distance = player.pos.distanceTo(vehiclePosition);
    if (distance < lastDistance) {
      vehicle = veh;
      lastDistance = distance;
    }
  });

  if (vehicle == undefined)
    return;

  if (!native.isVehicleSeatFree(vehicle, -1, false))
    return;

  if (!native.hasEntityClearLosToEntity(player, vehicle, 17))
    return;

  native.taskEnterVehicle(player, vehicle, timeout, -1, 2, 1, null, 0);
}

export async function enterVehicleAsPassenger() {
  if (player.vehicle)
    return;

  if (player.getMeta('animation_freeze'))
    return;

  if (player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_INJURED) != 0)
    return;

  let vehicle;
  let lastDistance = 7;
  const vehicles = [...alt.Vehicle.streamedIn];
  vehicles.forEach(veh => {
    var vehiclePosition = veh.pos;
    var distance = player.pos.distanceTo(vehiclePosition);
    if (distance < lastDistance) {
      vehicle = veh;
      lastDistance = distance;
    }
  });

  if (vehicle == undefined)
    return;

  if (!native.hasEntityClearLosToEntity(player, vehicle, 17))
    return;

  if (native.isThisModelABike(vehicle.model)) {
    if (native.isVehicleSeatFree(vehicle, 0, false))
      native.taskEnterVehicle(player, vehicle, timeout, 0, 1, 1, null, 0);
    return;
  }

  const seatRear = native.getEntityBoneIndexByName(vehicle, 'seat_r');
  const seatFrontPassenger = native.getEntityBoneIndexByName(vehicle, 'seat_pside_f');
  const seatRearDriver = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r');
  const seatRearDriver1 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r1');
  const seatRearDriver2 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r2');
  const seatRearDriver3 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r3');
  const seatRearDriver4 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r4');
  const seatRearDriver5 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r5');
  const seatRearDriver6 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r6');
  const seatRearDriver7 = native.getEntityBoneIndexByName(vehicle, 'seat_dside_r7');
  const seatRearPassenger = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r');
  const seatRearPassenger1 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r1');
  const seatRearPassenger2 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r2');
  const seatRearPassenger3 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r3');
  const seatRearPassenger4 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r4');
  const seatRearPassenger5 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r5');
  const seatRearPassenger6 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r6');
  const seatRearPassenger7 = native.getEntityBoneIndexByName(vehicle, 'seat_pside_r7');

  const seatRearPosition = seatRear === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRear);
  const seatFrontPassengerPosition = seatFrontPassenger === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatFrontPassenger);
  const seatRearDriverPosition = seatRearDriver === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver);
  const seatRearDriver1Position = seatRearDriver1 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver1);
  const seatRearDriver2Position = seatRearDriver2 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver2);
  const seatRearDriver3Position = seatRearDriver3 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver3);
  const seatRearDriver4Position = seatRearDriver4 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver4);
  const seatRearDriver5Position = seatRearDriver5 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver5);
  const seatRearDriver6Position = seatRearDriver6 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver6);
  const seatRearDriver7Position = seatRearDriver7 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearDriver7);
  const seatRearPassengerPosition = seatRearPassenger === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger);
  const seatRearPassenger1Position = seatRearPassenger1 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger1);
  const seatRearPassenger2Position = seatRearPassenger2 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger2);
  const seatRearPassenger3Position = seatRearPassenger3 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger3);
  const seatRearPassenger4Position = seatRearPassenger4 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger4);
  const seatRearPassenger5Position = seatRearPassenger5 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger5);
  const seatRearPassenger6Position = seatRearPassenger6 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger6);
  const seatRearPassenger7Position = seatRearPassenger7 === -1 ? null : native.getWorldPositionOfEntityBone(vehicle, seatRearPassenger7);

  let closestFreeSeatNumber = -1;
  let seatIndex = -1;
  let closestSeatDistance = Number.MAX_SAFE_INTEGER;
  let calculatedDistance = null;
  const playerPos = player.pos;

  // Inline Rear
  calculatedDistance = seatRearPosition === null ? null : playerPos.distanceTo(seatRearPosition);
  seatIndex = seatRear === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`1 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  // Side by Side vehicles
  calculatedDistance = seatFrontPassengerPosition === null ? null : playerPos.distanceTo(seatFrontPassengerPosition);
  seatIndex = seatFrontPassenger === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`2 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriverPosition === null ? null : playerPos.distanceTo(seatRearDriverPosition);
  seatIndex = seatRearDriver === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`3 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassengerPosition === null ? null : playerPos.distanceTo(seatRearPassengerPosition);
  seatIndex = seatRearPassenger === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`4 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  // Force inner seats before outer grab holds if shift not pressed
  calculatedDistance = seatRearDriver1Position === null ? null : playerPos.distanceTo(seatRearDriver1Position);
  seatIndex = seatRearDriver1 === -1 ? seatIndex : seatIndex + 1; // 3
  if (!native.isVehicleSeatFree(vehicle, seatIndex - 2, false) || alt.isKeyDown(16)) {
    if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
      closestSeatDistance = calculatedDistance;
      closestFreeSeatNumber = seatIndex;
    }
  }

  if (alt.debug)
    alt.log(`5 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  // Force inner seats before outer grab holds if shift not pressed
  calculatedDistance = seatRearPassenger1Position === null ? null : playerPos.distanceTo(seatRearPassenger1Position);
  seatIndex = seatRearPassenger1 === -1 ? seatIndex : seatIndex + 1; // 4
  if (!native.isVehicleSeatFree(vehicle, seatIndex - 2, false) || alt.isKeyDown(16)) {
    if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
      closestSeatDistance = calculatedDistance;
      closestFreeSeatNumber = seatIndex;
    }
  }

  if (alt.debug)
    alt.log(`6 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  // Force inner seats before outer grab holds if shift not pressed
  calculatedDistance = seatRearDriver2Position === null ? null : playerPos.distanceTo(seatRearDriver2Position);
  seatIndex = seatRearDriver2 === -1 ? seatIndex : seatIndex + 1; // 5
  if (!native.isVehicleSeatFree(vehicle, seatIndex - 4, false) || alt.isKeyDown(16)) {
    if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
      closestSeatDistance = calculatedDistance;
      closestFreeSeatNumber = seatIndex;
    }
  }

  if (alt.debug)
    alt.log(`7 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  // Force inner seats before outer grab holds if shift not pressed
  calculatedDistance = seatRearPassenger2Position === null ? null : playerPos.distanceTo(seatRearPassenger2Position);
  seatIndex = seatRearPassenger2 === -1 ? seatIndex : seatIndex + 1; // 6
  if (!native.isVehicleSeatFree(vehicle, seatIndex - 4, false) || alt.isKeyDown(16)) {
    if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
      closestSeatDistance = calculatedDistance;
      closestFreeSeatNumber = seatIndex;
    }
  }

  if (alt.debug)
    alt.log(`8 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriver3Position === null ? null : playerPos.distanceTo(seatRearDriver3Position);
  seatIndex = seatRearDriver3 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`9 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassenger3Position === null ? null : playerPos.distanceTo(seatRearPassenger3Position);
  seatIndex = seatRearPassenger3 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`10 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriver4Position === null ? null : playerPos.distanceTo(seatRearDriver4Position);
  seatIndex = seatRearDriver4 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`11 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassenger4Position === null ? null : playerPos.distanceTo(seatRearPassenger4Position);
  seatIndex = seatRearPassenger4 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`12 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriver5Position === null ? null : playerPos.distanceTo(seatRearDriver5Position);
  seatIndex = seatRearDriver5 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`13 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassenger5Position === null ? null : playerPos.distanceTo(seatRearPassenger5Position);
  seatIndex = seatRearPassenger5 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`14 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriver6Position === null ? null : playerPos.distanceTo(seatRearDriver6Position);
  seatIndex = seatRearDriver6 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`15 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassenger6Position === null ? null : playerPos.distanceTo(seatRearPassenger6Position);
  seatIndex = seatRearPassenger6 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`16 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearDriver7Position === null ? null : playerPos.distanceTo(seatRearDriver7Position);
  seatIndex = seatRearDriver7 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`17 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  calculatedDistance = seatRearPassenger7Position === null ? null : playerPos.distanceTo(seatRearPassenger7Position);
  seatIndex = seatRearPassenger7 === -1 ? seatIndex : seatIndex + 1;
  if (calculatedDistance !== null && native.isVehicleSeatFree(vehicle, seatIndex, false) && calculatedDistance < closestSeatDistance) {
    closestSeatDistance = calculatedDistance;
    closestFreeSeatNumber = seatIndex;
  }

  if (alt.debug)
    alt.log(`18 closestSeatDistance: ${closestSeatDistance} | closestFreeSeatNumber: ${closestFreeSeatNumber}`);

  if (closestFreeSeatNumber === -1)
    return;

  native.taskEnterVehicle(player, vehicle, timeout, closestFreeSeatNumber, 2, 1, null, 0);
}