import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFactionsStorages', (update, html) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionstorage/index.html')
      view.emit('loaded', html);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionstorage/index.html'));
  view.on('load', () => {
    view.emit('loaded', html);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffFactionStorageGoto', id);
  });
  view.on('save', (id, factionId, posX, posY, posZ, dimension) => {
    alt.emitServer('StaffFactionStorageSave', id, factionId, new alt.Vector3(posX, posY, posZ), dimension);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffFactionStorageRemove', id);
  });
  view.on('editItems', (id) => {
    alt.emitServer('StaffFactionsStoragesItemsShow', id);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z, alt.Player.local.dimension);
  });
  view.focus();
  toggleView(true);
});