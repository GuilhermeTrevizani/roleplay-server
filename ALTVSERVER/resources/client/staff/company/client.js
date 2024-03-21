import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffCompanies', (update, companiesHTML) => {
  if (update) {
    if (view?.url == 'http://resource/staff/company/index.html')
      view.emit('loaded', companiesHTML);

    return;
  }

  setView(new alt.WebView('http://resource/staff/company/index.html'));
  view.on('load', () => {
    view.emit('loaded', companiesHTML);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffCompanyGoto', id);
  });
  view.on('save', (id, name, posX, posY, posZ, weekRentValue) => {
    alt.emitServer('StaffCompanySave', id, name, new alt.Vector3(posX, posY, posZ), weekRentValue);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffCompanyRemove', id);
  });
  view.on('removeOwner', (id) => {
    alt.emitServer('StaffCompanyRemoveOwner', id);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
  });
  view.focus();
  toggleView(true);
});