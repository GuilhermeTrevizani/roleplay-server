import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffVehicles', (update, vehiclesHtml) => {
  if (update) {
    if (view?.url == 'http://resource/staff/vehicle/index.html')
      view.emit('loaded', vehiclesHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/vehicle/index.html'));
  view.on('load', () => {
    view.emit('loaded', vehiclesHtml);
  });
  view.on('closeView', closeView);
  view.on('save', (vehicleId, model, type, typeId, livery, color1R, color1G, color1B, color2R, color2G, color2B) => {
    alt.emitServer('StaffVehicleSave', vehicleId, model, type, typeId, livery, color1R, color1G, color1B, color2R, color2G, color2B);
  });
  view.on('remove', (vehicleId) => {
    alt.emitServer('StaffVehicleRemove', vehicleId);
  });
  view.focus();
  toggleView(true);
});