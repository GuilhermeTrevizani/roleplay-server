import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFactions', (update, factionsHtml, typesJson) => {
  if (update) {
    if (view?.url == 'http://resource/staff/faction/index.html')
      view.emit('loaded', factionsHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/faction/index.html'));
  view.on('load', () => {
    view.emit('loaded', factionsHtml);
    view.emit('loadTypes', JSON.parse(typesJson));
  });
  view.on('closeView', closeView);
  view.on('save', (id, name, type, color, slots, chatColor) => {
    alt.emitServer('StaffFactionSave', id, name, type, color, slots, chatColor);
  });
  view.on('ranks', (id) => {
    alt.emitServer('StaffFactionShowRanks', id);
  });
  view.on('members', (id) => {
    alt.emitServer('StaffFactionShowMembers', id);
  });
  view.focus();
  toggleView(true);
});