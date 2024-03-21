import alt from 'alt-client';
import native from 'natives';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffDoors', (update, htmlPortas) => {
  if (update) {
    if (view?.url == 'http://resource/staff/door/index.html')
      view.emit('loaded', htmlPortas);

    return;
  }

  setView(new alt.WebView('http://resource/staff/door/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlPortas);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffDoorGoto', id);
  });
  view.on('save', (id, name, hash, posX, posY, posZ, factionId, companyId, locked) => {
    alt.emitServer('StaffDoorSave', id, name, hash, new alt.Vector3(posX, posY, posZ), factionId, companyId, locked);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffDoorRemove', id);
  });
  view.focus();
  toggleView(true);
});

alt.onServer('DoorControl', (hash, pos, closed) => {
  native.setLockedUnstreamedInDoorOfType(hash, pos.x, pos.y, pos.z, closed, 0.0, 0.0, 0.0);
});