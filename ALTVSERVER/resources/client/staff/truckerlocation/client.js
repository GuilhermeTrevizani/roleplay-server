import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffTruckerLocations', (update, htmlTruckerLocations) => {
  if (update) {
    if (view?.url == 'http://resource/staff/truckerlocation/index.html')
      view.emit('loaded', htmlTruckerLocations);

    return;
  }

  setView(new alt.WebView('http://resource/staff/truckerlocation/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlTruckerLocations);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffTruckerLocationGoto', id);
  });
  view.on('save', (id, name, posX, posY, posZ, deliveryValue, loadWaitTime, unloadWaitTime, allowedVehiclesJSON) => {
    alt.emitServer('StaffTruckerLocationSave', id, name, new alt.Vector3(posX, posY, posZ),
      deliveryValue, loadWaitTime, unloadWaitTime, allowedVehiclesJSON);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffTruckerLocationRemove', id);
  });
  view.on('editDeliveries', (id) => {
    alt.emitServer('StaffTruckerLocationsDeliveriesShow', id);
  });
  view.focus();
  toggleView(true);
});