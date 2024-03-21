import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffSpots', (update, spotsHtml, typesJson) => {
  if (update) {
    if (view?.url == 'http://resource/staff/spot/index.html')
      view.emit('loaded', spotsHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/spot/index.html'));
  view.on('load', () => {
    view.emit('loaded', spotsHtml);
    view.emit('loadTypes', JSON.parse(typesJson));
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffSpotGoto', id);
  });
  view.on('save', (id, type, posX, posY, posZ, auxiliarPosX, auxiliarPosY, auxiliarPosZ) => {
    alt.emitServer('StaffSpotSave', id, type, new alt.Vector3(posX, posY, posZ), new alt.Vector3(auxiliarPosX, auxiliarPosY, auxiliarPosZ));
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffSpotRemove', id);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
  });
  view.on('getAuxiliarPosition', () => {
    view.emit('setAuxiliarPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
  });
  view.focus();
  toggleView(true);
});