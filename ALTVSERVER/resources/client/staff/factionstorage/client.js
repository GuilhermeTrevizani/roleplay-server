import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFactionsArmories', (update, htmlFactionsArmories) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionstorage/index.html')
      view.emit('loaded', htmlFactionsArmories);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionstorage/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlFactionsArmories);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffFactionArmoryGoto', id);
  });
  view.on('save', (id, factionId, posX, posY, posZ, dimension) => {
    alt.emitServer('StaffFactionArmorySave', id, factionId, new alt.Vector3(posX, posY, posZ), dimension);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffFactionArmoryRemove', id);
  });
  view.on('editWeapons', (id) => {
    alt.emitServer('StaffFactionsArmorysWeaponsShow', id);
  });
  view.focus();
  toggleView(true);
});