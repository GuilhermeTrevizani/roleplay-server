import * as alt from 'alt-client';
import * as native from 'natives';
import { view, setView, toggleView, showCursor, closeView } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.js';

let tuningEveryTick = null;

alt.onServer('VehicleTuning', (tuning) => {
  tuningEveryTick = alt.everyTick(() => {
    native.disableControlAction(0, 71, true); // INPUT_VEH_ACCELERATE
    native.disableControlAction(0, 72, true); // INPUT_VEH_BRAKE
  });

  native.displayHud(false);
  native.displayRadar(false);
  activateChat(false);

  setView(new alt.WebView('http://resource/tuning/html/index.html'));
  view.on('load', () => {
    view.emit('tuning:SetData', JSON.parse(tuning));
  });
  view.on('tuning:Done', (confirm, _tuning) => {
    alt.emitServer('ConfirmTuning', confirm, JSON.stringify(_tuning));
  });
  view.on('tuning:SyncMod', (mod) => {
    alt.emitServer('TuningSyncMod', mod.Type, mod.Selected);
  });
  view.on('tuning:SyncColor', (color1, color2) => {
    alt.emitServer('TuningSyncColor', color1, color2);
  });
  view.on('tuning:SyncWheel', (wheelType, wheelVariation, wheelColor) => {
    alt.emitServer('TuningSyncWheel', wheelType, wheelVariation, wheelColor);
  });
  view.on('tuning:SyncNeon', (neonColor, neonLeft, neonRight, neonFront, neonBack) => {
    alt.emitServer('TuningSyncNeon', neonColor, neonLeft, neonRight, neonFront, neonBack);
  });
  view.on('tuning:SyncXenonColor', (headlightColor, lightsMultiplier) => {
    alt.emitServer('TuningSyncXenonColor', headlightColor, lightsMultiplier);
  });
  view.on('tuning:SyncWindowTint', (windowTint) => {
    alt.emitServer('TuningSyncWindowTint', windowTint);
  });
  view.on('tuning:SyncTireSmokeColor', (tireSmokeColor) => {
    alt.emitServer('TuningSyncTireSmokeColor', tireSmokeColor);
  });
  view.focus();
  toggleView(true);
  alt.on('keydown', keydown);
});

alt.onServer('VehicleTuning:Close', () => {
  if (tuningEveryTick)
    alt.clearEveryTick(tuningEveryTick);

  native.displayHud(true);
  native.displayRadar(true);
  activateChat(true);

  alt.off('keydown', keydown);
  closeView();
  toggleView(false);
})

function keydown(key) {
  if (key == 114) { // F3
    const gameControls = !alt.gameControlsEnabled();
    showCursor(!gameControls);
    alt.toggleGameControls(gameControls);
  }
}

alt.onServer('Tuning:ShowMessage', (message) => {
  view.emit('tuning:ShowMessage', message);
});