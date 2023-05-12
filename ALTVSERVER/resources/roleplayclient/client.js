import * as alt from 'alt-client';
import * as native from 'natives';
import { view, setView, toggleView, closeView, getAddress, syncDecorations, getRightCoordsZ } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.mjs';
import { playAnimation } from '/helpers/animation.js';
import { drawText2d } from '/helpers/text.js';
import * as nametags from '/helpers/nametags.js';
import { enterVehicleAsDriver, enterVehicleAsPassenger} from '/helpers/enterVehicles.js';
import * as charCreator from '/charcreator/editor.js';
import * as vehtags from '/helpers/vehtags.js';
import * as clothes from '/clothes/editor.js';
import Fingerpointing from '/helpers/fingerpointing.js';
import * as spotlight from '/helpers/spotlight.js';
import { updateCellphone, isCellphoneOpened } from '/cellphone/client.mjs';
import * as bankSystem from '/bank/client.mjs';
import * as staffSystem from '/staff/client.mjs';
import * as factionSystem from '/faction/client.mjs';
import * as tattoos from '/tattoos/editor.js';
import * as inventorySystem from '/inventory/client.mjs';
import * as tuningSystem from '/tuning/client.mjs';
import * as Constants from '/helpers/constants.js';

const WeaponModel = {
    AntiqueCavalryDagger: 2460120199,
    BaseballBat: 2508868239,
    BrokenBottle: 4192643659,
    Crowbar: 2227010557,
    Fist: 2725352035,
    Flashlight: 2343591895,
    GolfClub: 1141786504,
    Hammer: 1317494643,
    Hatchet: 4191993645,
    BrassKnuckles: 3638508604,
    Knife: 2578778090,
    Machete: 3713923289,
    Switchblade: 3756226112,
    Nightstick: 1737195953,
    PipeWrench: 419712736,
    BattleAxe: 3441901897,
    PoolCue: 2484171525,
    StoneHatchet: 940833800,
    Pistol: 453432689,
    PistolMkII: 3219281620,
    CombatPistol: 1593441988,
    APPistol: 584646201,
    StunGun: 911657153,
    Pistol50: 2578377531,
    SNSPistol: 3218215474,
    SNSPistolMkII: 2285322324,
    HeavyPistol: 3523564046,
    VintagePistol: 137902532,
    FlareGun: 1198879012,
    MarksmanPistol: 3696079510,
    HeavyRevolver: 3249783761,
    HeavyRevolverMkII: 3415619887,
    DoubleActionRevolver: 2548703416,
    UpnAtomizer: 2939590305,
    MicroSMG: 324215364,
    SMG: 736523883,
    SMGMkII: 2024373456,
    AssaultSMG: 4024951519,
    CombatPDW: 171789620,
    MachinePistol: 3675956304,
    MiniSMG: 3173288789,
    UnholyHellbringer: 1198256469,
    PumpShotgun: 487013001,
    PumpShotgunMkII: 1432025498,
    SawedOffShotgun: 2017895192,
    AssaultShotgun: 3800352039,
    BullpupShotgun: 2640438543,
    Musket: 2828843422,
    HeavyShotgun: 984333226,
    DoubleBarrelShotgun: 4019527611,
    SweeperShotgun: 317205821,
    AssaultRifle: 3220176749,
    AssaultRifleMkII: 961495388,
    CarbineRifle: 2210333304,
    CarbineRifleMkII: 4208062921,
    AdvancedRifle: 2937143193,
    SpecialCarbine: 3231910285,
    SpecialCarbineMkII: 2526821735,
    BullpupRifle: 2132975508,
    BullpupRifleMkII: 2228681469,
    CompactRifle: 1649403952,
    MG: 2634544996,
    CombatMG: 2144741730,
    CombatMGMkII: 3686625920,
    GusenbergSweeper: 1627465347,
    SniperRifle: 100416529,
    HeavySniper: 205991906,
    HeavySniperMkII: 177293209,
    MarksmanRifle: 3342088282,
    MarksmanRifleMkII: 1785463520,
    RPG: 2982836145,
    GrenadeLauncher: 2726580491,
    GrenadeLauncherSmoke: 1305664598,
    Minigun: 1119849093,
    FireworkLauncher: 2138347493,
    Railgun: 1834241177,
    HomingLauncher: 1672152130,
    CompactGrenadeLauncher: 125959754,
    Widowmaker: 3056410471,
    Grenade: 2481070269,
    BZGas: 2694266206,
    MolotovCocktail: 615608432,
    StickyBomb: 741814745,
    ProximityMines: 2874559379,
    Snowballs: 126349499,
    PipeBombs: 3125143736,
    Baseball: 600439132,
    TearGas: 4256991824,
    Flare: 1233104067,
    JerryCan: 883325847,
    Parachute: 4222310262,
    FireExtinguisher: 101631238,
    GadgetPistol: 1470379660,
    MilitaryRifle: 2636060646,
    CombatShotgun: 94989220,
    Fertilizercan: 406929569,
    HeavyRifle: 3347935668,
    EMPLauncher: 3676729658,
    CeramicPistol: 727643628,
    NavyRevolver: 2441047180,
    HazardCan: 3126027122,
};

const WeaponDamage = [
    { Weapon: WeaponModel.SniperRifle, Damage: 1.0 },
    { Weapon: WeaponModel.HeavySniper, Damage: 1.0 },
    { Weapon: WeaponModel.HeavySniperMkII, Damage: 1.0 },
    { Weapon: WeaponModel.MarksmanRifle, Damage: 1.0 },
    { Weapon: WeaponModel.MarksmanRifleMkII, Damage: 1.0 },
    { Weapon: WeaponModel.Fist, Damage: 0.2 },
    { Weapon: WeaponModel.Nightstick, Damage: 0.2 },
    { Weapon: WeaponModel.BrassKnuckles, Damage: 0.2 },
    { Weapon: WeaponModel.Crowbar, Damage: 0.2 },
    { Weapon: WeaponModel.GolfClub, Damage: 0.2 },
    { Weapon: WeaponModel.Hammer, Damage: 0.2 },
    { Weapon: WeaponModel.PipeWrench, Damage: 0.2 },
    { Weapon: WeaponModel.BaseballBat, Damage: 0.2 },
    { Weapon: WeaponModel.Flashlight, Damage: 0.2 },
    { Weapon: WeaponModel.AntiqueCavalryDagger, Damage: 0.3 },
    { Weapon: WeaponModel.BrokenBottle, Damage: 0.3 },
    { Weapon: WeaponModel.BattleAxe, Damage: 0.3 },
    { Weapon: WeaponModel.Machete, Damage: 0.3 },
    { Weapon: WeaponModel.StoneHatchet, Damage: 0.3 },
    { Weapon: WeaponModel.Switchblade, Damage: 0.3 },
    { Weapon: WeaponModel.Knife, Damage: 0.3 },
    { Weapon: WeaponModel.AssaultShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.BullpupShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.DoubleBarrelShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.HeavyShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.Musket, Damage: 0.4 },
    { Weapon: WeaponModel.PumpShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.PumpShotgunMkII, Damage: 0.4 },
    { Weapon: WeaponModel.SawedOffShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.SweeperShotgun, Damage: 0.4 },
    { Weapon: WeaponModel.HeavyRevolver, Damage: 0.3 },
    { Weapon: WeaponModel.HeavyRevolverMkII, Damage: 0.3 },
    { Weapon: WeaponModel.DoubleActionRevolver, Damage: 0.3 },
];

setWeaponDamageModifier();

const bombasCombustivel = [ 'prop_gas_pump_1a', 'prop_gas_pump_1b', 'prop_gas_pump_1c', 'prop_gas_pump_1d', 'prop_gas_pump_old2', 
    'prop_gas_pump_old3', 'prop_vintage_pump', 'v_serv_metro_wallbin', 'v_serv_tc_bin3_'
];
const directions = [ 
    { name: 'N', value: 0 }, 
    { name: 'O', value: 90 }, 
    { name: 'S', value: 180 }, 
    { name: 'L', value: 270 }, 
    { name: 'N', value: 360 }
];
const player = alt.Player.local;
const pointing = new Fingerpointing();
let playersCount = alt.Player.all.length;
let streetName, zoneName, directionName = '';
let playersViewLoading = false;
let melee = false;

native.setPlayerHealthRechargeMultiplier(player, 0.0);
native.replaceHudColourWithRgba(143, 174, 106, 178, 255);
alt.setStat('stamina', 100);
alt.setStat('strength', 100);
alt.setStat('lung_capacity', 100);
alt.setStat('wheelie_ability', 100);
alt.setStat('flying_ability', 100);
alt.setStat('shooting_ability', 100);
alt.setStat('stealth_ability', 100);
alt.setConfigFlag('DISABLE_AUTO_WEAPON_SWAP', true);
alt.setConfigFlag('DISABLE_PED_PROP_KNOCK_OFF', true);
alt.setConfigFlag('DISABLE_IDLE_CAMERA', true);
alt.setConfigFlag('DISABLE_VEHICLE_ENGINE_SHUTDOWN_ON_LEAVE', true);
alt.setMsPerGameMinute(60000);
alt.setMinimapIsRectangle(true);

function isDriver() {
    return player.vehicle && player.seat == 1;
}

function setWeaponDamageModifier() {
    for (const weapon in WeaponModel) {
        const damage = WeaponDamage.find(x => x.Weapon == WeaponModel[weapon]);
        const damageModifier = damage ? damage.Damage : 0.7;
        const weaponData = alt.WeaponData.getForHash(WeaponModel[weapon]);
        weaponData.playerDamageModifier = damageModifier;
    }
}

alt.setInterval(() => {
    if (!player.hasStreamSyncedMeta(Constants.PLAYER_META_DATA_NAMETAG)) 
        return;

    playersCount = alt.Player.all.length;

    updateCellphone();
    
    const animationDic = player.getMeta('animation_dic');
    if (animationDic != '' && animationDic !== undefined) {
        const animationName = player.getMeta('animation_name');
        if (!native.isEntityPlayingAnim(player, animationDic, animationName, 3)) {
            if (player.getMeta('animation_freeze'))
                playAnimation(
                    animationDic, 
                    animationName, 
                    player.getMeta('animation_flag'), 
                    player.getMeta('animation_duration'), 
                    true
                );
            else
                alt.emitServer('StopAnimation');
        }
    }

    melee = false;
}, 1000);

alt.everyTick(() => {
    if (!player.hasStreamSyncedMeta(Constants.PLAYER_META_DATA_NAMETAG)) 
        return;

    drawText2d(`~s~Segunda Vida ~w~Roleplay`, 1, 0.90, 0.5, 4, 174, 106, 178, 180, true, true, 2, false);
    drawText2d(`~s~v1.17 ~w~(${playersCount}/100)`, 1, 0.93, 0.4, 4, 174, 106, 178, 180, true, true, 2, false);
    
    [zoneName, streetName] = getAddress(player.pos);

    native.hideHudComponentThisFrame(6);
    native.hideHudComponentThisFrame(7);
    native.hideHudComponentThisFrame(8);
    native.hideHudComponentThisFrame(9);

    native.blockWeaponWheelThisFrame();
    native.disableControlAction(0, 23, true); // INPUT_ENTER
    native.disableControlAction(0, 37, true); // INPUT_SELECT_WEAPON

    //native.setPedConfigFlag(player, 429, true); // PED_FLAG_DISABLE_STARTING_VEH_ENGINE - TESTAR SEM ISSO PQ TEM FLAG AGORA
    //native.setPedConfigFlag(player, 241, true); // PED_FLAG_DISABLE_STOPPING_VEH_ENGINE - TESTAR SEM ISSO PQ TEM FLAG AGORA
    native.setPedConfigFlag(player, 184, true); // PED_FLAG_DISABLE_SHUFFLING_TO_DRIVER_SEAT

    native.setPedSuffersCriticalHits(player, false);

    if (player.vehicle) {
        if (player.vehicle.hasStreamSyncedMeta(Constants.VEHICLE_META_DATA_ATTACHED)) {
            native.disableControlAction(0, 71, true); // INPUT_VEH_ACCELERATE
            native.disableControlAction(0, 72, true); // INPUT_VEH_BRAKE
        }
        
        native.setPedHelmet(player, false);

        native.setVehicleRadioEnabled(player.vehicle, player.vehicle.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_RADIO_ENABLED));

        if (isDriver()) {
            // Desabilitar X em motos
            if (player.vehicle.wheelsCount == 2) {
                native.disableControlAction(0, 73, true); // INPUT_VEH_DUCK
                native.disableControlAction(0, 345, true); // INPUT_VEH_MELEE_HOLD
                native.disableControlAction(0, 346, true); // INPUT_VEH_MELEE_LEFT
                native.disableControlAction(0, 347, true); // INPUT_VEH_MELEE_RIGHT
            }

            const roll = native.getEntityRoll(player.vehicle);
            if ((roll > 75.0 || roll < -75.0) && native.getEntitySpeed(player.vehicle) < 2) {
                native.disableControlAction(0, 59, true);
                native.disableControlAction(0, 60, true);
            }
        }
    } else {
        // Desabilitar coronhada
        if (native.isPedArmed(player, 6) 
            || (native.getPedStealthMovement(player) && player.currentWeapon == WeaponModel.Fist)) {
            native.disableControlAction(0, 140, true);
            native.disableControlAction(0, 141, true);
            native.disableControlAction(0, 142, true);
        }

        // Desabilitar tiro as cegas
        if (native.isPedInCover(player, 1) && !native.isPedAimingFromCover(player)) {
            native.disableControlAction(0, 24, true);
            native.disableControlAction(0, 142, true);
            native.disableControlAction(0, 257, true);
        }

        // Desabilitar rolamento
        if (player.isReloading ||
            player.isAiming ||
            native.getPedConfigFlag(player, 78, true) ||
            native.isPlayerFreeAiming(player) ||
            native.isControlJustPressed(1, 25)) 
                native.disableControlAction(0, 22, true); // Space Bar
        
        // Melee Shift + G
        if (native.isShockingEventInSphere(112, player.pos.x, player.pos.y, player.pos.z, 1.0)) {
            melee = true;
            if (melee)
                native.disableControlAction(0, 24, true);
        }

        const animationDic = player.getMeta('animation_dic');
        if (animationDic != '' && animationDic !== undefined) {
            native.disableControlAction(0, 22, true); // Space Bar
            native.disableControlAction(0, 24, true);
            native.disableControlAction(0, 25, true);
            native.disableControlAction(0, 140, true);
            native.disableControlAction(0, 141, true);
            native.disableControlAction(0, 142, true);
            native.disableControlAction(0, 257, true);
        }
    }

    if (player.getMeta('f7'))
        return;
    
    if (!native.isRadarHidden()) {
        if (streetName != '' && zoneName != '') {
            const direction = player.vehicle ? native.getEntityHeading(player.vehicle) : native.getEntityHeading(player);
            directions.forEach(x => {
                if (Math.abs(direction - x.value) < 45) 
                    directionName = x.name;
            });

            drawText2d(directionName, 0.15, 0.945, 1.0, 4, 255, 255, 255, 230, true, true, 1);
            drawText2d(zoneName, 0.165, 0.945, 0.55, 4, 174, 106, 178, 230, true, true, 1);
            drawText2d(streetName, 0.165, 0.975, 0.45, 4, 255, 255, 255, 230, true, true, 1);
        }

        if (isDriver()) {
            drawText2d(player.vehicle.getStreamSyncedMeta(Constants.VEHICLE_META_DATA_FUEL), 0.15, 0.905, 0.4, 4, 255, 255, 255, 230, true, true, 1);
            drawText2d(`${(native.getEntitySpeed(player.vehicle) * 3.6).toFixed(0)} KM/H${(cruiseEveryTick ? ' ~g~(CRUISE CONTROL)' : '')}`, 
                0.15, 0.925,
                0.4, 4, 
                255, 255, 255, 230, 
                true, true, 1);
        }
    }
});

const functionsKeyDown = {
    70() { // F
        enterVehicleAsDriver().catch((res) => {
            alt.log(res);
        });
    },
    71() { // G
        enterVehicleAsPassenger().catch((res) => {
            alt.log(res);
        });
    },
    81() { // Q
        if (isDriver()) {
            const hasMutedSirens = !player.vehicle.getStreamSyncedMeta('hasMutedSirens');
            alt.emitServer('SetVehicleMeta', player.vehicle, 'hasMutedSirens', hasMutedSirens);
        }
    },
    118() { // F7
        const f7 = !player.getMeta('f7');
        player.setMeta('f7', f7);
        activateChat(!f7);
        native.displayHud(!f7);
        native.displayRadar(!f7);
    },
    66() { // B
        if (!player.getSyncedMeta('animation')) {
            if (pointing.active) 
                pointing.stop();
            else if (!player.vehicle)
                pointing.start();
        }
    },
    112() { // F1
        alt.emitServer('AbrirPainelControleUsuario');
    },
    73() { // I
        alt.emitServer('ShowInventory');
    },
    76() { // L
        alt.emitServer('Trancar');
    },
    90() { // Z
        if (isDriver()) 
            alt.emitServer('Motor');
        else
            toggleCrouch();
    },
    74() { // J
        toggleCruise();
    },
    114() { // F3
        alt.log('F3');
    },
    115() { // F4
        alt.log('F4');
    },
    192() { // '
        alt.emitServer('EquipWeapon', 0);
    },
    49() { // 1
        alt.emitServer('EquipWeapon', -1);
    },
    50() { // 2
        alt.emitServer('EquipWeapon', -2);
    },
    51() { // 3
        alt.emitServer('EquipWeapon', -3);
    },
    9() { // TAB
        playersViewLoading = true;
        alt.emitServer('ShowPlayerList');
    },
    77() { // M
        alt.log('[Aiming info]');
        const entityFound = native.getEntityPlayerIsFreeAimingAt(player);
        if (entityFound[0]) {
            const entityHash = native.getEntityModel(entityFound[1]);
            alt.log('  Entity hash (dec): ' + entityHash.toString(10));
            const entityPosition = native.getEntityCoords(entityFound[1], false);
            alt.log('  Entity position: (X: ' + entityPosition.x + ' Y: ' + entityPosition.y + ' Z: ' + entityPosition.z + ')');
        }
    },
    89() { // Y
        alt.emitServer('KeyY');
    },
    46() { // DEL
        alt.emitServer('KeyDelete');
    }
}

alt.on('keydown', (key) => {
    const func = functionsKeyDown[key];
    if (func) {
        if (!player.hasStreamSyncedMeta(Constants.PLAYER_META_DATA_NAMETAG) || player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_CHATTING) 
        || alt.isMenuOpen() || view != null 
        || isCellphoneOpened || playersViewLoading 
        || (!alt.gameControlsEnabled() && key != 46)) 
            return;

        func();
    }
});

let crouch = false;
function toggleCrouch() {
    if (player.vehicle || player.getStreamSyncedMeta(Constants.PLAYER_META_DATA_INJURED) != 0) 
        return;

    alt.Utils.requestAnimSet('move_ped_crouched').then(() => {
        alt.Utils.requestAnimSet('move_ped_crouched_strafing').then(() => {
            if (crouch) {
                native.resetPedStrafeClipset(player);
                native.resetPedMovementClipset(player, 0.2);

                const movement = player.getMeta('movement');
                if (movement != '')
                    native.setPedMovementClipset(player, movement, 0.2);
            } else {
                native.setPedStrafeClipset(player, 'move_ped_crouched_strafing');
                native.setPedMovementClipset(player, 'move_ped_crouched', 0.2);
            }

            crouch = !crouch;
        });
    });
}

let cruiseSpeed = 0;
let cruiseEveryTick;
function toggleCruise() {
    if (!isDriver()) 
        return;

    if (!cruiseEveryTick) {
        const speed = native.getEntitySpeed(player.vehicle);
        if (speed <= 19.5 && speed > 0 && player.vehicle.gear > 0) { // 70 KM/H
            cruiseSpeed = speed;
            cruiseEveryTick = alt.everyTick(() => {
                if (!player.vehicle || native.isControlJustPressed(2, 76) || native.isControlJustPressed(2, 72) 
                    || native.isControlJustPressed(2, 75) || native.isControlJustPressed(2, 71)
                    || native.isEntityInAir(player.vehicle) || native.hasEntityCollidedWithAnything(player.vehicle)
                    || !native.getIsVehicleEngineRunning(player.vehicle) || native.isEntityInWater(player.vehicle)) {
                    toggleCruise();
                    return;
                }
            
                native.setVehicleForwardSpeed(player.vehicle, cruiseSpeed);
            });
        } else {
            alt.emit('chat:notify', 'Veículo precisa estar em movimento e abaixo de 70 km/h.', 'danger');
        }
    } else {
        alt.clearEveryTick(cruiseEveryTick);
        cruiseEveryTick = null;
    }
}

alt.onServer('Server:RequestIpl', (ipl) => {
    alt.requestIpl(ipl);
});

alt.onServer('Server:RemoveIpl', (ipl) => {
    alt.removeIpl(ipl);
});

alt.onServer('Server:SetWaypoint', (x, y) => {
    native.setNewWaypoint(x, y);
});

alt.onServer('SetVehicleDoorState', (vehicle, porta, state) => {
    if (state) 
        native.setVehicleDoorShut(vehicle.scriptID, porta, false);
    else 
        native.setVehicleDoorOpen(vehicle.scriptID, porta, false, false);
});

alt.onServer('Server:setArtificialLightsState', (state) => {
    native.setArtificialVehicleLightsState(false);
    native.setArtificialLightsState(state);
});

alt.onServer('Server:ToggleGameControls', (state) => {
    alt.toggleGameControls(state);
});

alt.onServer('SetVehicleMaxSpeed', (speed) => {
    native.setVehicleMaxSpeed(player.vehicle, speed);
});

alt.onServer('SetPlayerCanDoDriveBy', (state) => {
    native.setPlayerCanDoDriveBy(player, state);
});

alt.onServer('TaskRappelFromHeli', () => {
    native.taskRappelFromHeli(player, 0);
});

alt.onServer('Server:BaseHTML', (html) => {
    if (view != null) 
        view.destroy();

    setView(new alt.WebView('http://resource/login/base.html'));
    view.on('load', () => {
        view.emit('showHTML', html);
    });
    view.on('closeView', closeView);
    view.focus();
    toggleView(true);
});

alt.onServer('Server:DisableHUD', disableHud);
function disableHud() {
    native.displayHud(false);
    native.displayRadar(false);
    activateChat(false);
    player.setMeta('f7', false);
}

disableHud();
setView(new alt.WebView('http://resource/login/login.html'));
view.on('requestDiscordToken', () => {
    requestDiscordToken();
});
toggleView(true);

async function requestDiscordToken() {
    try {
        const token = await alt.Discord.requestOAuth2Token(Constants.DISCORD_APP_ID);
        alt.emitServer('ValidateDiscordToken', token);
    } catch (ex) {
        view.emit('mostrarErro', ex);
    }
}

requestDiscordToken();

alt.onServer('Server:ExibirPerguntas', (perguntas) => {
    view.destroy();
    setView(new alt.WebView('http://resource/login/perguntas.html'));  
    view.on('load', () => {
        view.emit('showHTML', perguntas);
    });
    view.on('confirmar', () => {
        alt.emitServer('ListarPersonagens');
    });
    view.focus();
});

alt.onServer('Server:ListarPersonagens', (nome, personagens, slots, alerta) => {
    if (view != null) 
        view.destroy();
    else 
        toggleView(true);

    setView(new alt.WebView('http://resource/login/personagens.html'));
    view.on('load', () => {
        view.emit('showHTML', nome, personagens, slots, alerta);
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
    view.on('punicoesAdministrativas', () => {
        alt.emitServer('PunicoesAdministrativas');
    });
    view.focus();
});

alt.onServer('Server:PunicoesAdministrativas', (nome, data, punicoesAdministrativas) => {
    view.destroy();
    setView(new alt.WebView('http://resource/login/punicoesadministrativas.html'));
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
    setView(new alt.WebView('http://resource/login/criarpersonagem.html'));
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

alt.onServer('Server:SelecionarPersonagem', (personalizationStep, sex, personalizationJSON) => {
    if (view != null) 
        view.destroy();

    setView(null);
    toggleView(false);

    native.destroyAllCams(true);
    native.renderScriptCams(false, false, 0, false, false, 0);
    native.clearPedTasks(player);
    
    const personalization = JSON.parse(personalizationJSON);
    syncDecorations(personalization);

    if (personalizationStep == 4) {
        native.freezeEntityPosition(player, false);
        activateChat(true);
        native.displayHud(true);
        native.displayRadar(true);
    } else if (personalizationStep == 1) {
        alt.emit('character:Edit', sex, personalization, false);
    } else if (personalizationStep == 2) {
        alt.emit('character:EditTattoos', sex, personalization);
    } else if (personalizationStep == 3) {
        alt.emit('character:EditClothes', sex);
    }
});

alt.onServer('Server:MostrarErro', mostrarErro);
function mostrarErro(erro, componente) {
    view.emit('mostrarErro', erro, componente);
}

alt.onServer('Server:MostrarSucesso', (mensagem) => {
    view.emit('mostrarSucesso', mensagem);
});

alt.onServer('Server:ComprarVeiculo', (titulo, tipo, veiculos) => {
    setView(new alt.WebView('http://resource/concessionaria/concessionaria.html'));
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
    setView(new alt.WebView('http://resource/conveniencia/conveniencia.html'));
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

alt.onServer('alt:log', (msg) => {
    alt.log(msg);
});

alt.on('streamSyncedMetaChange', (entity, key, value, oldValue) => {
    if(entity instanceof alt.Vehicle) {
        if (key == 'hasMutedSirens')
            native.setVehicleHasMutedSirens(entity.scriptID, value);
    }
});

alt.on('gameEntityCreate', (entity) => {
    if(entity instanceof alt.Vehicle) {
        native.setVehicleHasMutedSirens(entity.scriptID, entity.getStreamSyncedMeta('hasMutedSirens'));
    }
});

alt.onServer('Server:Abastecer', (veiculo) => {
    let temBombaPerto = false;
    bombasCombustivel.forEach(bomb => {
        if (temBombaPerto) 
            return;

        const object = native.getClosestObjectOfType(player.pos.x, player.pos.y, player.pos.z, 2.0, alt.hash(bomb), false, false, false);
        temBombaPerto = object != 0;
    });

    if (!temBombaPerto) {
        alt.emit('chat:notify', 'Você não está próximo de uma bomba de combustível.', 'danger');
        return;
    }

    alt.emitServer('AbastecerVeiculo', veiculo);
});

let intervalSpec = null;
alt.onServer('SpectatePlayer', (target) => {
    if (intervalSpec != null) 
        alt.clearInterval(intervalSpec);

    intervalSpec = alt.setInterval(() => { 
        if (target.scriptID != 0) {
            alt.clearInterval(intervalSpec);
            native.attachEntityToEntity(player.scriptID, target.scriptID, 0, 0.0, 0.0, 5.0, 0.0, 0.0, 0.0, true, false, false, false, 0, false);
            const cam = native.createCamWithParams('DEFAULT_SCRIPTED_CAMERA', target.pos.x, target.pos.y, target.pos.z, 0, 0, 0, 60, true, 0);
            native.setCamActive(cam, true);
            native.renderScriptCams(true, false, 0, true, false, 0);
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
    native.renderScriptCams(false, false, 0, false, false, 0);
    native.detachEntity(player.scriptID, true, true);
});

alt.onServer('Server:PintarVeiculo', (veiculo) => {
    setView(new alt.WebView('http://resource/concessionaria/pintar.html'));
    view.on('closeView', closeView);
    view.on('pintar', (r1, g1, b1, r2, g2, b2) => {
        alt.emitServer('PintarVeiculo', veiculo, r1, g1, b1, r2, g2, b2);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Server:SpawnarVeiculos', (titulo, veiculos) => {
    setView(new alt.WebView('http://resource/concessionaria/spawnveiculos.html'));
    view.on('load', () => {
        view.emit('abrirSpawnVeiculos', titulo, JSON.parse(veiculos));
    });
    view.on('closeView', closeView);
    view.on('spawnarVeiculo', (codigo) => {
        alt.emitServer('SpawnarVeiculo', codigo);
    });
    view.focus();
    toggleView(true);
});

let objetoEmMaos = null;
alt.onServer('Server:PegarSacoLixo', () => {
    objetoEmMaos?.destroy();

    objetoEmMaos = new alt.Object('ng_proc_binbag_01a', player.pos, alt.Vector3.zero, false, true);
    objetoEmMaos.attachToEntity(player, native.getPedBoneIndex(player, 0xdead), new alt.Vector3(0.0, -0.1, -0.4), alt.Vector3.zero, false, false, false);
});

alt.onServer('Server:VerificarSoltarSacoLixo', (veh) => {
    const bone = native.getEntityBoneIndexByName(veh.scriptID, 'platelight');
    const pos = native.getWorldPositionOfEntityBone(veh.scriptID, bone);
    alt.emitServer('SoltarSacoLixo', pos.x, pos.y, pos.z);
});

alt.onServer('Server:SoltarSacoLixo', () => {
    objetoEmMaos?.destroy();
    objetoEmMaos = null;
});

alt.onServer('Server:AbrirPainelControleUsuario', (htmlComandos, htmlMinhasInformacoes, htmlConfiguracoes) => {
    if (view != null)
        return;

    setView(new alt.WebView('http://resource/ucp/index.html'));
    view.on('load', () => {
        view.emit('abrirUCP', htmlComandos, htmlMinhasInformacoes, htmlConfiguracoes);
    });
    view.on('closeView', closeView);
    view.on('gravar', (timestamp, dl, anuncios, pm, chatfaccao, chatstaff, tipoFonte, tamanhoFonte, linhas, faction) => {
        alt.emitServer('UCPGravar', timestamp, dl, anuncios, pm, chatfaccao, chatstaff, tipoFonte, tamanhoFonte, linhas, faction);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('Waypoint', () => {
    const waypoint = native.getFirstBlipInfoId(8);

    if (native.doesBlipExist(waypoint)) {
        const coords = native.getBlipInfoIdCoord(waypoint);
        getRightCoordsZ(coords.x, coords.y, coords.z).then((res) => {
            alt.emitServer('Waypoint', res.x, res.y, res.z);
        });
        return;
    }

    alt.emit('chat:notify', 'Você não possui um waypoint marcado no mapa.', 'danger');
});

alt.onServer('Server:ListarPlayers', (nomeServidor, players, rodape) => {
    setView(new alt.WebView('http://resource/players/players.html'));
    view.on('load', () => {
        view.emit('showPlayers', nomeServidor, JSON.parse(players), rodape);
        playersViewLoading = false;
    });
    view.on('close', () => {
        closeView();
        playersViewLoading = false;
    })
    view.focus();
    toggleView(true);
});

alt.onServer('RegistrarImagemDMV', (valor) => {
    alt.Utils.registerPedheadshot3Base64(player).then((pedHeadshot) => {
        alt.emitServer('RegistrarImagemDMV', pedHeadshot, valor);
    });
});

alt.onServer('PlayScenario', (scenarioName) => {
    native.taskStartScenarioInPlace(player, scenarioName, 0, true);
});

let oldWeather = 'CLEAR';
let intervalWeather;
alt.onServer('SyncWeather', (weather) => {
    if (intervalWeather != null)
        return;

    if (oldWeather != weather) {
        let i = 0;
        let intervalWeather = alt.setInterval(() => {
            i++;
            if (i < 100) {
                native.setCurrWeatherState(alt.hash(oldWeather), alt.hash(weather), (i / 100));
            } else {
                alt.clearInterval(intervalWeather);
                oldWeather = weather;
                intervalWeather = null;
            }
        }, 100);
    }

    if (weather === 'XMAS') {
        native.useSnowWheelVfxWhenUnsheltered(true);
        native.useSnowFootVfxWhenUnsheltered(true);
    } else {
        native.useSnowWheelVfxWhenUnsheltered(false);
        native.useSnowFootVfxWhenUnsheltered(false);
    }
});

alt.onServer('AddPedDecorationFromHashes', (collection, overlay) => {
    native.addPedDecorationFromHashes(
        player, 
        alt.hash(collection), 
        alt.hash(overlay)
    );
});

alt.onServer('SetDrugEffect', (drug) => {
    if (drug == 24 || drug == 25) { // Cocaína, Crack
        native.setTimecycleModifier("spectator5");
        native.setPedMotionBlur(player, true);

        player.setMeta('movement', 'MOVE_M@QUICK');
        alt.Utils.requestAnimSet('MOVE_M@QUICK').then(() => {
            native.setPedMovementClipset(player, 'MOVE_M@QUICK', 1.0);
        });

        native.setPedIsDrunk(player, true);
        native.setPedMoveRateOverride(player, 2.5);
        native.setRunSprintMultiplierForPlayer(player, 1.19);
        native.animpostfxPlay("DrugsMichaelAliensFight", 10000001, true);
        native.shakeGameplayCam("DRUNK_SHAKE", 0.7);
    } else if (drug == 23 || drug == 28 || drug == 29) { // Maconha, Xanax, Oxycontin
        native.setTimecycleModifier("spectator6");
        native.setPedMotionBlur(player, true);
        native.animpostfxPlay("ChopVision", 10000001, true);
        native.shakeGameplayCam("DRUNK_SHAKE", 0.2);
    } else if (drug == 26) { // Heroína
        native.setTimecycleModifier("spectator3");
        native.setPedIsDrunk(player, true);
        native.setPedMotionBlur(player, true);
        native.animpostfxPlay("HeistCelebPass", 10000001, true);
        native.shakeGameplayCam("DRUNK_SHAKE", 0.4);

        player.setMeta('movement', 'move_m@hobo@a');
        alt.Utils.requestAnimSet('move_m@hobo@a').then(() => {
            native.setPedMovementClipset(player, 'move_m@hobo@a', 1.0);
        });
    } else if (drug == 27 || drug == 30) { // MDMA, Metanfetamina
        native.setTimecycleModifier("spectator5");
        native.setPedIsDrunk(player, true);
        native.setPedMotionBlur(player, true);
        native.animpostfxPlay("SuccessMichael", 10000001, true);
        native.shakeGameplayCam("DRUNK_SHAKE", 0.5);

        player.setMeta('movement', 'move_m@drunk@slightlydrunk');
        alt.Utils.requestAnimSet('move_m@drunk@slightlydrunk').then(() => {
            native.setPedMovementClipset(player, 'move_m@drunk@slightlydrunk', 1.0);
        });
    }
});

alt.onServer('ClearDrugEffect', () => {
    native.setPedMoveRateOverride(player, 1);
    native.setRunSprintMultiplierForPlayer(player, 1);
    native.setPedIsDrunk(player, false);
    native.resetPedMovementClipset(player, 0.2);
    native.animpostfxStopAll();
    native.shakeGameplayCam("DRUNK_SHAKE", 0.0);
    native.setTimecycleModifierStrength(0.0);
    player.setMeta('movement', '');
});

alt.onServer('VehicleUpgrade', (title, vehicleId, items) => {
    setView(new alt.WebView('http://resource/concessionaria/vehicleupgrade.html'));
    view.on('load', () => {
        view.emit('showHTML', title, JSON.parse(items));
    });
    view.on('closeView', closeView);
    view.on('buy', (name) => {
        alt.emitServer('BuyVehicleUpgrade', vehicleId, name);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('PropertyUpgrade', (title, propertyId, items) => {
    setView(new alt.WebView('http://resource/concessionaria/vehicleupgrade.html'));
    view.on('load', () => {
        view.emit('showHTML', title, JSON.parse(items));
    });
    view.on('closeView', closeView);
    view.on('buy', (name) => {
        alt.emitServer('BuyPropertyUpgrade', propertyId, name);
    });
    view.focus();
    toggleView(true);
});

alt.onServer('SetAreaName', () => {
    alt.emitServer('SetAreaName', `${streetName}, ${zoneName}`);
});

alt.on('windowFocusChange', (isFocused) => {
    alt.emitServer('AtualizarInformacoes', isFocused);
});

alt.on('playerWeaponShoot', (weaponHash, totalAmmo, ammoInClip) => {
    alt.emitServer('UpdateWeaponAmmo', weaponHash, totalAmmo);
});