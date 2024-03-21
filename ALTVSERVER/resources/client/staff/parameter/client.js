import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffParameters', (parameters) => {
  setView(new alt.WebView('http://resource/staff/parameter/index.html'));
  view.on('load', () => {
    view.emit('loaded', JSON.parse(parameters));
  });
  view.on('closeView', closeView);
  view.on('save', (jsonParameters) => {
    alt.emitServer('StaffParametersSave', jsonParameters);
  });
  view.focus();
  toggleView(true);
});