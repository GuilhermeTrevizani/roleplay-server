import * as alt from 'alt-client';
import * as native from 'natives';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('ShowFaction', (htmlRanks, htmlMembers, ranksJSON, flagsJson, factionJson, government, playerFactionFlags, isLeader) => {
    setView(new alt.WebView('http://resource/faction/faction.html'));
    view.on('load', () => {
        view.emit('init', JSON.parse(flagsJson), JSON.parse(factionJson), government);
        view.emit('updateMembers', htmlMembers, JSON.parse(playerFactionFlags), isLeader);
        view.emit('updateRanks', htmlRanks, JSON.parse(ranksJSON));
    });
    view.on('closeView', closeView);
    view.on('saveRank', (factionId, factionRankId, name) => {
        alt.emitServer('FactionRankSave', factionId, factionRankId, name);
    });
    view.on('removeRank', (factionId, factionRankId) => {
        alt.emitServer('FactionRankRemove', factionId, factionRankId);
    });
    view.on('orderRank', (factionId, ranksJSON) => {
        alt.emitServer('FactionRankOrder', factionId, ranksJSON);
    });
    view.on('inviteMember', (factionId, characterId) => {
        alt.emitServer('FactionMemberInvite', factionId, characterId);
    });
    view.on('saveMember', (factionId, characterId, factionRankId, badge, flags) => {
        alt.emitServer('FactionMemberSave', factionId, characterId, factionRankId, badge, flags);
    });
    view.on('removeMember', (factionId, characterId) => {
        alt.emitServer('FactionMemberRemove', factionId, characterId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('ShowFactionUpdateRanks', (htmlRanks, ranksJSON) => {
    view.emit('updateRanks', htmlRanks, JSON.parse(ranksJSON));
});

alt.onServer('ShowFactionUpdateMembers', (htmlMembers, playerFactionFlags, isLeader) => {
    view.emit('updateMembers', htmlMembers, JSON.parse(playerFactionFlags), isLeader);
});

alt.onServer('ShowFactionArmory', (update, weaponsHtml, factionJSON, government) => {
    if (update) {
        if (view?.url == 'http://resource/faction/factionarmory.html')
            view.emit('loaded', weaponsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/faction/factionarmory.html'));
    view.on('load', () => {
        view.emit('loaded', weaponsHtml);
        view.emit('init', JSON.parse(factionJSON), government);
    });
    view.on('closeView', closeView);
    view.on('equipWeapon', (factionArmoryWeaponId) => {
        alt.emitServer('FactionArmoryEquipWeapon', factionArmoryWeaponId);
    });
    view.on('giveBack', () => {
        alt.emitServer('FactionArmoryGiveBack');
    });
    view.on('equipArmor', () => {
        alt.emitServer('FactionArmoryEquipArmor');
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:SpawnarVeiculosFaccao', (ponto, faccao, veiculos) => {
    setView(new alt.WebView('http://resource/faction/spawnveiculos.html'));
    view.on('load', () => {
        view.emit('abrirSpawnVeiculos', faccao, veiculos);
    });
    view.on('closeView', closeView);
    view.on('spawnarVeiculo', (codigo) => {
        alt.emitServer('SpawnarVeiculoFaccao', ponto, codigo);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AbrirMDC', (tipoFaccao, nomeFaccao, ligacoes911, apb, bolo, unidades, relatoriosPendentes) => {
    setView(new alt.WebView('http://resource/faction/mdc.html'));
    view.on('load', () => {
        view.emit('abrirMDC', tipoFaccao, nomeFaccao, ligacoes911, apb, bolo, unidades, relatoriosPendentes);
    });
    view.on('closeView', closeView);
    view.on('pesquisarPessoa', (pesquisa) => {
        alt.emitServer('MDCPesquisarPessoa', pesquisa);
    });
    view.on('pesquisarVeiculo', (pesquisa) => {
        alt.emitServer('MDCPesquisarVeiculo', pesquisa);
    });
    view.on('pesquisarPropriedade', (pesquisa) => {
        alt.emitServer('MDCPesquisarPropriedade', pesquisa);
    });
    view.on('rastrearVeiculo', (codigo) => {
        alt.emitServer('MDCRastrearVeiculo', codigo);
    });
    view.on('adicionarBOLO', (tipo, codigo, motivo, pesquisa) => {
        alt.emitServer('MDCAdicionarBOLO', tipo, codigo, motivo, pesquisa);
    });
    view.on('removerBOLO', (codigo, pesquisa) => {
        alt.emitServer('MDCRemoverBOLO', codigo, pesquisa);
    });
    view.on('rastrear911', (codigo) => {
        alt.emitServer('MDCRastrear911', codigo);
    });
    view.on('multar', (codigo, nome, valor, motivo) => {
        alt.emitServer('MDCMultar', codigo, nome, valor, motivo);
    });
    view.on('revogarLicenca', (codigo) => {
        alt.emitServer('MDCRevogarLicencaMotorista', codigo);
    });
    view.on('adicionarUnidade', (unidade, numeracao, plate, parceiros) => {
        alt.emitServer('MDCAdicionarUnidade', unidade, numeracao, plate, parceiros);
    });
    view.on('encerrarUnidade', (codigo) => {
        alt.emitServer('MDCEncerrarUnidade', codigo);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AtualizarMDC', (botao, div, html) => {
    if (view?.url == 'http://resource/faction/mdc.html')
        view.emit('atualizarMDC', botao, div, html);
});

alt.onServer('PoliceConfiscationShow', (items) => {
    setView(new alt.WebView('http://resource/faction/confiscation.html'));
    view.on('load', () => {
        view.emit('loaded', JSON.parse(items));
    });
    view.on('closeView', closeView);
    view.on('save', (characterName, confiscationItems) => {
        alt.emitServer('ConfiscationSave', characterName, confiscationItems);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('ShowFactionDrugHouse', (update, itemsHtml, factionJSON) => {
    if (update) {
        if (view?.url == 'http://resource/faction/factiondrughouse.html')
            view.emit('loaded', itemsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/faction/factiondrughouse.html'));
    view.on('load', () => {
        view.emit('loaded', itemsHtml);
        view.emit('init', JSON.parse(factionJSON));
    });
    view.on('closeView', closeView);
    view.on('buyItem', (factionDrugHouseItemId, quantity) => {
        alt.emitServer('FactionDrugHouseBuyItem', factionDrugHouseItemId, quantity);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('ShowCrackDen', (update, itemsHtml, crackDenId) => {
    if (update) {
        if (view?.url == 'http://resource/faction/crackden.html')
            view.emit('loaded', itemsHtml);

        return;
    }

    setView(new alt.WebView('http://resource/faction/crackden.html'));
    view.on('load', () => {
        view.emit('loaded', itemsHtml);
    });
    view.on('closeView', closeView);
    view.on('sellItem', (crackDenItemId, quantity) => {
        alt.emitServer('CrackDenSellItem', crackDenItemId, quantity);
    });
    view.on('showSells', () => {
        alt.emitServer('CrackDenShowSells', crackDenId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('TruckerLocations', (htmlTruckerLocations) => {
    setView(new alt.WebView('http://resource/faction/truckerlocations.html'));
    view.on('load', () => {
        view.emit('loaded', htmlTruckerLocations);
    });
    view.on('closeView', closeView);
    view.on('track', (posX, posZ) => {
        native.setNewWaypoint(posX, posZ);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Boombox', (itemId, url, volume) => {
    setView(new alt.WebView('http://resource/faction/boombox.html'));
    view.on('load', () => {
        view.emit('loaded', 'Boombox', url, volume);
    });
    view.on('closeView', closeView);
    view.on('confirm', (url, volume) => {
        alt.emitServer('ConfirmBoombox', itemId, url, volume);
    });
    view.on('turnOff', () => {
        alt.emitServer('TurnOffBoombox', itemId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('XMR', (vehicleId, url, volume) => {
    setView(new alt.WebView('http://resource/faction/boombox.html'));
    view.on('load', () => {
        view.emit('loaded', 'XMR', url, volume);
    });
    view.on('closeView', closeView);
    view.on('confirm', (url, volume) => {
        alt.emitServer('ConfirmXMR', vehicleId, url, volume);
    });
    view.on('turnOff', () => {
        alt.emitServer('TurnOffXMR', vehicleId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('PropertyFurnitures', (propertyId, furnituresHTML) => {
    if (view?.url == 'http://resource/faction/propertyfurnitures.html') {
        view.emit('showHTML', propertyId, furnituresHTML);
        return;
    }

    setView(new alt.WebView('http://resource/faction/propertyfurnitures.html'));
    view.on('load', () => {
        view.emit('showHTML', propertyId, furnituresHTML);
    });
    view.on('closeView', closeView);
    view.on('buy', () => {
        alt.emitServer('BuyPropertyFurniture', propertyId);
    });
    view.on('edit', (id) => {
        alt.emitServer('EditPropertyFurniture', propertyId, id);
    });
    view.on('remove', (id) => {
        alt.emitServer('RemovePropertyFurniture', propertyId, id);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('BuyPropertyFurnitures', (propertyId, furnituresHTML) => {
    setView(new alt.WebView('http://resource/faction/furnitures.html'));
    view.on('load', () => {
        view.emit('showHTML', propertyId, furnituresHTML);
    });
    view.on('closeView', closeView);
    view.on('buy', (propertyFurnitureId) => {
        alt.emitServer('SelectBuyPropertyFurniture', propertyId, propertyFurnitureId);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Companies', (update, companiesHTML) => {
    if (update) {
        if (view?.url == 'http://resource/faction/companies.html')
            view.emit('loaded', companiesHTML);

        return;
    }

    setView(new alt.WebView('http://resource/faction/companies.html'));
    view.on('load', () => {
        view.emit('loaded', companiesHTML);
    });
    view.on('closeView', closeView);
    view.on('save', (id, color, blipType, blipColor) => {
        alt.emitServer('CompanySave', id, color, blipType, blipColor);
    });
    view.on('employees', (id) => {
        alt.emitServer('CompanyEmployees', id);
    });
    view.on('openClose', (id) => {
        alt.emitServer('CompanyOpenClose', id);
    });
    view.on('announce', (id, message) => {
        alt.emitServer('CompanyAnnounce', id, message);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('CompanyCharacters', (update, charactersHtml, flagsJSON, isOwner, companyId, companyName, companyFlagsJSON) => {
    if (update) {
        if (view?.url == 'http://resource/faction/companiescharacters.html')
            view.emit('loaded', charactersHtml, JSON.parse(flagsJSON), isOwner);

        return;
    }

    setView(new alt.WebView('http://resource/faction/companiescharacters.html'));
    view.on('load', () => {
        view.emit('loaded', charactersHtml, JSON.parse(flagsJSON), isOwner);
        view.emit('loadCompany', companyId, companyName, JSON.parse(companyFlagsJSON));
    });
    view.on('closeView', closeView);
    view.on('invite', (id) => {
        alt.emitServer('CompanyCharacterInvite', companyId, id);
    });
    view.on('save', (id, flagsJSON) => {
        alt.emitServer('CompanyCharacterSave', companyId, id, flagsJSON);
    });
    view.on('remove', (id) => {
        alt.emitServer('CompanyCharacterRemove', companyId, id);
    });
    view.focus();
    toggleView(true);
});