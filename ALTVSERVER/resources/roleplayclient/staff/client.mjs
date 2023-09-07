import * as alt from 'alt-client';
import * as native from 'natives';
import { view, setView, toggleView, closeView, getAddress } from '/helpers/cursor.js';

alt.onServer('Staff:GiveItem', (categories) => {
    setView(new alt.WebView('http://resource/staff/giveitem.html'));
    view.on('load', () => {
        view.emit('loaded', JSON.parse(categories));
    });
    view.on('closeView', closeView);
    view.on('giveItem', (category, type, extra, quantity, targetId) => {
        alt.emitServer('StaffGiveItem', category, type, extra, quantity, targetId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffDoors', (update, htmlPortas) => {
    if (update) {
        if (view?.url == 'http://resource/staff/doors.html')
            view.emit('loaded', htmlPortas);

        return;
    }

    setView(new alt.WebView('http://resource/staff/doors.html'));
    view.on('load', () => {
        view.emit('loaded', htmlPortas);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffDoorGoto', id);
    });
    view.on('save', (id, name, hash, posX, posY, posZ, factionId, companyId, locked) => {
        alt.emitServer('StaffDoorSave', id, name, hash, new alt.Vector3(posX, posY, posZ), factionId, companyId, locked);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffDoorRemove', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('DoorControl', (hash, pos, closed) => {
    native.setLockedUnstreamedInDoorOfType(hash, pos.x, pos.y, pos.z, closed, 0.0, 0.0, 0.0);
});

alt.onServer('ACPShow', (flags, bans, sos, logTypes, staff) => {
    setView(new alt.WebView('http://resource/staff/acp.html'));
    view.on('load', () => {
        view.emit('updateFlags', JSON.parse(flags));
        view.emit('updateBans', JSON.parse(bans));
        view.emit('updateSOS', JSON.parse(sos));
        view.emit('updateLogTypes', JSON.parse(logTypes));
        view.emit('updateStaff', JSON.parse(staff));
    });
    view.on('closeView', closeView);
    view.on('unban', (id, total) => {
        alt.emitServer('StaffUnban', id, total);
    });
    view.on('searchLogs', (dataInicial, dataFinal, tipo, personagemOrigem, personagemDestino, descricao) => {
        alt.emitServer('StaffSearchLogs', dataInicial, dataFinal, tipo, personagemOrigem, personagemDestino, descricao);
    });
    view.on('searchUser', (search) => {
        alt.emitServer('StaffSearchUser', search);
    });
    view.on('searchCharacter', (search) => {
        alt.emitServer('StaffSearchCharacter', search);
    });
    view.on('banCharacter', (id, days, reason) => {
        alt.emitServer('StaffBanCharacter', id, days, reason);
    });
    view.on('saveUser', (userId, staff, flagsJSON) => {
        alt.emitServer('StaffSaveUser', userId, staff, flagsJSON);
    });
    view.on('ckCharacter', (id, reason) => {
        alt.emitServer('StaffCKCharacter', id, reason);
    });
    view.on('ckAvalationRemoveCharacter', (id) => {
        alt.emitServer('StaffCKAvalationRemoveCharacter', id);
    });
    view.on('ckAvalationCharacter', (id) => {
        alt.emitServer('StaffCKAvalationCharacter', id);
    });
    view.on('nameChangeStatusCharacter', (id) => {
        alt.emitServer('StaffNameChangeStatusCharacter', id);
    });
    view.on('removeForumNameChange', (id) => {
        alt.emitServer('StaffRemoveForumNameChangeUser', id);
    });
    view.on('removeJailCharacter', (id) => {
        alt.emitServer('StaffRemoveJailCharacter', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('ACPUpdateBans', (bans) => {
    if (view?.url == 'http://resource/staff/acp.html')
        view.emit('updateBans', JSON.parse(bans));
});

alt.onServer('ACPUpdateSOS', (sos) => {
    if (view?.url == 'http://resource/staff/acp.html')
        view.emit('updateSOS', JSON.parse(sos));
});

alt.onServer('ACPUpdateLogs', (logs) => {
    if (view?.url == 'http://resource/staff/acp.html')
        view.emit('updateLogs', JSON.parse(logs));
});

alt.onServer('ACPUpdateUser', (html) => {
    if (view?.url == 'http://resource/staff/acp.html')
        view.emit('updateUser', html);
});

alt.onServer('ACPUpdateCharacter', (html) => {
    if (view?.url == 'http://resource/staff/acp.html')
        view.emit('updateCharacter', html);
});

alt.onServer('StaffParameters', (parameters) => {
    setView(new alt.WebView('http://resource/staff/parameters.html'));
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

alt.onServer('Staff:MostrarMensagem', (mensagem, fechar) => {
    view.emit('mostrarMensagem', mensagem, fechar);
});

alt.onServer('StaffPrices', (update, pricesHtml, typesJson) => {
    if (update) {
        if (view?.url == 'http://resource/staff/prices.html')
            view.emit('loaded', pricesHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/prices.html'));
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

alt.onServer('StaffSpots', (update, spotsHtml, typesJson) => {
    if (update) {
        if (view?.url == 'http://resource/staff/spots.html')
            view.emit('loaded', spotsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/spots.html'));
    view.on('load', () => {
        view.emit('loaded', spotsHtml);
        view.emit('loadTypes', JSON.parse(typesJson));
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffSpotGoto', id);
    });
    view.on('save', (id, type, posX, posY, posZ, auxiliarPosX, auxiliarPosY, auxiliarPosZ) => {
        alt.emitServer('StaffSpotSave', id, type, new alt.Vector3(posX, posY, posZ), new alt.Vector3(auxiliarPosX, auxiliarPosY, auxiliarPosZ));
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffSpotRemove', id);
    });
    view.on('getPosition', () => {
        view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
    });
    view.on('getAuxiliarPosition', () => {
        view.emit('setAuxiliarPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffBlips', (update, blipsHtml) => {
    if (update) {
        if (view?.url == 'http://resource/staff/blips.html')
            view.emit('loaded', blipsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/blips.html'));
    view.on('load', () => {
        view.emit('loaded', blipsHtml);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffBlipGoto', id);
    });
    view.on('save', (id, name, posX, posY, posZ, type, color) => {
        alt.emitServer('StaffBlipSave', id, name, new alt.Vector3(posX, posY, posZ), type, color);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffBlipRemove', id);
    });
    view.on('getPosition', () => {
        view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffProperties', (update, propertiesHtml, interiorsJson) => {
    if (update) {
        if (view?.url == 'http://resource/staff/properties.html')
            view.emit('loaded', propertiesHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/properties.html'));
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

alt.onServer('StaffFactions', (update, factionsHtml, typesJson) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factions.html')
            view.emit('loaded', factionsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factions.html'));
    view.on('load', () => {
        view.emit('loaded', factionsHtml);
        view.emit('loadTypes', JSON.parse(typesJson));
    });
    view.on('closeView', closeView);
    view.on('save', (id, name, type, color, slots, chatColor) => {
        alt.emitServer('StaffFactionSave', id, name, type, color, slots, chatColor);
    });
    view.on('ranks', (id) => {
        alt.emitServer('StaffFactionShowRanks', id);
    });
    view.on('members', (id) => {
        alt.emitServer('StaffFactionShowMembers', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffShowFactionRanks', (update, ranksHtml, factionId, factionName) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionranks.html')
            view.emit('loaded', ranksHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionranks.html'));
    view.on('load', () => {
        view.emit('loaded', ranksHtml);
        view.emit('init', factionName);
    });
    view.on('closeView', closeView);
    view.on('save', (factionRankId, name, salary) => {
        alt.emitServer('StaffFactionRankSave', factionId, factionRankId, name, salary);
    });
    view.on('remove', (factionRankId) => {
        alt.emitServer('StaffFactionRankRemove', factionRankId);
    });
    view.on('order', (ranksJSON) => {
        alt.emitServer('StaffFactionRankOrder', factionId, ranksJSON);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffShowFactionMembers', (update, membersHtml, factionId, factionName, government, ranksJSON, flagsJSON) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionmembers.html')
            view.emit('loaded', membersHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionmembers.html'));
    view.on('load', () => {
        view.emit('init', factionName, JSON.parse(ranksJSON), JSON.parse(flagsJSON), government);
        view.emit('loaded', membersHtml);
    });
    view.on('closeView', closeView);
    view.on('invite', (characterId) => {
        alt.emitServer('StaffFactionMemberInvite', factionId, characterId);
    });
    view.on('save', (characterId, factionRankId, badge, flags) => {
        alt.emitServer('StaffFactionMemberSave', factionId, characterId, factionRankId, badge, flags);
    });
    view.on('remove', (characterId) => {
        alt.emitServer('StaffFactionMemberRemove', factionId, characterId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffFactionsArmories', (update, htmlFactionsArmories) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionsarmories.html')
            view.emit('loaded', htmlFactionsArmories);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionsarmories.html'));
    view.on('load', () => {
        view.emit('loaded', htmlFactionsArmories);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffFactionArmoryGoto', id);
    });
    view.on('save', (id, factionId, posX, posY, posZ, dimension) => {
        alt.emitServer('StaffFactionArmorySave', id, factionId, new alt.Vector3(posX, posY, posZ), dimension);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffFactionArmoryRemove', id);
    });
    view.on('editWeapons', (id) => {
        alt.emitServer('StaffFactionsArmorysWeaponsShow', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffFactionsArmoriesWeapons', (update, htmlFactionsArmoriesWeapons, factionArmoryId) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionsarmoriesweapons.html')
            view.emit('loaded', htmlFactionsArmoriesWeapons);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionsarmoriesweapons.html'));
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

alt.onServer('StaffVehicles', (update, vehiclesHtml) => {
    if (update) {
        if (view?.url == 'http://resource/staff/vehicles.html')
            view.emit('loaded', vehiclesHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/vehicles.html'));
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

alt.onServer('StaffInfos', (update, infosHtml) => {
    if (update) {
        if (view?.url == 'http://resource/staff/infos.html')
            view.emit('loaded', infosHtml);

        return;
    }

    setView(new alt.WebView('http://resource/staff/infos.html'));
    view.on('load', () => {
        view.emit('loaded', infosHtml);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffInfoGoto', id);
    });
    view.on('save', (days, message) => {
        alt.emitServer('StaffInfoSave', days, message);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffInfoRemove', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffFactionsDrugsHouses', (update, htmlFactionsDrugsHouses) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionsdrugshouses.html')
            view.emit('loaded', htmlFactionsDrugsHouses);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionsdrugshouses.html'));
    view.on('load', () => {
        view.emit('loaded', htmlFactionsDrugsHouses);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffFactionDrugHouseGoto', id);
    });
    view.on('save', (id, factionId, posX, posY, posZ, dimension) => {
        alt.emitServer('StaffFactionDrugHouseSave', id, factionId, new alt.Vector3(posX, posY, posZ), dimension);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffFactionDrugHouseRemove', id);
    });
    view.on('editItems', (id) => {
        alt.emitServer('StaffFactionsDrugsHousesItemsShow', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffFactionsDrugsHousesItems', (update, htmlFactionsDrugsHousesItems, drugHouseId) => {
    if (update) {
        if (view?.url == 'http://resource/staff/factionsdrugshousesitems.html')
            view.emit('loaded', htmlFactionsDrugsHousesItems);

        return;
    }

    setView(new alt.WebView('http://resource/staff/factionsdrugshousesitems.html'));
    view.on('load', () => {
        view.emit('loaded', htmlFactionsDrugsHousesItems);
    });
    view.on('closeView', closeView);
    view.on('save', (drugHouseItemId, item, quantity) => {
        alt.emitServer('StaffFactionDrugHouseItemSave', drugHouseItemId, drugHouseId, item, quantity);
    });
    view.on('remove', (factionArmoryWeaponId) => {
        alt.emitServer('StaffFactionDrugHouseItemRemove', factionArmoryWeaponId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffCrackDens', (update, htmlCrackDens) => {
    if (update) {
        if (view?.url == 'http://resource/staff/crackdens.html')
            view.emit('loaded', htmlCrackDens);

        return;
    }

    setView(new alt.WebView('http://resource/staff/crackdens.html'));
    view.on('load', () => {
        view.emit('loaded', htmlCrackDens);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffCrackDenGoto', id);
    });
    view.on('save', (id, posX, posY, posZ, dimension, onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours) => {
        alt.emitServer('StaffCrackDenSave', id, new alt.Vector3(posX, posY, posZ), dimension, 
            onlinePoliceOfficers, cooldownQuantityLimit, cooldownHours);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffCrackDenRemove', id);
    });
    view.on('editItems', (id) => {
        alt.emitServer('StaffCrackDensItemsShow', id);
    });
    view.on('revokeCooldown', (id) => {
        alt.emitServer('StaffCrackDenRevokeCooldown', id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('StaffCrackDensItems', (update, htmlCrackDensItems, crackDenId) => {
    if (update) {
        if (view?.url == 'http://resource/staff/crackdensitems.html')
            view.emit('loaded', htmlCrackDensItems);

        return;
    }

    setView(new alt.WebView('http://resource/staff/crackdensitems.html'));
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

alt.onServer('StaffTruckerLocations', (update, htmlTruckerLocations) => {
    if (update) {
        if (view?.url == 'http://resource/staff/truckerlocations.html')
            view.emit('loaded', htmlTruckerLocations);

        return;
    }

    setView(new alt.WebView('http://resource/staff/truckerlocations.html'));
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

alt.onServer('StaffTruckerLocationsDeliveries', (update, htmlTruckerLocationsDeliveries, truckerLocationId) => {
    if (update) {
        if (view?.url == 'http://resource/staff/truckerlocationsdeliveries.html')
            view.emit('loaded', htmlTruckerLocationsDeliveries);

        return;
    }

    setView(new alt.WebView('http://resource/staff/truckerlocationsdeliveries.html'));
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

alt.onServer('StaffFurnitures', (update, furnituresHTML) => {
    if (update) {
        if (view?.url == 'http://resource/staff/furnitures.html')
            view.emit('loaded', furnituresHTML);

        return;
    }

    setView(new alt.WebView('http://resource/staff/furnitures.html'));
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

alt.onServer('StaffAnimations', (update, animationsHTML) => {
    if (update) {
        if (view?.url == 'http://resource/staff/animations.html')
            view.emit('loaded', animationsHTML);

        return;
    }

    setView(new alt.WebView('http://resource/staff/animations.html'));
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

alt.onServer('StaffCompanies', (update, companiesHTML) => {
    if (update) {
        if (view?.url == 'http://resource/staff/companies.html')
            view.emit('loaded', companiesHTML);

        return;
    }

    setView(new alt.WebView('http://resource/staff/companies.html'));
    view.on('load', () => {
        view.emit('loaded', companiesHTML);
    });
    view.on('closeView', closeView);
    view.on('goto', (id) => {
        alt.emitServer('StaffCompanyGoto', id);
    });
    view.on('save', (id, name, posX, posY, posZ, weekRentValue) => {
        alt.emitServer('StaffCompanySave', id, name, new alt.Vector3(posX, posY, posZ), weekRentValue);
    });
    view.on('remove', (id) => {
        alt.emitServer('StaffCompanyRemove', id);
    });
    view.on('removeOwner', (id) => {
        alt.emitServer('StaffCompanyRemoveOwner', id);
    });
    view.on('getPosition', () => {
        view.emit('setPosition', alt.Player.local.pos.x, alt.Player.local.pos.y, alt.Player.local.pos.z);
    });
    view.focus();
    toggleView(true);
});