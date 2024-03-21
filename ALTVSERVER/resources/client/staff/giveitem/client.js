import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('Staff:GiveItem', (categories) => {
  setView(new alt.WebView('http://resource/staff/giveitem/index.html'));
  view.on('load', () => {
    view.emit('loaded', JSON.parse(categories));
  });
  view.on('closeView', closeView);
  view.on('giveItem', (category, type, extra, quantity, targetId) => {
    alt.emitServer('StaffGiveItem', category, type, extra, quantity, targetId);
  });
  view.focus();
  toggleView(true);
});