import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffFactionsArmoriesWeapons', (update, htmlFactionsArmoriesWeapons, factionArmoryId) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionstorageitem/index.html')
      view.emit('loaded', htmlFactionsArmoriesWeapons);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionstorageitem/index.html'));
  view.on('load', () => {
    view.emit('loaded', htmlFactionsArmoriesWeapons);
  });
  view.on('closeView', closeView);
  view.on('save', (factionArmoryWeaponId, weapon, ammo, quantity, tintIndex, componentsJSON) => {
    alt.emitServer('StaffFactionArmoryWeaponSave', factionArmoryWeaponId, factionArmoryId, weapon, ammo, quantity, tintIndex, componentsJSON);
  });
  view.on('remove', (factionArmoryWeaponId) => {
    alt.emitServer('StaffFactionArmoryWeaponRemove', factionArmoryWeaponId);
  });
  view.focus();
  toggleView(true);
});