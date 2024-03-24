import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFactionsStoragesItems', (update, htmlFactionsStoragesItems, factionStorageId) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionstorageitem/index.html')
      view.emit('loaded', htmlFactionsStoragesItems);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionstorageitem/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlFactionsStoragesItems);
  });
  view.on('closeView', closeView);
  view.on('save', (factionStorageItemId, category, type, quantity, extra) => {
    alt.emitServer('StaffFactionStorageItemSave', factionStorageItemId, factionStorageId, category, type, quantity, extra);
  });
  view.on('remove', (factionStorageItemId) => {
    alt.emitServer('StaffFactionStorageItemRemove', factionStorageItemId);
  });
  view.focus();
  toggleView(true);
});