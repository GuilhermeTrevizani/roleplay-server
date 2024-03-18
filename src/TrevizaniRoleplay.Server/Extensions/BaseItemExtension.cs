﻿using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class BaseItemExtension
    {
        public static string GetName(this BaseItem baseItem)
        {
            if (baseItem.Category == ItemCategory.Weapon)
            {
                if (baseItem.Type == (uint)WeaponModel.JerryCan)
                    return "Galão de Combustível";

                return ((WeaponModel)baseItem.Type).ToString();
            }

            return baseItem.Category.GetDisplay();
        }

        public static float GetWeight(this BaseItem baseItem)
        {
            if (baseItem.Category == ItemCategory.Weapon && baseItem.Type == (uint)WeaponModel.JerryCan)
                return 3f;

            return baseItem.Category switch
            {
                ItemCategory.Cloth1 => 0.5f,
                ItemCategory.Cloth3 => 0.5f,
                ItemCategory.Cloth4 => 0.5f,
                ItemCategory.Cloth5 => 0.5f,
                ItemCategory.Cloth6 => 0.5f,
                ItemCategory.Cloth7 => 0.5f,
                ItemCategory.Cloth8 => 0.5f,
                ItemCategory.Cloth9 => 0.5f,
                ItemCategory.Cloth10 => 0.5f,
                ItemCategory.Cloth11 => 0.5f,
                ItemCategory.Accessory0 => 0.5f,
                ItemCategory.Accessory1 => 0.5f,
                ItemCategory.Accessory2 => 0.5f,
                ItemCategory.Accessory6 => 0.5f,
                ItemCategory.Accessory7 => 0.5f,
                ItemCategory.Money => 0.000001f,
                ItemCategory.VehicleKey => 0.1f,
                ItemCategory.PropertyKey => 0.1f,
                ItemCategory.WalkieTalkie => 0.1f,
                ItemCategory.Weed => 0.001f,
                ItemCategory.Cocaine => 0.001f,
                ItemCategory.Crack => 0.001f,
                ItemCategory.Heroin => 0.001f,
                ItemCategory.MDMA => 0.001f,
                ItemCategory.Xanax => 0.001f,
                ItemCategory.Oxycontin => 0.001f,
                ItemCategory.Metanfetamina => 0.001f,
                ItemCategory.Boombox => 1.5f,
                ItemCategory.Microphone => 0.7f,
                _ => 1,
            };
        }

        public static string GetObjectName(this BaseItem baseItem)
        {
            if (baseItem.Category == ItemCategory.Weapon)
            {
                return (WeaponModel)baseItem.Type switch
                {
                    WeaponModel.AntiqueCavalryDagger => "prop_w_me_dagger",
                    WeaponModel.BaseballBat => "p_cs_bbbat_01",
                    WeaponModel.BrokenBottle => "prop_w_me_bottle",
                    WeaponModel.Crowbar => "prop_ing_crowbar",
                    WeaponModel.Flashlight => "w_me_flashlight",
                    WeaponModel.GolfClub => "prop_golf_iron_01",
                    WeaponModel.Hammer => "prop_tool_hammer",
                    WeaponModel.Hatchet => "prop_w_me_hatchet",
                    WeaponModel.BrassKnuckles => "w_me_knuckle_dmd",
                    WeaponModel.Knife => "prop_w_me_knife_01",
                    WeaponModel.Machete => "ch_prop_ch_bloodymachete_01a",
                    WeaponModel.Switchblade => "w_me_switchblade",
                    WeaponModel.Nightstick => "w_me_nightstick",
                    WeaponModel.PipeWrench => "w_me_wrench",
                    WeaponModel.BattleAxe => "w_me_battleaxe",
                    WeaponModel.PoolCue => "w_me_poolcue",
                    WeaponModel.StoneHatchet => "w_me_stonehatchet",
                    WeaponModel.Pistol => "w_pi_pistol",
                    WeaponModel.PistolMkII => "w_pi_pistolmk2",
                    WeaponModel.CombatPistol => "w_pi_combatpistol",
                    WeaponModel.APPistol => "w_pi_appistol",
                    WeaponModel.StunGun => "w_pi_stungun",
                    WeaponModel.Pistol50 => "w_pi_pistol50",
                    WeaponModel.SNSPistol => "w_pi_sns_pistol",
                    WeaponModel.SNSPistolMkII => "w_pi_sns_pistolmk2",
                    WeaponModel.HeavyPistol => "w_pi_heavypistol",
                    WeaponModel.VintagePistol => "w_pi_vintage_pistol",
                    WeaponModel.FlareGun => "w_pi_flaregun",
                    WeaponModel.MarksmanPistol => "w_pi_singleshot",
                    WeaponModel.HeavyRevolver => "w_pi_revolver",
                    WeaponModel.HeavyRevolverMkII => "w_pi_revolvermk2",
                    WeaponModel.DoubleActionRevolver => "w_pi_wep1_gun",
                    WeaponModel.UpnAtomizer => "w_pi_raygun",
                    WeaponModel.MicroSMG => "w_sb_microsmg",
                    WeaponModel.SMG => "w_sb_smg",
                    WeaponModel.SMGMkII => "w_sb_smgmk2",
                    WeaponModel.AssaultSMG => "w_sb_assaultsmg",
                    WeaponModel.CombatPDW => "w_sb_pdw",
                    WeaponModel.MachinePistol => "w_sb_compactsmg",
                    WeaponModel.MiniSMG => "w_sb_compactsmg",
                    WeaponModel.UnholyHellbringer => "w_ar_srifle",
                    WeaponModel.PumpShotgun => "w_sg_pumpshotgun",
                    WeaponModel.PumpShotgunMkII => "w_sg_pumpshotgunmk2",
                    WeaponModel.SawedOffShotgun => "w_sg_sawnoff",
                    WeaponModel.AssaultShotgun => "w_sg_assaultshotgun",
                    WeaponModel.BullpupShotgun => "w_sg_bullpupshotgun",
                    WeaponModel.Musket => "w_ar_musket",
                    WeaponModel.HeavyShotgun => "w_sg_heavyshotgun",
                    WeaponModel.DoubleBarrelShotgun => "w_sg_doublebarrel",
                    WeaponModel.SweeperShotgun => "w_sg_sweeper",
                    WeaponModel.AssaultRifle => "w_ar_assaultrifle",
                    WeaponModel.AssaultRifleMkII => "w_ar_assaultriflemk2",
                    WeaponModel.CarbineRifle => "w_ar_carbinerifle",
                    WeaponModel.CarbineRifleMkII => "w_ar_carbineriflemk2",
                    WeaponModel.AdvancedRifle => "w_ar_advancedrifle",
                    WeaponModel.SpecialCarbine => "w_ar_specialcarbine",
                    WeaponModel.SpecialCarbineMkII => "w_ar_specialcarbinemk2",
                    WeaponModel.BullpupRifle => "w_ar_bullpuprifle",
                    WeaponModel.BullpupRifleMkII => "w_ar_bullpupriflemk2",
                    WeaponModel.CompactRifle => "w_ar_assaultrifle_smg",
                    WeaponModel.MG => "w_mg_mg",
                    WeaponModel.CombatMG => "w_mg_combatmg",
                    WeaponModel.CombatMGMkII => "w_mg_combatmgmk2",
                    WeaponModel.GusenbergSweeper => "w_sb_gusenberg",
                    WeaponModel.SniperRifle => "w_sr_sniperrifle",
                    WeaponModel.HeavySniper => "w_sr_heavysniper",
                    WeaponModel.HeavySniperMkII => "w_sr_heavysnipermk2",
                    WeaponModel.MarksmanRifle => "w_sr_marksmanrifle",
                    WeaponModel.MarksmanRifleMkII => "w_sr_marksmanriflemk2",
                    WeaponModel.RPG => "w_lr_rpg",
                    WeaponModel.GrenadeLauncher => "w_lr_grenadelauncher",
                    WeaponModel.GrenadeLauncherSmoke => "w_lr_grenadelauncher",
                    WeaponModel.Minigun => "w_mg_minigun",
                    WeaponModel.FireworkLauncher => "w_lr_firework",
                    WeaponModel.Railgun => "w_ar_railgun",
                    WeaponModel.HomingLauncher => "w_lr_homing",
                    WeaponModel.CompactGrenadeLauncher => "w_lr_compactgl",
                    WeaponModel.Widowmaker => "w_mg_sminigun",
                    WeaponModel.Grenade => "w_ex_grenadefrag",
                    WeaponModel.BZGas => "prop_gas_grenade",
                    WeaponModel.MolotovCocktail => "w_ex_molotov",
                    WeaponModel.StickyBomb => "w_ex_vehiclemine",
                    WeaponModel.ProximityMines => "w_ex_apmine",
                    WeaponModel.Snowballs => "w_ex_snowball",
                    WeaponModel.PipeBombs => "w_ex_pipebomb",
                    WeaponModel.Baseball => "w_am_baseball",
                    WeaponModel.TearGas => "w_ex_grenadesmoke",
                    WeaponModel.Flare => "prop_flare_01",
                    WeaponModel.JerryCan => "v_ind_cs_jerrycan01",
                    WeaponModel.Parachute => "prop_parachute",
                    WeaponModel.FireExtinguisher => "prop_fire_exting_1a",
                    WeaponModel.GadgetPistol => "w_pi_singleshoth4",
                    WeaponModel.MilitaryRifle => "w_ar_bullpuprifleh4",
                    WeaponModel.CombatShotgun => "w_sg_pumpshotgunh4",
                    WeaponModel.Fertilizercan => "w_ch_jerrycan",
                    WeaponModel.HeavyRifle => "w_ar_heavyrifleh",
                    WeaponModel.EMPLauncher => "w_lr_compactml",
                    WeaponModel.CeramicPistol => "w_pi_ceramic_pistol",
                    WeaponModel.NavyRevolver => "w_pi_wep2_gun",
                    WeaponModel.HazardCan => "w_am_jerrycan_sf",
                    _ => "w_am_case",
                };
            }

            if (baseItem.Category == ItemCategory.Money)
                return "bkr_prop_money_sorted_01";

            if (baseItem.Category == ItemCategory.VehicleKey)
                return "p_car_keys_01";

            if (baseItem.Category == ItemCategory.PropertyKey)
                return "prop_cs_keys_01";

            if (baseItem.Category == ItemCategory.WalkieTalkie)
                return "prop_cs_hand_radio";

            if (baseItem.Category == ItemCategory.Cellphone)
                return "prop_amb_phone";

            if (baseItem.Category == ItemCategory.Weed)
                return "prop_weed_block_01";

            if (baseItem.Category == ItemCategory.Cocaine)
                return "hei_prop_hei_drug_pack_01a";

            if (baseItem.Category == ItemCategory.Crack)
                return "prop_mp_drug_pack_red";

            if (baseItem.Category == ItemCategory.Heroin)
                return "prop_mp_drug_pack_blue";

            if (baseItem.Category == ItemCategory.MDMA)
                return "prop_drug_package_02";

            if (baseItem.Category == ItemCategory.Xanax)
                return "ng_proc_drug01a002";

            if (baseItem.Category == ItemCategory.Oxycontin)
                return "ng_proc_drug01a002";

            if (baseItem.Category == ItemCategory.Metanfetamina)
                return "prop_drug_package";

            if (baseItem.Category == ItemCategory.Boombox)
                return "prop_boombox_01";

            if (baseItem.Category == ItemCategory.Microphone)
                return "prop_microphone_02";

            return string.Empty;
        }

        public static string GetImageName(this BaseItem baseItem)
        {
            if (baseItem.Category == ItemCategory.Weapon)
                return $"1/{baseItem.Type}";

            return ((int)baseItem.Category).ToString();
        }

        public static string GetExtra(this BaseItem baseItem)
        {
            if (string.IsNullOrWhiteSpace(baseItem.Extra))
                return string.Empty;

            if (baseItem.Category == ItemCategory.Weapon)
            {
                var extra = Functions.Deserialize<WeaponItem>(baseItem.Extra);
                var components = string.Join(", ", extra.Components.Select(x => Global.WeaponComponents.FirstOrDefault(y => y.Hash == x)?.Name ?? string.Empty));
                if (!string.IsNullOrWhiteSpace(components))
                    components = $"<br/>Componentes: <strong>{components}</strong>";

                return $"Munição: <strong>{extra.Ammo}</strong><br/>Pintura: <strong>{extra.TintIndex}</strong>{components}";
            }

            if (baseItem.GetIsCloth())
            {
                var extra = Functions.Deserialize<ClotheAccessoryItem>(baseItem.Extra);
                return $"Modelo: <strong>{baseItem.Type}{(!string.IsNullOrWhiteSpace(extra.DLC) ? $" ({extra.DLC})" : string.Empty)}</strong><br/>Textura: <strong>{extra.Texture}</strong>";
            }

            if (baseItem.Category == ItemCategory.PropertyKey
                || baseItem.Category == ItemCategory.VehicleKey)
                return $"Fechadura: <strong>{baseItem.Type}</strong>";

            if (baseItem.Category == ItemCategory.WalkieTalkie)
            {
                var extra = Functions.Deserialize<WalkieTalkieItem>(baseItem.Extra);
                return $"Canal 1: <strong>{extra.Channel1}</strong><br/>Canal 2: <strong>{extra.Channel2}</strong><br/>Canal 3: <strong>{extra.Channel3}</strong><br/>Canal 4: <strong>{extra.Channel4}</strong><br/>Canal 5: <strong>{extra.Channel5}</strong>";
            }

            if (baseItem.Category == ItemCategory.Cellphone)
                return $"Número: <strong>{baseItem.Type}</strong>";

            return string.Empty;
        }

        public static bool GetIsCloth(this BaseItem baseItem)
        {
            return baseItem.Category == ItemCategory.Cloth1 || baseItem.Category == ItemCategory.Cloth3
                    || baseItem.Category == ItemCategory.Cloth4 || baseItem.Category == ItemCategory.Cloth5
                    || baseItem.Category == ItemCategory.Cloth6 || baseItem.Category == ItemCategory.Cloth7
                    || baseItem.Category == ItemCategory.Cloth8 || baseItem.Category == ItemCategory.Cloth9
                    || baseItem.Category == ItemCategory.Cloth10 || baseItem.Category == ItemCategory.Cloth11
                    || baseItem.Category == ItemCategory.Accessory0 || baseItem.Category == ItemCategory.Accessory1
                    || baseItem.Category == ItemCategory.Accessory2 || baseItem.Category == ItemCategory.Accessory6
                    || baseItem.Category == ItemCategory.Accessory7;
        }

        public static bool GetIsStack(this BaseItem baseItem)
        {
            return baseItem.Category == ItemCategory.Money || baseItem.Category == ItemCategory.Weed
                || baseItem.Category == ItemCategory.Cocaine || baseItem.Category == ItemCategory.Crack
                || baseItem.Category == ItemCategory.Heroin || baseItem.Category == ItemCategory.MDMA
                || baseItem.Category == ItemCategory.Xanax || baseItem.Category == ItemCategory.Oxycontin
                || baseItem.Category == ItemCategory.Metanfetamina;
        }
    }
}