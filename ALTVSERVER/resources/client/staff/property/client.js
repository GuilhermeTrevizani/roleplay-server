import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffProperties', (update, propertiesHtml, interiorsJson) => {
  if (update) {
    if (view?.url == 'http://resource/staff/property/index.html')
      view.emit('loaded', propertiesHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/property/index.html'));
  view.on('load', () => {
    view.emit('loaded', propertiesHtml);
    view.emit('loadInteriors', JSON.parse(interiorsJson));
  });
  view.on('closeView', closeView);
  view.on('goto', (id) => {
    alt.emitServer('StaffPropertyGoto', id);
  });
  view.on('save', (id, interior, value, dimension, posX, posY, posZ) => {
    const pos = new alt.Vector3(posX, posY, posZ);
    const [zoneName, streetName] = getAddress(pos);
    const address = `${streetName}, ${zoneName}`;
    alt.emitServer('StaffPropertySave', id, interior, value, dimension, pos, address);
  });
  view.on('remove', (id) => {
    alt.emitServer('StaffPropertyRemove', id);
  });
  view.on('getPosition', () => {
    view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z, alt.Player.local.dimension);
  });
  view.focus();
  toggleView(true);
});