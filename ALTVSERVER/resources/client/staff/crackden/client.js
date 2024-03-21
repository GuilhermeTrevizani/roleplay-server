import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffCrackDens', (update, htmlCrackDens) => {
  if (update) {
    if (view?.url == 'http://resource/staff/crackden/index.html')
      view.emit('loaded', htmlCrackDens);

    return;
  }

  setView(new alt.WebView('http://resource/staff/crackden/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlCrackDens);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffCrackDenGoto', id);
  });
  view.on('save', (id, posX, posY, posZ, dimension, onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours) => {
    alt.emitServer('StaffCrackDenSave', id, new alt.Vector3(posX, posY, posZ), dimension,
      onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffCrackDenRemove', id);
  });
  view.on('editItems', (id) => {
    alt.emitServer('StaffCrackDensItemsShow', id);
  });
  view.on('revokeCooldown', (id) => {
    alt.emitServer('StaffCrackDenRevokeCooldown', id);
  });
  view.focus();
  toggleView(true);
});