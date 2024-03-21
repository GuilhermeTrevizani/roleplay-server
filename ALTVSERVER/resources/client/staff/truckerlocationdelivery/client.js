import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffTruckerLocationsDeliveries', (update, htmlTruckerLocationsDeliveries, truckerLocationId) => {
  if (update) {
    if (view?.url == 'http://resource/staff/truckerlocationdelivery/index.html')
      view.emit('loaded', htmlTruckerLocationsDeliveries);

    return;
  }

  setView(new alt.WebView('http://resource/staff/truckerlocationdelivery/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlTruckerLocationsDeliveries);
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffTruckerLocationDeliveryGoto', id);
  });
  view.on('save', (truckerLocationDeliveryId, posX, posY, posZ) => {
    alt.emitServer('StaffTruckerLocationDeliverySave', truckerLocationDeliveryId, truckerLocationId, new alt.Vector3(posX, posY, posZ));
  });
  view.on('remove', (truckerLocationDeliveryId) => {
    alt.emitServer('StaffTruckerLocationDeliveryRemove', truckerLocationDeliveryId);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
  });
  view.focus();
  toggleView(true);
});