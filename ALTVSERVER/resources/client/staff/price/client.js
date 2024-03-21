import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffPrices', (update, pricesHtml, typesJson) => {
  if (update) {
    if (view?.url == 'http://resource/staff/price/index.html')
      view.emit('loaded', pricesHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/price/index.html'));
  view.on('load', () => {
    view.emit('loaded', pricesHtml);
    view.emit('loadTypes', JSON.parse(typesJson));
  });
  view.on('closeView', closeView);
  view.on('save', (id, type, name, value) => {
    alt.emitServer('StaffPriceSave', id, type, name, value);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffPriceRemove', id);
  });
  view.focus();
  toggleView(true);
});