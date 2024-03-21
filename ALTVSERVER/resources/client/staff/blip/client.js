import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffBlips', (update, blipsHtml) => {
  if (update) {
    if (view?.url == 'http://resource/staff/blip/index.html')
      view.emit('loaded', blipsHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/blip/index.html'));
  view.on('load', () => {
    view.emit('loaded', blipsHtml);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffBlipGoto', id);
  });
  view.on('save', (id, name, posX, posY, posZ, type, color) => {
    alt.emitServer('StaffBlipSave', id, name, new alt.Vector3(posX, posY, posZ), type, color);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffBlipRemove', id);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
  });
  view.focus();
  toggleView(true);
});