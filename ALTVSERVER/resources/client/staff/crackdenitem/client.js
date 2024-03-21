import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffCrackDensItems', (update, htmlCrackDensItems, crackDenId) => {
  if (update) {
    if (view?.url == 'http://resource/staff/crackdenitem/index.html')
      view.emit('loaded', htmlCrackDensItems);

    return;
  }

  setView(new alt.WebView('http://resource/staff/crackdenitem/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlCrackDensItems);
  });
  view.on('closeView', closeView);
  view.on('save', (crackDenItemId, item, value) => {
    alt.emitServer('StaffCrackDenItemSave', crackDenItemId, crackDenId, item, value);
  });
  view.on('remove', (crackDenItemId) => {
    alt.emitServer('StaffCrackDenItemRemove', crackDenItemId);
  });
  view.focus();
  toggleView(true);
});