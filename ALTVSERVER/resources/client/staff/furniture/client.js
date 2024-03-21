import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFurnitures', (update, furnituresHTML) => {
  if (update) {
    if (view?.url == 'http://resource/staff/furniture/index.html')
      view.emit('loaded', furnituresHTML);

    return;
  }

  setView(new alt.WebView('http://resource/staff/furniture/index.html'));
  view.on('load', () => {
    view.emit('loaded', furnituresHTML);
  });
  view.on('closeView', closeView);
  view.on('save', (id, category, name, model, value) => {
    alt.emitServer('StaffFurnitureSave', id, category, name, model, value);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffFurnitureRemove', id);
  });
  view.focus();
  toggleView(true);
});