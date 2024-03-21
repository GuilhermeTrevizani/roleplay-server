import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffAnimations', (update, animationsHTML) => {
  const viewUrl = 'http://resource/staff/animations/index.html';
  if (update && view?.url == viewUrl) {
    view.emit('loaded', animationsHTML);
    return;
  }

  setView(new alt.WebView(viewUrl));
  view.on('load', () => {
    view.emit('loaded', animationsHTML);
  });
  view.on('closeView', closeView);
  view.on('save', (id, display, dictionary, name, flag, duration, vehicle) => {
    alt.emitServer('StaffAnimationSave', id, display, dictionary, name, flag, duration, vehicle);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffAnimationRemove', id);
  });
  view.focus();
  toggleView(true);
});