import alt from 'alt-client';
import native from 'natives'
import * as Constants from '/helpers/constants.js';

alt.RmlElement.prototype.shown = false;
alt.loadRmlFont("/resources/fonts/arialbd.ttf", "arial", false, true);
const document = new alt.RmlDocument("/vehtags/index.rml");
const container = document.getElementByID("nametag-container");
const nameTags = new Map();
let tickHandle = undefined;
const drawDistance = 10;

alt.on('gameEntityCreate', entity => {
  if (entity instanceof alt.Vehicle) {
    const rmlElement = document.createElement("button");
    rmlElement.rmlId = entity.id.toString();
    rmlElement.addClass("nametag");
    rmlElement.addClass("hide");
    nameTags.set(entity, rmlElement);
    container.appendChild(rmlElement);
    if (tickHandle !== undefined) return;
    tickHandle = alt.everyTick(drawMarkers);
  }
});

alt.on('gameEntityDestroy', entity => {
  if (entity instanceof alt.Vehicle) {
    const rmlElement = nameTags.get(entity);
    if (rmlElement === undefined) return;
    container.removeChild(rmlElement);
    rmlElement.destroy();
    nameTags.delete(entity);
    if (tickHandle === undefined || nameTags.size > 0) return;
    alt.clearEveryTick(tickHandle);
    tickHandle = undefined;
  }
});

function drawMarkers() {
  nameTags.forEach((rmlElement, entity) => {
    const { x, y, z } = entity.pos;

    if (alt.Player.local.pos.distanceTo(entity.pos) > drawDistance
      || !native.isSphereVisible(x, y, z, 0.0099999998)
      || !native.hasEntityClearLosToEntity(alt.Player.local, entity, 17)) {
      if (!rmlElement.shown) return;
      rmlElement.addClass("hide");
      rmlElement.shown = false;
    } else {
      if (!rmlElement.shown) {
        rmlElement.removeClass("hide");
        rmlElement.shown = true;
      }

      const id = entity.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_ID);
      const plate = entity.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_PLATE);
      const model = entity.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_MODEL);
      const engine = native.getVehicleEngineHealth(entity).toFixed(0);
      rmlElement.innerRML = `[id: ${id}, model: ${model}, plate: ${plate}, engine: ${engine}]`;

      const { x: screenX, y: screenY } = alt.worldToScreen(x, y, z);
      rmlElement.style["left"] = `${screenX}px`;
      rmlElement.style["top"] = `${screenY}px`;
    }
  });
}