import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffInfos', (update, infosHtml) => {
  if (update) {
    if (view?.url == 'http://resource/staff/info/index.html')
      view.emit('loaded', infosHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/info/index.html'));
  view.on('load', () => {
    view.emit('loaded', infosHtml);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffInfoGoto', id);
  });
  view.on('save', (days, message) => {
    alt.emitServer('StaffInfoSave', days, message);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffInfoRemove', id);
  });
  view.focus();
  toggleView(true);
});