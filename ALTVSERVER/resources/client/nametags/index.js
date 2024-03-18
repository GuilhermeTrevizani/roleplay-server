import alt from 'alt-client';
import native from 'natives'
import * as Constants from '/helpers/constants.js';

alt.RmlElement.prototype.shown = false;
alt.loadRmlFont("/resources/fonts/arialbd.ttf", "arial", false, true);
const document = new alt.RmlDocument("/nametags/index.rml");
const container = document.getElementByID("nametag-container");
const nameTags = new Map();
let tickHandle = undefined;

alt.on('gameEntityCreate', entity => {
  if (entity instanceof alt.Player) {
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
  if (entity instanceof alt.Player) {
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

    let name = player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_NAMETAG);

    if (!name
      || alt.Player.local.pos.distanceTo(entity.pos) > drawDistance
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

      name = `~${player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_DAMAGED) ? 'r' : 'w'}~${name}`;
      const ferido = parseInt(player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_INJURED));
      if (ferido == 1 || ferido == 2)
        name = `(( Este jogador está gravemente ferido. ))\n${name}`;
      else if (ferido >= 3)
        name = `(( Este jogador está morto. ))\n${name}`;

      if (player.hasStreamSyncedMeta(Constants.PLAYER_META_DATA_GAME_UNFOCUSED))
        name += `~r~*`;
      else if (player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_CHATTING))
        name += `~y~*`;
      else if (player.isTalking)
        name += `~b~*`;

      // Colocar o action

      rmlElement.innerRML = name;

      const { x: screenX, y: screenY } = alt.worldToScreen(x, y, z + 2);
      rmlElement.style["left"] = `${screenX}px`;
      rmlElement.style["top"] = `${screenY}px`;
    }
  });
}