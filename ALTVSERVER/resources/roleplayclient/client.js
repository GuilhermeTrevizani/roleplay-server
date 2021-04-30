import * as alt from 'alt';
import * as native from 'natives';
import { showCursor } from '/helpers/cursor.js';
import { activateChat, toggleInput } from '/chat/client.mjs';
import { playAnimation } from '/helpers/animation.js';
import { drawText2d } from '/helpers/text.js';
import * as blips from '/helpers/blip.js';
import * as systemsInteriors from '/helpers/interiors.js';
import * as nametags from '/helpers/nametags.js';
//import * as enterVehicles from '/helpers/enterVehicles.js';
import * as charCreator from '/charcreator/editor.js';
import * as vehtags from '/helpers/vehtags.js';
import * as clothes from '/clothes/editor.js';
import * as markers from '/helpers/markers.js';
import Fingerpointing from '/helpers/fingerpointing.js';

function toggleView(show) {
    showCursor(show);
    alt.toggleGameControls(!show);
    toggleInput(!show);
}

function closeView() {
    if (view != null)
        view.destroy();
    view = null;
    toggleView(false);
}

alt.Vector3.prototype.distance = function distance(vector) {
    return Math.sqrt(Math.pow(this.x - vector.x, 2) + Math.pow(this.y - vector.y, 2) + Math.pow(this.z - vector.z, 2));
};

const weaponSlots = new Set([100416529, 101631238, 125959754,126349499, 137902532, 171789620, 177293209, 205991906, 317205821, 324215364, 419712736, 453432689,  487013001, 584646201, 600439132, 615608432, 736523883, 741814745, 883325847, 911657153, 940833800, 961495388,  984333226, 1119849093, 1141786504, 1198256469, 1198879012, 1233104067, 1305664598, 1317494643, 1432025498, 1593441988,  1627465347, 1649403952,  1672152130, 1737195953, 1785463520, 1834241177, 2017895192, 2024373456, 2132975508, 2138347493, 2144741730, 2210333304, 2227010557, 2228681469, 2285322324, 2343591895, 2460120199, 2481070269, 2484171525, 2508868239, 2526821735, 2548703416, 2578377531, 2578778090, 2634544996, 2640438543, 2694266206, 2726580491, 2828843422, 2874559379, 2937143193, 2939590305, 2982836145, 3056410471, 3125143736, 3173288789, 3218215474, 3219281620, 3220176749, 3231910285, 3249783761, 3342088282, 3415619887, 3441901897, 3523564046, 3638508604, 3675956304, 3686625920, 3696079510, 3713923289, 3756226112, 3800352039, 4019527611, 4024951519, 4191993645, 4192643659, 4208062921, 4222310262, 4256991824]);
const fModel = alt.hash('mp_f_freemode_01');
const mModel = alt.hash(`mp_m_freemode_01`);
const bombasCombustivel = ['prop_gas_pump_1a', 'prop_gas_pump_1b', 'prop_gas_pump_1c', 'prop_gas_pump_1d', 'prop_gas_pump_old2', 'prop_gas_pump_old3', 'prop_vintage_pump'];
const atms = ['prop_atm_01', 'prop_atm_02', 'prop_atm_03', 'prop_fleeca_atm'];

let player = alt.Player.local;
let view, playersView = null;
let playersViewLoading, isMetric = false;
let areaName, zoneName, directionName = '';
let intervalSpec = null;
let objetoEmMaos = null;
let pointing = new Fingerpointing();

native.doorControl(-1246222793, 256.3116, 220.6579, 106.4296, true, 0.0, 0.0, 0.0); // Banco Principal
native.doorControl(1956494919, 237.7704, 227.87, 106.426, true, 0.0, 0.0, 0.0); // Banco Principal

native.doorControl(631614199, 461.8065, -994.4086, 25.06443, true, 0.0, 0.0, 0.0); // Mission Row Police Station Cell Door 1
native.doorControl(631614199, 461.8065, -997.6583, 25.06443, true, 0.0, 0.0, 0.0); // Mission Row Police Station Cell Door 2
native.doorControl(631614199, 461.8065, -1001.302, 25.06443, true, 0.0, 0.0, 0.0); // Mission Row Police Station Cell Door 3

native.doorControl(520341586, -14.86892, -1441.182, 31.19323, true, 0.0, 0.0, 0.0); // Franklin House Enter Door
native.doorControl(703855057, -25.2784, -1431.061, 30.83955, true, 0.0, 0.0, 0.0); // Franklin House Garage Door

native.doorControl(-8873588, 842.7685, -1024.539, 28.34478, true, 0.0, 0.0, 0.0); // Ammu Nation Vespucci Boulevard Doors Right
native.doorControl(97297972, 845.3694, -1024.539, 28.34478, true, 0.0, 0.0, 0.0); // Ammu Nation Vespucci Boulevard Doors Left
native.doorControl(-8873588, -662.6415, -944.3256, 21.97915, true, 0.0, 0.0, 0.0); // Ammu Nation Lindsay Circus Doors Doors Right
native.doorControl(97297972, -665.2424, -944.3256, 21.97915, true, 0.0, 0.0, 0.0); // Ammu Nation Lindsay Circus Doors Doors Left
native.doorControl(-8873588, 810.5769, -2148.27, 29.76892, true, 0.0, 0.0, 0.0); // Ammu Nation Cypress Flats Doors Right
native.doorControl(97297972, 813.1779, -2148.27, 29.76892, true, 0.0, 0.0, 0.0); // Ammu Nation Cypress Flats Doors Left
native.doorControl(-8873588, 18.572, -1115.495, 29.94694, true, 0.0, 0.0, 0.0); // Ammu Nation Popular Street Doors Right
native.doorControl(97297972, 16.12787, -1114.606, 29.94694, true, 0.0, 0.0, 0.0); // Ammu Nation Popular Street Doors Left
native.doorControl(452874391, 6.81789, -1098.209, 29.94685, true, 0.0, 0.0, 0.0); // Ammu Nation Adams Apple Boulevard
native.doorControl(-8873588, 243.8379, -46.52324, 70.09098, true, 0.0, 0.0, 0.0); // Ammu Nation Vinewood Plaza Doors Right
native.doorControl(97297972, 244.7275, -44.07911, 70.09098, true, 0.0, 0.0, 0.0); // Ammu Nation Vinewood Plaza Doors Left

native.doorControl(741314661, 1844.72, 2608.49, 46.0, true, 0.0, 0.0, 0.0); // Bolingbroke Penitentiary Main Enter First Door
native.doorControl(741314661, 1818.252, 2608.384, 46.0, true, 0.0, 0.0, 0.0); // Bolingbroke Penitentiary Main Enter Second Door
native.doorControl(741314661, 1795.98, 2616.696, 45.565, true, 0.0, 0.0, 0.0); // Bolingbroke Penitentiary Main Enter Third Door

native.doorControl(270330101, 723.116, -1088.831, 23.23201, false, 0.0, 0.0, 0.0); // Los Santos Customs Popular Street Door
native.doorControl(-550347177, -356.0905, -134.7714, 40.01295, false, 0.0, 0.0, 0.0); // Los Santos Customs Carcer Way Door       
native.doorControl(-550347177, -1145.898, -1991.144, 14.18357, false, 0.0, 0.0, 0.0); // Los Santos Customs Greenwich Parkway Door
native.doorControl(-822900180, 1174.656, 2644.159, 40.50673, false, 0.0, 0.0, 0.0); // Los Santos Customs Route 68 Doors        
native.doorControl(-822900180, 1182.307, 2644.166, 40.50784, false, 0.0, 0.0, 0.0); // Los Santos Customs Route 68 Doors       
native.doorControl(1335311341, 1187.202, 2644.95, 38.55176, false, 0.0, 0.0, 0.0); // Los Santos Customs Route 68 Office Door  
native.doorControl(1544229216, 1182.646, 2641.182, 39.31031, false, 0.0, 0.0, 0.0); // Los Santos Customs Route 68 Office Door  

native.doorControl(-822900180, 114.3135, 6623.233, 32.67305, false, 0.0, 0.0, 0.0); // Beekers Garage Paleto Bay Doors       
native.doorControl(-822900180, 108.8502, 6617.877, 32.67305, false, 0.0, 0.0, 0.0); // Beekers Garage Paleto Bay Doors       
native.doorControl(1335311341, 105.1518, 6614.655, 32.58521, false, 0.0, 0.0, 0.0); // Beekers Garage Paleto Bay Office Door
native.doorControl(1544229216, 105.7772, 6620.532, 33.34266, false, 0.0, 0.0, 0.0); // Beekers Garage Paleto Bay Interior Door	

native.doorControl(159994461, -816.716, 179.098, 72.82738, true, 0.0, 0.0, 0.0); // Michael Entrada Left
native.doorControl(-1686014385, -816.1068, 177.5109, 72.82738, true, 0.0, 0.0, 0.0); // Michael Entrada Right
native.doorControl(30769481, -815.2816, 185.975, 72.99993, true, 0.0, 0.0, 0.0); // Michael Garage
native.doorControl(-1454760130, -793.3943, 180.5075, 73.04045, true, 0.0, 0.0, 0.0); // Michael Left Atrás
native.doorControl(1245831483, -794.1853, 182.568, 73.04045, true, 0.0, 0.0, 0.0); // Michael Right Atrás
native.doorControl(-1454760130, -796.5657, 177.2214, 73.04045, true, 0.0, 0.0, 0.0); // Michael Left Atrás
native.doorControl(1245831483, -794.5051, 178.0124, 73.04045, true, 0.0, 0.0, 0.0); // Michael Right Atrás

native.doorControl(132154435, 1972.769, 3815.366, 33.66326, true, 0.0, 0.0, 0.0); // Trevor
native.doorControl(-607040053, -1149.709, -1521.088, 10.78267, true, 0.0, 0.0, 0.0); // Trevor Praia

native.doorControl(1145337974, 1273.815, -1720.697, 54.92143, true, 0.0, 0.0, 0.0); // Lester

native.doorControl(308207762, 7.51835, 539.5268, 176.1776, true, 0.0, 0.0, 0.0); // Mansão Fraklin Porta
native.doorControl(2052512905, 18.65038, 546.3401, 176.3448, true, 0.0, 0.0, 0.0); // Mansão Fraklin Garagem

native.requestModel(fModel);
native.requestModel(mModel);
native.displayHud(false);
native.displayRadar(false);
alt.toggleGameControls(false);
native.disablePlayerVehicleRewards(player.scriptID);
native.setPlayerHealthRechargeMultiplier(player.scriptID, 0.0);
native.pauseClock(true);
alt.emit('load:Interiors');
player.setMeta('chatting', false);
alt.setStat('shooting_ability', 100);
native.freezeEntityPosition(player.scriptID, true);

let cam = native.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', -436.0717, 1039.26, 372.1287, 0, 0, 0, 60, true, 0);
native.pointCamAtCoord(cam, 3.063985, 0.0, -170.8151);
native.setCamActive(cam, true);
native.renderScriptCams(true, false, 0, true, false);
let directions = [ { name: 'N', value: 0 }, { name: 'NO', value: 45 }, { name: 'O', value: 90 }, { name: 'SO', value: 135 }, { name: 'S', value: 180 }, { name: 'SE', value: 225 }, { name: 'L', value: 270 }, { name: 'ND', value: 315 }, { name: 'N', value: 360 }];

alt.setInterval(() => {
    native.invalidateIdleCam();
    native.invalidateVehicleIdleCam();
}, 20000);

alt.setInterval(() => {
    if (!player.hasSyncedMeta('nametag'))
        return;

    native.restorePlayerStamina(player.scriptID, 100);

    native.setWeaponDamageModifierThisFrame(2725352035, 0.2);

    native.setWeaponDamageModifierThisFrame(1737195953, 0.3);
    native.setWeaponDamageModifierThisFrame(3638508604, 0.3);
    native.setWeaponDamageModifierThisFrame(2227010557, 0.3);
    native.setWeaponDamageModifierThisFrame(1141786504, 0.3);
    native.setWeaponDamageModifierThisFrame(1317494643, 0.3);
    native.setWeaponDamageModifierThisFrame(419712736, 0.3);
    native.setWeaponDamageModifierThisFrame(2508868239, 0.3);
    native.setWeaponDamageModifierThisFrame(2343591895, 0.3);

    native.setWeaponDamageModifierThisFrame(2460120199, 0.4);
    native.setWeaponDamageModifierThisFrame(2460120199, 0.4);
    native.setWeaponDamageModifierThisFrame(4192643659, 0.4);
    native.setWeaponDamageModifierThisFrame(3441901897, 0.4);
    native.setWeaponDamageModifierThisFrame(3713923289, 0.4);
    native.setWeaponDamageModifierThisFrame(940833800, 0.4);
    native.setWeaponDamageModifierThisFrame(3756226112, 0.4);

    native.setWeaponDamageModifierThisFrame(2578778090, 0.4);
    native.setWeaponDamageModifierThisFrame(3800352039, 0.4);
    native.setWeaponDamageModifierThisFrame(2640438543, 0.4);
    native.setWeaponDamageModifierThisFrame(4019527611, 0.4);
    native.setWeaponDamageModifierThisFrame(984333226, 0.4);
    native.setWeaponDamageModifierThisFrame(2828843422, 0.4);
    native.setWeaponDamageModifierThisFrame(487013001, 0.4);
    native.setWeaponDamageModifierThisFrame(1432025498, 0.4);
    native.setWeaponDamageModifierThisFrame(2017895192, 0.4);
    native.setWeaponDamageModifierThisFrame(317205821, 0.4);
    
    isMetric = native.getProfileSetting(227) == 1;

    let obj = native.getStreetNameAtCoord(player.pos.x, player.pos.y, player.pos.z, 0, 0);
    areaName = native.getStreetNameFromHashKey(obj[1]);

    zoneName = native.getStreetNameFromHashKey(obj[2]);
    if (zoneName != '')
        zoneName += ', ';

    zoneName += native.getLabelText(native.getNameOfZone(player.pos.x, player.pos.y, player.pos.z));

    let direction = native.getEntityHeading(player.scriptID);
    if (player.vehicle)
        direction = native.getEntityHeading(player.vehicle.scriptID);
    directions.forEach(x => {
        if (Math.abs(direction - x.value) < 22.5) 
            directionName = x.name;
    });

    let weapons = '';
    weaponSlots.forEach(weaponSlot => {
        var tint = native.getPedWeaponTintIndex(player.scriptID, weaponSlot);
        if (tint >= 0)
            weapons += `${weaponSlot}|${native.getAmmoInPedWeapon(player.scriptID, weaponSlot)}|${tint};`;
    });

    alt.emitServer('AtualizarInformacoes', areaName, zoneName, weapons);
}, 1000);

alt.everyTick(() => {
    if (!player.hasSyncedMeta('nametag'))
        return;

    native.hideHudComponentThisFrame(6);
    native.hideHudComponentThisFrame(7);
    native.hideHudComponentThisFrame(8);
    native.hideHudComponentThisFrame(9);

    native.setPedConfigFlag(player.scriptID, 429, true); // Do not start engine automatically 
    native.setPedConfigFlag(player.scriptID, 241, true); // PED_FLAG_DISABLE_STOPPING_VEH_ENGINE
    native.setPedConfigFlag(player.scriptID, 184, true); // PASSENGER SEAT TO DRIVER SEAT

    native.setPedHelmet(player.scriptID, false);
    native.setPedSuffersCriticalHits(player.scriptID, false);
    
    native.disableControlAction(0, 140, true); // Disable weapon knockout
    if (native.getPedConfigFlag(player.scriptID, 78, true) ||
        native.isPlayerFreeAiming(player) ||
        native.isControlJustPressed(1, 25)) {
            native.disableControlAction(0, 141, true);
            native.disableControlAction(0, 142, true);
            native.disableControlAction(0, 22, true); // Space Bar
    }

    let animationDic = player.getMeta('animation_dic');
    if (animationDic != '') {
        native.disableControlAction(0, 22, true); // Space Bar
        native.disablePlayerFiring(player.scriptID, true);
        native.disableControlAction(0, 25, true);

        let animationName = player.getMeta('animation_name');
        if (!native.isEntityPlayingAnim(player.scriptID, animationDic, animationName, 3)) {
            if (animationDic == 'mp_arresting' && animationName == 'idle')
                playAnimation(player, 'mp_arresting', 'idle', -1, 49);
            else 
                player.setMeta('animation_dic', '');
        }
    }

    if (player.getSyncedMeta('ferido') != 0)
        native.setPedToRagdoll(player.scriptID, -1, -1, 0, 0, 0, 0);

    drawText2d('~w~Segunda Vida ~s~Roleplay', 0.8, 0.92, 0.5, 4, 174, 106, 178, 180, true, true, 2);

    if (player.hasMeta('f7'))
        return;

    if (areaName != '' && zoneName != '' && !native.isRadarHidden()) {
        drawText2d(directionName, 0.16, 0.92, 1.0, 4, 255, 255, 255, 200, true, true, 1);
        drawText2d(areaName, 0.185, 0.92, 0.5, 4, 174, 106, 178, 200, true, true, 1);
        drawText2d(zoneName, 0.185, 0.95, 0.4, 4, 255, 255, 255, 200, true, true, 1);
    }

    if (player.vehicle) {
        let driver = native.getPedInVehicleSeat(player.vehicle.scriptID, -1, false);
        if (player.scriptID == driver)  {
            drawText2d(`${(native.getEntitySpeed(player.vehicle.scriptID) * (isMetric ? 3.6 : 2.236936)).toFixed(0)} ${(isMetric) ? 'KM/H' : 'MPH'}`, 0.16, 0.90,
                0.4, 4, 255, 255, 255, 200, true, true, 1);

            drawText2d(player.vehicle.getSyncedMeta('combustivel'), 0.16, 0.88, 0.4, 4, 255, 255, 255, 200, true, true, 1);
        }
    }

    drawText2d(player.getSyncedMeta('dinheiro'), 0.8, 0.05, 0.8, 4, 115, 186, 131, 200, true, true, 2);
});

alt.on('keyup', (key) => {
    if (!player.hasSyncedMeta('nametag') || player.getMeta('chatting') || alt.isMenuOpen())
        return;

    if (key == 113) { // F2
        if (view != null || playersViewLoading)
            return;

        playersViewLoading = true;
        if (playersView != null) {
            playersView.destroy();
            playersView = null;
            toggleView(false);
            playersViewLoading = false;
            return;
        }

        alt.emitServer('ListarPlayers');
    } else if (key == 90) { // Z
        if (player.vehicle) {
            let driver = native.getPedInVehicleSeat(player.vehicle.scriptID, -1, false);
            if (player.scriptID == driver)  {
                let hasMutedSirens = !player.vehicle.getStreamSyncedMeta('hasMutedSirens');
                alt.emitServer('SetVehicleMeta', player.vehicle, 'hasMutedSirens', hasMutedSirens);
                native.setVehicleHasMutedSirens(player.vehicle.scriptID, hasMutedSirens);
            }
        }
    } else if (key == 118) { // F7
        (player.hasMeta('f7')) ? player.deleteMeta('f7') : player.setMeta('f7', true); 
        let f7 = player.hasMeta('f7');
        player.setMeta('STOP_DRAWS', f7);
        activateChat(!f7);
        native.displayHud(!f7);
        native.displayRadar(!f7);
    } else if (key == 66) { // B
        if (!player.vehicle && !player.getSyncedMeta('animation') && view == null) {
            if (pointing.active)
                pointing.stop();
            else 
                pointing.start();
        }
    } else if (key == 114) { // F3
    } else if (key == 115) { // F4
    }
});

alt.onServer('Server:ListarPlayers', (nomeServidor, players, rodape) => {
    if (playersView != null)
        playersView.destroy();

    playersView = new alt.WebView('http://resource/players/players.html');
    playersView.on('load', () => {
        playersView.emit('showPlayers', nomeServidor, players, rodape);
        playersViewLoading = false;
    });
    playersView.focus();
    toggleView(true);
});

alt.onServer('Server:BaseHTML', (html) => {
    if (view != null)
        view.destroy();
    view = new alt.WebView('http://resource/login/base.html');
    view.on('load', () => {
        view.emit('showHTML', html);
    });
    view.on('closeView', closeView);
    view.focus();
    toggleView(true);
});

alt.onServer('Server:Login', (usuario) => {
    if (view != null) 
        view.destroy();
    else
        toggleView(true);

    view = new alt.WebView('http://resource/login/login.html');
    view.on('load', () => {
        view.emit('showHTML', usuario);
    });
    view.on('entrarUsuario', (usuario, senha) => {
        alt.emitServer('EntrarUsuario', usuario, senha);
    });
    view.on('esqueciMinhaSenha', () => {
        view.destroy();
        view = new alt.WebView('http://resource/login/esqueciminhasenha.html');  
        view.on('voltarLogin', () => {
            alt.emitServer('OnPlayerConnectLogin');
        });
        view.on('confirmar', (usuario, email) => {
            alt.emitServer('EnviarEmailAlterarSenha', usuario, email);
        });
        view.focus();
    });
    view.on('registrarUsuario', () => {
        alt.emitServer('ExibirPerguntas');
    });
    view.focus();
});

alt.onServer('Server:ExibirPerguntas', (perguntas) => {
    view.destroy();
    view = new alt.WebView('http://resource/login/perguntas.html');  
    view.on('load', () => {
        view.emit('showHTML', perguntas);
    });
    view.on('voltarLogin', () => {
        alt.emitServer('OnPlayerConnectLogin');
    });
    view.on('confirmar', (respostas) => {
        alt.emitServer('ValidarPerguntas', respostas);
    });
    view.focus();
});

alt.onServer('Server:RegistrarUsuario', () => {
    view.destroy();
    view = new alt.WebView('http://resource/login/registro.html');  
    view.on('voltarLogin', () => {
        alt.emitServer('OnPlayerConnectLogin');
    });
    view.on('registrarUsuario', (usuario, email, senha, senha2) => {
        alt.emitServer('RegistrarUsuario', usuario, email, senha, senha2);
    });
    view.focus();
});

alt.onServer('Server:RequestIpl', (ipl) => {
    native.requestIpl(ipl);
});

alt.onServer('Server:RemoveIpl', (ipl) => {
    native.removeIpl(ipl);
});

alt.onServer('Server:SetWaypoint', (x, y) => {
    native.setNewWaypoint(x, y);
});

alt.onServer('Server:PlayAnim', (dict, name, flag) => {
    playAnimation(player, dict, name, -1, flag);
});

alt.onServer('Server:StopAnim', () => {
    native.clearPedTasks(player.scriptID);
});

alt.onServer('Server:ConfirmacaoRegistro', (nome, email) => {
    if (view != null)
        view.destroy();
    view = new alt.WebView('http://resource/login/confirmacaoregistro.html');
    view.on('load', () => {
        view.emit('showHTML', nome, email);
    });
    view.on('enviarEmail', (email) => {
        alt.emitServer('EnviarEmailConfirmacao', email);
    });
    view.on('validarToken', (token) => {
        alt.emitServer('ValidarTokenConfirmacao', token);
    });
    view.focus();
});

alt.onServer('Server:ListarPersonagens', (nome, personagens, slots) => {
    view.destroy();
    view = new alt.WebView('http://resource/login/personagens.html');
    view.on('load', () => {
        view.emit('showHTML', nome, personagens, slots);
    });
    view.on('selecionarPersonagem', (id, namechange) => {
        alt.emitServer('SelecionarPersonagem', id, namechange);
    });
    view.on('criarPersonagem', () => {
        criarPersonagem();
    });
    view.on('deletarPersonagem', (id) => {
        alt.emitServer('DeletarPersonagem', id);
    });
    view.on('atualizar', () => {
        alt.emitServer('ListarPersonagens');
    });
    view.on('alterarEmail', () => {
        view.destroy();
        view = new alt.WebView('http://resource/login/alteraremail.html');
        view.on('voltar', () => {
            alt.emitServer('ListarPersonagens');
        });
        view.on('alterar', (email) => {
            alt.emitServer('AlterarEmail', email);
        });
        view.focus();
    });
    view.on('alterarSenha', () => {
        view.destroy();
        view = new alt.WebView('http://resource/login/alterarsenha.html');
        view.on('voltar', () => {
            alt.emitServer('ListarPersonagens');
        });
        view.on('alterar', (senhaAntiga, novaSenha, novaSenha2) => {
            alt.emitServer('AlterarSenha', senhaAntiga, novaSenha, novaSenha2);
        });
        view.focus();
    });
    view.on('punicoesAdministrativas', () => {
        alt.emitServer('PunicoesAdministrativas');
    });
    view.focus();
});

alt.onServer('Server:PunicoesAdministrativas', (nome, data, punicoesAdministrativas) => {
    view.destroy();
    view = new alt.WebView('http://resource/login/punicoesadministrativas.html');
    view.on('load', () => {
        view.emit('showHTML', nome, data, punicoesAdministrativas);
    });
    view.on('voltar', () => {
        alt.emitServer('ListarPersonagens');
    });
    view.focus();
});

alt.onServer('Server:CriarPersonagem', criarPersonagem);
function criarPersonagem(codigo = 0, nome = '', sobrenome = '', sexo = '', dataNascimento = '', historia = '', motivoRejeicao = '', staffer = '') {
    view.destroy();
    view = new alt.WebView('http://resource/login/criarpersonagem.html');
    view.on('load', () => {
        view.emit('showHTML', nome, sobrenome, sexo, dataNascimento, historia, motivoRejeicao, staffer);
    });
    view.on('criarPersonagem', (nome, sobrenome, sexo, dataNascimento, historia) => {
        alt.emitServer('CriarPersonagem', codigo, nome, sobrenome, sexo, dataNascimento, historia);
    });
    view.on('voltar', () => {
        alt.emitServer('ListarPersonagens');
    });
    view.focus();
}

alt.onServer('Server:SelecionarPersonagem', (personalizacao, roupas, acessorios, roupa, tipo) => {
    let dataPersonalizacao = JSON.parse(personalizacao);
    let dataRoupas = JSON.parse(roupas);
    let dataAcessorios = JSON.parse(acessorios);

    if (view != null)
        view.destroy();

    view = null;
    toggleView(false);

    native.destroyAllCams(true);
    native.renderScriptCams(false, false, 0, false, false);
    
    native.setEntityInvincible(player.scriptID, false);
    native.freezeEntityPosition(player.scriptID, false);
    native.clearPedTasks(player.scriptID);
    alt.emit('character:Sync', dataPersonalizacao, false);
    alt.emit('character:SyncClothes', dataRoupas.filter(x => x.ID == roupa), dataAcessorios.filter(x => x.ID == roupa));

    if (tipo == 2) {
        activateChat(true);
        native.displayHud(true);
        native.displayRadar(true);
    } else if (tipo == 0) {
        alt.emit('character:Edit', dataPersonalizacao, 0);
    } else if (tipo == 1) {
        alt.emit('character:EditClothes', dataRoupas, dataAcessorios, dataPersonalizacao.sex);
    }
});

alt.onServer('Server:MostrarErro', (erro) => {
    view.emit('mostrarErro', erro);
});

alt.onServer('Server:MostrarSucesso', (mensagem) => {
    view.emit('mostrarSucesso', mensagem);
});

alt.onServer('Server:ComprarVeiculo', (titulo, tipo, veiculos) => {
    view = new alt.WebView('http://resource/concessionaria/concessionaria.html');
    view.on('load', () => {
        view.emit('showHTML', titulo, veiculos);
    });
    view.on('closeView', closeView);
    view.on('confirmarCompra', (veiculo, r1, g1, b1, r2, g2, b2) => {
        alt.emitServer('ComprarVeiculo', tipo, veiculo, r1, g1, b1, r2, g2, b2);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:ComprarConveniencia', (itens) => {
    view = new alt.WebView('http://resource/conveniencia/conveniencia.html');
    view.on('load', () => {
        view.emit('comprarConveniencia', itens);
    });
    view.on('closeView', closeView);
    view.on('confirmarCompra', (item) => {
        alt.emitServer('ComprarConveniencia', item);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AbrirCelular', (celular, contatos) => {
    view = new alt.WebView('http://resource/celular/celular.html');
    view.on('load', () => {
        view.emit('abrirCelular', celular, contatos);
    });
    view.on('closeView', closeView);
    view.on('adicionarContato', (nome, celular) => {
        alt.emitServer('AdicionarContatoCelular', nome, celular);
    });
    view.on('removerContato', (celular) => {
        alt.emitServer('RemoverContatoCelular', celular);
    });
    view.on('ligarContato', (celular) => {
        alt.emitServer('LigarContatoCelular', celular);
    });
    view.on('enviarLocalizacaoContato', (celular) => {
        alt.emitServer('EnviarLocalizacaoContatoCelular', celular);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AtualizarCelular', (celular, contatos) => {
    view.emit('abrirCelular', celular, contatos);
});

alt.onServer('Server:AbrirMultas', (multas) => {
    view = new alt.WebView('http://resource/multas/multas.html');
    view.on('load', () => {
        view.emit('abrirMultas', multas);
    });
    view.on('closeView', closeView);
    view.on('pagarMulta', (codigo) => {
        alt.emitServer('PagarMulta', codigo);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AbrirArmario', (codigo, faccao, armas, componentes, governo, policia, precoComponente) => {
    view = new alt.WebView('http://resource/faccao/armario.html');
    view.on('load', () => {
        view.emit('abrirArmario', faccao, armas, componentes, governo, policia, precoComponente);
    });
    view.on('closeView', closeView);
    view.on('equiparItem', (item) => {
        alt.emitServer('PegarItemArmario', codigo, item);
    });
    view.on('devolverItens', () => {
        alt.emitServer('DevolverItensArmario');
    });
    view.on('equiparColete', () => {
        alt.emitServer('EquiparColeteArmario');
    });
    view.on('equiparComponente', (arma, componente) => {
        if (native.getPedWeaponTintIndex(player.scriptID, arma) < 0) {
            alt.emit('chat:notify', 'Você não está com essa arma.', 'danger');
            return;
        }

        if (native.hasPedGotWeaponComponent(player.scriptID, arma, componente)) {
            alt.emit('chat:notify', 'Você já possui esse componente nessa arma.', 'danger');
            return;
        }

        alt.emitServer('PegarComponenteArmario', arma, componente);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AtualizarArmario', (itens, componentes) => {
    view.emit('atualizarArmario', itens, componentes);
});

alt.onServer('Server:CloseView', closeView);

alt.onServer('vehicle:setVehicleEngineOn', (vehicle, state) => {
    native.setVehicleEngineOn(vehicle.scriptID, state, false, true);
});

alt.onServer('alt:log', (msg) => {
    alt.log(msg);
});

alt.onServer('EntregarArma', (codigo, arma) => {
    alt.emitServer('EntregarArma', codigo, arma.toString(), native.getAmmoInPedWeapon(player.scriptID, parseInt(arma)));
});

alt.onServer('RemoveWeapon', (arma) => {
    native.setPedAmmo(player.scriptID, parseInt(arma), 0 , true);
    native.removeWeaponFromPed(player.scriptID, parseInt(arma));
});

alt.on('streamSyncedMetaChange', (entity, key, value, oldValue) => {
    if(entity instanceof alt.Vehicle) {
        if (key == 'hasMutedSirens')
            native.setVehicleHasMutedSirens(entity.scriptID, value);
    }
})

alt.on('gameEntityCreate', (entity) => {
    if(entity instanceof alt.Vehicle) {
        native.setVehicleHasMutedSirens(entity.scriptID, entity.getStreamSyncedMeta('hasMutedSirens'));
    }
});

alt.onServer('Server:SpawnarVeiculosFaccao', (ponto, faccao, veiculos) => {
    view = new alt.WebView('http://resource/faccao/spawnveiculos.html');
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

alt.onServer('setPedIntoVehicle', (veh, seatIndex) => {
    let interval = alt.setInterval(() => { 
        native.setPedIntoVehicle(player.scriptID, veh.scriptID, seatIndex); 
        if (native.isPedSittingInVehicle(player.scriptID, veh.scriptID)) 
            alt.clearInterval(interval);
    }, 60);
});

alt.onServer('Server:ToggleFerido', (state) => {
    native.setEntityInvincible(player.scriptID, state == 2);
    if (state == 0) 
        native.clearPedBloodDamage(player.scriptID);
});

alt.onServer('Server:freezeEntityPosition', (state) => {
    native.freezeEntityPosition(player.scriptID, state);
    if (player.vehicle)
        native.freezeEntityPosition(player.vehicle.scriptID, state);
});

alt.onServer('AbrirBarbeariaMaquiagem', (personalizacao, tipo) => {
    native.displayHud(false);
    native.displayRadar(false);
    activateChat(false);
    native.setEntityInvincible(player.scriptID, true);
    alt.emit('character:Edit', JSON.parse(personalizacao), tipo);
});

alt.onServer('SetVehicleDoorState', (vehicle, porta, state) => {
    if (state)
        native.setVehicleDoorShut(vehicle.scriptID, porta, false);
    else
        native.setVehicleDoorOpen(vehicle.scriptID, porta, false, false);
});

alt.onServer('Server:Abastecer', (veiculo) => {
    let temBombaPerto = false;
    bombasCombustivel.forEach(bomb => {
        let object = native.getClosestObjectOfType(player.pos.x, player.pos.y, player.pos.z, 2.0, alt.hash(bomb), false, false, false);
        if (object != 0)
            temBombaPerto = true;
    });

    if (!temBombaPerto) {
        alt.emit('chat:notify', 'Você não está próximo de nenhuma bomba de combustível.', 'danger');
        return;
    }

    alt.emitServer('AbastecerVeiculo', veiculo);
});

alt.onServer('SpectatePlayer', (target) => {
    native.freezeEntityPosition(player.scriptID, true);
    native.destroyAllCams(true);
    native.renderScriptCams(false, false, 0, false, false);
    native.setPedCanSwitchWeapon(player.scriptID, false);

    if (intervalSpec != null)
        alt.clearInterval(intervalSpec);

    intervalSpec = alt.setInterval(() => { 
        native.setEntityVisible(player.scriptID, false, false);
        native.setEntityInvincible(player.scriptID, true);
        if (target.scriptID != 0) {
            alt.clearInterval(intervalSpec);
            native.attachEntityToEntity(player.scriptID, target.scriptID, 0, 0.0, 0.0, 5.0, 0.0, 0.0, 0.0, true, false, false, false, 0, false);
            let cam = native.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', target.pos.x, target.pos.y, target.pos.z, 0, 0, 0, 60);
            native.setCamActive(cam, true);
            native.renderScriptCams(true, false, 0, true, false);
            native.setCamAffectsAiming(cam, false);
            native.attachCamToEntity(cam, target.scriptID, 0, -8.0, 5.0, true); 
            native.pointCamAtEntity(cam, target.scriptID, 0.0, 0.0, 0.0, true);
            intervalSpec = null;
        }
    }, 61);
});

alt.onServer('UnspectatePlayer', () => {
    if (intervalSpec != null)
        alt.clearInterval(intervalSpec);

    native.destroyAllCams(true);
    native.renderScriptCams(false, false, 0, false, false);
    native.detachEntity(player.scriptID, true, true);
    native.freezeEntityPosition(player.scriptID, false);
    native.setEntityVisible(player.scriptID, true, true);
    native.setEntityInvincible(player.scriptID, false);
    native.setPedCanSwitchWeapon(player.scriptID, true);
});

alt.onServer('Server:UsarATM', (tipo, idNome, valor) => {
    let temAtmPerto = false;
    atms.forEach(atm => {
        let object = native.getClosestObjectOfType(player.pos.x, player.pos.y, player.pos.z, 1.0, alt.hash(atm), false, false, false);
        if (object != 0)
        temAtmPerto = true;
    });

    alt.emitServer('UsarATM', tipo, idNome, valor, temAtmPerto);
});

alt.onServer('vehicle:setVehicleEngineHealth', (vehicle, engineHealth) => {
    native.setVehicleEngineHealth(vehicle.scriptID, engineHealth);
});

alt.onServer('vehicle:setVehicleFixed', (vehicle) => {
    native.setVehicleFixed(vehicle.scriptID);
    native.setVehicleDeformationFixed(vehicle.scriptID);
    native.setVehicleUndriveable(vehicle.scriptID, false);
    native.setVehicleEngineHealth(vehicle.scriptID, 1000);
});

alt.onServer('Server:PintarVeiculo', (veiculo, tipo) => {
    view = new alt.WebView('http://resource/concessionaria/pintar.html');
    view.on('closeView', closeView);
    view.on('pintar', (r1, g1, b1, r2, g2, b2) => {
        alt.emitServer('PintarVeiculo', veiculo, tipo, r1, g1, b1, r2, g2, b2);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('displayAdvancedNotification', displayAdvancedNotification);
function displayAdvancedNotification(message, title = "Title", subtitle = "subtitle", notifImage = null, iconType = 0, backgroundColor = null, durationMult = 1) {
    native.beginTextCommandThefeedPost('STRING')
    native.addTextComponentSubstringPlayerName(message)
    if (backgroundColor != null) native.thefeedSetNextPostBackgroundColor(backgroundColor)
    if (notifImage != null) native.endTextCommandThefeedPostMessagetextTu(notifImage, notifImage, false, iconType, title, subtitle, durationMult)
    return native.endTextCommandThefeedPostTicker(false, true)
}

alt.onServer('Server:SpawnarVeiculos', (titulo, veiculos) => {
    view = new alt.WebView('http://resource/concessionaria/spawnveiculos.html');
    view.on('load', () => {
        view.emit('abrirSpawnVeiculos', titulo, veiculos);
    });
    view.on('closeView', closeView);
    view.on('spawnarVeiculo', (codigo) => {
        alt.emitServer('SpawnarVeiculo', codigo);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('AbrirLojaRoupas', (roupas, acessorios, sexo, slots, roupa, tipo, tipoFaccao = 0) => {
    native.displayHud(false);
    native.displayRadar(false);
    activateChat(false);
    native.setEntityInvincible(player.scriptID, true);
    alt.emit('character:EditClothes', JSON.parse(roupas), JSON.parse(acessorios), sexo, slots, roupa, tipo, tipoFaccao);
});

alt.onServer('Server:SyncClothes', (roupas, acessorios, roupa) => {
    let dataRoupas = JSON.parse(roupas);
    let dataAcessorios = JSON.parse(acessorios);
    alt.emit('character:SyncClothes', dataRoupas.filter(x => x.ID == roupa), dataAcessorios.filter(x => x.ID == roupa));
});

alt.onServer('Server:PegarSacoLixo', () => {
    if (objetoEmMaos != null)
        native.deleteObject(objetoEmMaos);

    objetoEmMaos = native.createObjectNoOffset(alt.hash('ng_proc_binbag_01a'), player.pos.x, player.pos.y, player.pos.z, true, true, true);
    native.attachEntityToEntity(objetoEmMaos, player.scriptID, native.getPedBoneIndex(player.scriptID, 0xdead), 0.0, -0.1, -0.4, 0.0, 0.0, 0.0, true, false, false, false, 0, false);
});

alt.onServer('Server:VerificarSoltarSacoLixo', (veh) => {
    let bone = native.getEntityBoneIndexByName(veh.scriptID, 'platelight');
    let pos = native.getWorldPositionOfEntityBone(veh.scriptID, bone);
    alt.emitServer('SoltarSacoLixo', pos.x, pos.y, pos.z);
});

alt.onServer('Server:SoltarSacoLixo', () => {
    if (objetoEmMaos != null)
        native.deleteObject(objetoEmMaos);
});

alt.onServer('Server:setArtificialLightsState', (state) => {
    native.setArtificialLightsState(state);
});

alt.onServer('Server:AbrirMDC', (tipoFaccao, nomeFaccao, ligacoes911, apb, bolo, relatoriosPendentes) => {
    view = new alt.WebView('http://resource/mdc/index.html');
    view.on('load', () => {
        view.emit('abrirMDC', tipoFaccao, nomeFaccao, ligacoes911, apb, bolo, relatoriosPendentes);
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
    view.on('multar', (codigo, nome, valor, motivo, descricao) => {
        alt.emitServer('MDCMultar', codigo, nome, valor, motivo, descricao);
    });
    view.on('revogarLicenca', (codigo, nome) => {
        alt.emitServer('MDCRevogarLicencaMotorista', codigo, nome);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:AtualizarMDC', (botao, div, html) => {
    view.emit('atualizarMDC', botao, div, html);
});