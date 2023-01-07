using GTAChaos.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GTAChaos.Effects
{
    public static class EffectDatabase
    {
        private static readonly Dictionary<AbstractEffect, int> EffectCooldowns = new Dictionary<AbstractEffect, int>();
        public static bool ShouldCooldown = true;

        public static WeightedRandomBag<AbstractEffect> Effects { get; } = new WeightedRandomBag<AbstractEffect>();

        private static void AddEffect(AbstractEffect effect, double weight = 1.0)
        {
            if (effect == null)
                return;
            EffectDatabase.EnabledEffects.Add(EffectDatabase.Effects.Add(effect, effect.Word, weight));
        }

        public static void PopulateEffects(string game)
        {
            foreach (Category category in Category.Categories)
                category.ClearEffects();
            EffectDatabase.Effects.Clear();
            EffectDatabase.EnabledEffects.Clear();
            if (game == "san_andreas")
            {
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Aggressive Drivers", "AllDriversAreCriminals", "aggressive_drivers"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Bounty On Your Head", "StopPickingOnMe", "have_a_bounty_on_your_head"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Elvis Is Everywhere", "BlueSuedeShoes", "elvis_is_everywhere"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Everyone Armed", "SurroundedByNutters", "everyone_armed"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Gang Members Everywhere", "OnlyHomiesAllowed", "gang_members_everywhere"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Gangs Control The Streets", "BetterStayIndoors", "gangs_control_the_streets"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Ghost Town", "GhostTown", "ghost_town"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "NPCs Attack Each Other", "RoughNeighbourhood", "npcs_attack_each_other"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "NPCs Attack You", "AttackOfTheVillagePeople", "npcs_attack_you"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Recruit Anyone (9mm)", "WannaBeInMyGang", "recruit_anyone_9mm"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Recruit Anyone (AK-47)", "NoOneCanStopUs", "recruit_anyone_ak47"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Recruit Anyone (Rockets)", "RocketMayhem", "recruit_anyone_rockets"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Riot Mode", "StateOfEmergency", "riot_mode"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Beach Party", "LifesABeach", "beach_theme"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Country Traffic", "HicksVille", "country_traffic"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Funhouse Theme", "CrazyTown", "funhouse_theme"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.NPCs, "Ninja Theme", "NinjaTown", "ninja_theme"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Death", "GoodbyeCruelWorld", "death").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "health_armor_money"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "infinite_ammo"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Invincible (Player)", "NoOneCanHurtMe", "infinite_health_player"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "weapon_set_1"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "weapon_set_2"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "weapon_set_3"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 4", "MinigunMadness", "weapon_set_4"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Beginner Weapon Skills", "BabysFirstGun", "beginner_level_for_all_weapons"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Fat Player", "WhoAteAllThePies", "fat_player"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Hitman Weapon Skills", "ProfessionalKiller", "hitman_level_for_all_weapons"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Hot CJ In Your Area", "HelloLadies", "lock_sex_appeal_at_max"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Lock Respect At Max", "WorshipMe", "lock_respect_at_max"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Max Driving Skills", "NaturalTalent", "max_driving_skills"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Max Lung Capacity", "FilledLungs", "max_lung_capacity"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Max Muscle", "BuffMeUp", "max_muscle"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Max Stamina", "ICanGoAllNight", "max_stamina"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Never Get Hungry", "IAmNeverHungry", "never_get_hungry"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "No Driving Skills", "BackToDrivingSchool", "no_driving_skills"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "No Lung Capacity", "EmptyLungs", "no_lung_capacity"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "No Stamina", "ImAllOutOfBreath", "no_stamina"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Stats, "Skinny Player", "LeanAndMean", "skinny_player"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WantedLevel, "+2 Wanted Stars", "TurnUpTheHeat", "wanted_level_plus_two"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "TurnDownTheHeat", "clear_wanted_level"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.WantedLevel, "Never Wanted", "IDoAsIPlease", "never_wanted"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.WantedLevel, "Six Wanted Stars", "BringItOn", "wanted_level_six_stars").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Foggy Weather", "CantSeeWhereImGoing", 9));
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Overcast Weather", "DullDullDay", 4));
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Rainy Weather", "StayInAndWatchTV", 16));
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Sandstorm", "SandInMyEars", 19));
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Sunny Weather", "PleasantlyWarm", 1));
                EffectDatabase.AddEffect((AbstractEffect)new WeatherEffect("Very Sunny Weather", "TooDamnHot", 0));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "get_jetpack"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "get_parachute"));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("OldSpeedDemon", 504));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("18Holes", 457));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("ItsAllBull", 486));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("NotForPublicRoads", 502));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("JustTryAndStopMe", 503));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("OhDude", 425));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("JumpJet", 520));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("MonsterMash", 556));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("EnergyFiveHundred", 522));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("FourWheelFun", 471));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("DoughnutHandicap", 489));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("SurpriseDriver", -1));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("TimeToKickAss", 432));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("WheresTheFuneral", 442));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("CelebrityStatus", 409));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("FlyingToStunt", 513));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("TrueGrime", 408));
                EffectDatabase.AddEffect((AbstractEffect)new SpawnVehicleEffect("IWantToHover", 539));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "0.25x Game Speed", "MatrixMode", "quarter_game_speed", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "0.5x Game Speed", "SlowItDown", "half_game_speed", multiplier: 2f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "2x Game Speed", "SpeedItUp", "double_game_speed", multiplier: 2f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "4x Game Speed", "YoureTooSlow", "quadruple_game_speed", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "Always Midnight", "NightProwler", "always_midnight"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "Faster Clock", "TimeJustFliesBy", "faster_clock"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "Stop Game Clock", "DontBringOnTheNight", "stop_game_clock"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Time, "Timelapse Mode", "DiscoInTheSky", "timelapse"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "All Cars Have Nitro", "SpeedFreak", "all_cars_have_nitro"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "DontTryAndStopMe", "all_green_lights"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "All Taxis Have Nitrous", "SpeedyTaxis", "all_taxis_have_nitro"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Black Vehicles", "SoLongAsItsBlack", "black_traffic"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "boats_fly"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "cars_float_away_when_hit"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "cars_fly"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "cars_on_water"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "cheap_cars"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "expensive_cars"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Explode All Vehicles", "AllCarsGoBoom", "explode_all_cars").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "insane_handling"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Pink Vehicles", "PinkIsTheNewCool", "pink_traffic"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "smash_n_boom"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.VehiclesTraffic, "Wheels Only, Please", "WheelsOnlyPlease", "wheels_only_please"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.PlayerModifications, "Drive-By Aiming", "IWannaDriveBy", "weapon_aiming_while_driving"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.PlayerModifications, "Infinite Oxygen", "ManFromAtlantis", "infinite_oxygen"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.PlayerModifications, "Huge Bunny Hop", "CJPhoneHome", "huge_bunny_hop"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.PlayerModifications, "Mega Jump", "Kangaroo", "mega_jump"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.PlayerModifications, "Mega Punch", "StingLikeABee", "mega_punch"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "0.5x Effect Speed", "LetsDragThisOutABit", "half_timer_speed", multiplier: 2.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "2x Effect Speed", "LetsDoThisABitFaster", "double_timer_speed", multiplier: 10f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "5x Effect Speed", "LetsDoThisReallyFast", "quintuple_timer_speed", multiplier: 25f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Clear Active Effects", "ClearActiveEffects", "clear_active_effects"), 3.0);
                EffectDatabase.AddEffect((AbstractEffect)new DiscountRapidFireEffect("LIDL Rapid-Fire", "SystemError", "discount_rapid_fire"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Hide Chaos UI", "AsIfNothingEverHappened", "hide_chaos_ui", multiplier: 5f).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new HyperRapidFireEffect("Hyper Rapid-Fire", "SystemCrash", "hyper_rapid_fire"));
                EffectDatabase.AddEffect((AbstractEffect)new RapidFireEffect("Rapid-Fire", "SystemOverload", "rapid_fire"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Reset Effect Timers", "HistoryRepeatsItself", "reset_effect_timers"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Backwards Clock", "TimeJustGoesBackwards", "backwards_clock"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Buttsbot", "ButtsbotYes", "buttsbot", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Custom Textures", "CustomTextures", "textures_custom"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Delayed Controls", "WhatsWrongWithThisKeyboard", "delayed_controls", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "EASY TO READ", "ItsEasierToRead", "very_big_font_scale"));
                EffectDatabase.AddEffect((AbstractEffect)new FakeCrashEffect("Game Crash", "TooManyModsInstalled"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "haha funni font", "ComicSansMasterRace", "font_comic_sans"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "It's Morbin Time", "ItsMorbinTime", "textures_its_morbin_time"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "KIRYU-CHAN!!!", "KiryuChan", "font_yakuza"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Low LOD Vehicles", "AtariVehicles", "low_lod_vehicles"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Mirrored Screen", "WhatsLeftIsRight", "mirrored_screen", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Mirrored World", "LetsTalkAboutParallelUniverses", "mirrored_world"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Night Vision", "NightVision", "night_vision", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Nothing", "ThereIsNoEffect", "nothing"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Visible Water", "OceanManGoneAgain", "no_visible_water"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Water Physics", "FastTrackToAtlantis", "no_water_physics"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pausing", "LetsPause", "pausing", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Queer Rights!", "QueerRights", "replace_all_text_queer_rights", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Quick Sprunk Stop", "ARefreshingDrink", "quick_sprunk_stop"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Random Inputs", "PossessedKeyboard", "random_inputs"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Radar Zoom (Small)", "SmallRadarZoom", "radar_zoom_small"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Radar Zoom (Large)", "LargeRadarZoom", "radar_zoom_large"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Random Stunt Jump", "RandomStuntJump", "random_stunt_jump").DisableRapidFire());
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Reload Autosave", "HereWeGoAgain", "reload_autosave").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Roll Credits", "WaitItsOver", "roll_credits"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Screen Flip", "MuscleMemoryMangler", "screen_flip", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Screensaver HUD", "ScreensaverHUD", "screensaver_hud"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Shoutouts to SimpleFlips.", "ShoutoutsToSimpleFlips", "replace_all_text_simpleflips", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Spawn Ramp", "FreeStuntJump", "spawn_ramp"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Super Smokio 64", "SuperSmokio64", "font_mario_64"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "TABLE!", "TABLE", "spawn_table"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Team Trees", "TeamTrees", "spawn_tree"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "The Mirror Dimension", "ThisSeemsStrange", "the_mirror_dimension", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Thermal Vision", "ThermalVision", "thermal_vision", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Too Much Information", "TooMuchInformation", "too_much_information", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Underwater View", "HelloHowAreYouIAmUnderTheWater", "underwater_view"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Uninstall CS: Source", "UninstallCSS", "textures_counter_strike_source"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Upside-Down Screen", "WhatsUpIsDown", "upside_down_screen", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "High Pitched Audio", "CJAndTheChipmunks", "high_pitched_audio"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pitch Shifter", "VocalRange", "pitch_shifter"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Double Gravity", "KilogramOfFeathers", "double_gravity", multiplier: 2f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Half Gravity", "ImFeelingLightheaded", "half_gravity", multiplier: 2f));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Insane Gravity", "StraightToHell", "insane_gravity", 10000).DisableRapidFire());
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Inverted Gravity", "BeamMeUpScotty", "inverted_gravity", 10000).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Quadruple Gravity", "KilogramOfSteel", "quadruple_gravity", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Quarter Gravity", "GroundControlToMajorTom", "quarter_gravity", multiplier: 1f));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Zero Gravity", "ImInSpaaaaace", "zero_gravity", 10000).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Blind", "WhoTurnedTheLightsOff", "blind", 10000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "disable_hud"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "DVD Screensaver", "ItsGonnaHitTheCorner", "dvd_screensaver", multiplier: 1f).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Freeze Radar", "OutdatedMaps", "freeze_radar"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Blips/Markers/Pickups", "INeedSomeInstructions", "disable_blips_markers_pickups"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Portrait Mode", "PortraitMode", "portrait_mode"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Tunnel Vision", "TunnelVision", "tunnel_vision", multiplier: 1f).DisableRapidFire());
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Fail Current Mission", "MissionFailed", "fail_current_mission").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IllTakeAFreePass", "pass_current_mission"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Fake Pass Current Mission", "IWontTakeAFreePass", "fake_pass_current_mission").SetDisplayName(DisplayNameType.GAME, "Pass Current Mission").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Action Figures", "ILikePlayingWithToys", "action_figures"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Ant Peds", "AntsAntsAntMan", "ped_size_super_tiny"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "ASSERT DOMINANCE", "AssertDominance", "t_pose_peds"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Backwards Peds", "BackwardsPeds", "ped_rotation_backwards"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Big Heads", "BigHeadsMode", "big_heads"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable Headshots", "BulletproofForeheads", "disable_headshots"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "TruePacifist", "disable_all_weapon_damage"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now!", "EverybodyBleedNow", "everybody_bleed_now", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Head Peds", "HeadPeds", "head_peds"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Hold The F Up...", "HoldTheFUp", "hold_the_f_up").SetDisplayName(DisplayNameType.STREAM, "Hold The F*** Up..."));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Inverted Weapon Damage", "HealingBullets", "inverted_weapon_damage"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Invincible (Everyone)", "NoOneCanHurtAnyone", "infinite_health_everyone"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Large Peds", "LargePeepoPeds", "ped_size_large"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Long Live The Rich!", "LongLiveTheRich", "long_live_the_rich"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Long Necks", "ICanSeeMyHouseFromUpHere", "long_necks"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "One Bullet Magazines", "OneInTheChamber", "one_bullet_magazines"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Ped One Hit K.O.", "ILikeToLiveDangerously", "one_hit_ko").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Ped Wallhack", "ICanSeeYouGuysThroughWalls", "ped_wallhack"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Peds Explode If Run Over", "ExplosivePeds", "peds_explode_when_run_over"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Peds Leave All Vehicles", "ImTiredOfDriving", "everyone_leaves_all_vehicles"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rainbow Peds", "TastyUnicornPoop", "rainbow_peds"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Remove Everyone's Weapons", "NoWeaponsAllowed", "remove_everyones_weapons"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Peds (X)", "PedsRotatingOnX", "ped_rotation_continuous_x"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Peds (Y)", "PedsRotatingOnY", "ped_rotation_continuous_y"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Peds (Z)", "PedsRotatingOnZ", "ped_rotation_continuous_z"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Set Everyone On Fire", "HotPotato", "set_everyone_on_fire").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Spawn Dumptrucks", "DamnBoiHeThicc", "big_butts"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Speeeeeen!", "Speeeeeen", "rotating_peds"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Tiny Peds", "TinyPeepoPeds", "ped_size_tiny"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Upside-Down Peds", "InALandDownUnder", "ped_rotation_flipped"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "WAYTOODANK", "WAYTOODANK", "dont_lose_your_head", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Where Is Everybody?", "ImHearingVoices", "where_is_everybody"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Backwards Objects", "ObjectsAreBackwards", "object_rotation_backwards", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Upside-Down Objects", "ObjectsAreUpsideDown", "object_rotation_flipped", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Objects (X)", "ObjectsRotatingOnX", "object_rotation_continuous_x", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Objects (Y)", "ObjectsRotatingOnY", "object_rotation_continuous_y", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Objects (Z)", "ObjectsRotatingOnZ", "object_rotation_continuous_z", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Tiny Objects", "ObjectsAreTiny", "object_size_tiny", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Large Objects", "ObjectsAreLarge", "object_size_large", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Wide Objects", "ObjectsAreWide", "object_size_wide", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Super Wide Objects", "ObjectsAreSuperWide", "object_size_super_wide", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Tall Objects", "ObjectsAreTall", "object_size_tall", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Long Objects", "ObjectsAreLong", "object_size_long", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Paper Thin Objects", "ObjectsArePaperThin", "object_size_paper_thin", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Flat Objects", "ObjectsAreFlat", "object_size_flat", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Cops Everywhere", "TooMuchLawAndOrder", "cops_everywhere"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disarm All NPCs", "LeaveTheGunsToMe", "disarm_all_npcs"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Explode All NPCs", "BoomGoesTheDynamite", "explode_all_npcs").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Give NPCs An RPG", "RocketParty", "give_npcs_an_rpg"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Launch All NPCs", "UpUpAndAway", "launch_all_npcs"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Teleport All NPCs To Player", "WhoAreYouPeople", "teleport_all_npcs_to_player").SetDisplayName(DisplayNameType.STREAM, "TP All NPCs To Player"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Bobcat", "BobcatAllAround", "vehicle_spawns_bobcat"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Caddy", "CaddyAllAround", "vehicle_spawns_caddy"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Combine", "CombineAllAround", "vehicle_spawns_combine"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Infernus", "InfernusAllAround", "vehicle_spawns_infernus"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Kart", "KartAllAround", "vehicle_spawns_kart"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Monster", "MonsterAllAround", "vehicle_spawns_monster"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Mr. Whoopee", "MrWhoopeeAllAround", "vehicle_spawns_whoopee"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Mower", "MowerAllAround", "vehicle_spawns_mower"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Rhino", "RhinoAllAround", "vehicle_spawns_rhino"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Traffic Is Vortex", "VortexAllAround", "vehicle_spawns_vortex"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "-Health, -Armor, -$250k", "INeedSomeHindrance", "anti_health_armor_money"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Add Random Blips", "PointsOfUninterest", "add_random_blips"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Arcade Racer Camera", "SegaRallyChampionship", "arcade_racer_camera", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Bankrupt", "CrashTookAllMyMoney", "bankrupt"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Carl! It's Zero!", "ZeroNeedsYourHelp", "teleport_to_zero").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Cinematic Vehicle Camera", "MachinimaMode", "cinematic_vehicle_camera", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Death (1% Chance)", "TheChanceOfSuddenDeath", "one_percent_death"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable Aiming", "IForgotHowToAim", "disable_aiming"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable One Movement Key", "DisableOneMovementKey", "disable_one_movement_key", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Disable Shooting", "IForgotHowToShoot", "disable_shooting"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Drive Wander", "Autopilot", "drive_wander", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Drunk Player", "DrunkPlayer", "drunk_player"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Experience The Lag", "PacketLoss", "experience_the_lag", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Explosive Bullets", "BombasticImpact", "explosive_bullets"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Eye For An Eye", "EyeForAnEye", "pacifist"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Fire Bullets", "OilOnTheStreets", "fire_bullets"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Flower Power", "FlowerPower", "flower_power"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Force Field", "ForceField", "force_field"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Force Mouse Steering", "ForceVehicleMouseSteering", "force_vehicle_mouse_steering"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Forced Aiming", "ICanOnlyAim", "forced_aiming", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Forced Look Behind", "EyesInTheBack", "forced_look_behind", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Forced Shooting", "ICanOnlyShoot", "forced_shooting", 30000));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Freefall!", "WhereWeDroppingBoys", "freefall").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Galaxy Note 7", "DangerousPhoneCalls", "galaxy_note_7"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "get_busted").DisableRapidFire());
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Get Wasted", "Hospitality", "get_wasted").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Grounded", "ImNotAKangaroo", "disable_jumping"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Gun Game", "ModernWarfare2Lobby", "gun_game"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Instantly Hungry", "IllHave2Number9s", "instantly_hungry"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Inverted Controls", "InvertedControls", "inverted_controls"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "It's Rewind Time!", "ItsRewindTime", "its_rewind_time", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Kick Out Of Vehicle", "ThisAintYourCar", "kick_player_out_of_vehicle"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Let's Take A Break", "LetsTakeABreak", "lets_take_a_break", 10000).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Lock Mouse", "WhoUnpluggedMyMouse", "lock_mouse", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Lock Player In Vehicle", "ThereIsNoEscape", "lock_player_inside_vehicle"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Low FOV", "LowFOV", "low_fov", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Millionaire", "IJustWonTheLottery", "millionaire"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Need To Hurry", "NoNeedToHurry", "no_need_to_hurry", multiplier: 1.5f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Shooting Allowed", "GunsAreDangerous", "no_shooting_allowed"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "No Tasks Allowed", "NoTasksAllowed", "no_tasks_allowed", 10000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pedal To The Metal", "PedalToTheMetal", "pedal_to_the_metal"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Portal Guns", "CaveJohnsonWouldBeProud", "portal_guns"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Quake FOV", "QuakeFOV", "quake_fov"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Random Outfit", "ASetOfNewClothes", "random_outfit"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Reset Camera", "NaturalView", "reset_camera"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "NoWeaponsForYou", "remove_all_weapons"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Remove Current Weapon", "IWillTakeThisGunFromYou", "remove_current_weapon"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Remove Random Weapon", "IWillTakeAGunFromYou", "remove_random_weapon"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Ring Ring !!", "RingRing", "ring_ring", 30000).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Shaky Hands", "IJustCantHoldStill", "shaky_hands"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Shuffle Blips", "ThesePlacesOnceMadeSense", "shuffle_blips"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Solid Water", "JesusInTheHouse", "walk_on_water"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Steer Bias (Left)", "LeftSideBias", "steer_bias_left"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Steer Bias (Right)", "RightSideBias", "steer_bias_right"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Super Low FOV", "SuperLowFOV", "super_low_fov", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "SUPER. HOT.", "SUPERHOT", "superhot"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Switch To Unarmed", "PleaseUseYourFists", "switch_to_unarmed"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "The Firing Circus", "TheFiringCircus", "the_firing_circus", 5000).DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "The Flash", "FastestManAlive", "the_flash"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Vehicle Bumper Camera", "FrontRowSeat", "vehicle_bumper_camera", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Void Warp", "UnderTheMap", "void_warp"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Walk Off", "LetsGoForAWalk", "walk_off", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Warp In Random Vehicle", "ItsYourUber", "warp_player_into_random_vehicle"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Weapon Recoil", "ThoseAreSomeStrongWeapons", "weapon_recoil"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Weapon Roulette", "WeaponRoulette", "weapon_roulette"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "You've been struck by...", "YouveBeenStruckBy", "struck_by_truck"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "You know what to do.", "WhyDidYouBlowUpRydersCar", "blow_up_ryders_car").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Zooming FOV", "ZoomingFOV", "zooming_fov", 30000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Powerpoint Presentation", "PowerpointPresentation", "fps_15", multiplier: 1f));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Smooth Criminal", "SmoothCriminal", "fps_60"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "All Vehicles Alarmy", "SoundTheAlarm", "all_vehicles_alarmy"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Backwards Vehicles", "BackPeepoHappy", "vehicle_rotation_backwards"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Beyblade", "LetItRip", "beyblade"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Carmageddon", "Carmageddon", "carmageddon"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Delete All Vehicles", "GoodbyeAllSweetRides", "delete_all_vehicles").DisableRapidFire());
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Delete Vehicle", "GoodbyeSweetRide", "delete_vehicle").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Do A Kickflip!", "DoAKickflip", "kickflip"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Explode Random Vehicle", "OneCarGoesBoom", "explode_random_vehicle").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Flat Vehicles", "FlatPeepoHappy", "vehicle_size_flat"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Flipped Vehicles", "FlippedPeepoHappy", "vehicle_rotation_flipped"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Freeze Vehicle", "StuckInTime", "freeze_vehicle", 5000));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Ghost Vehicles", "InvisibleVehicles", "invisible_vehicles"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "ghost_rider"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "VeryDampNoBounce", "high_suspension_damping"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "HONK!!!", "HONKHONK", "honk_vehicle", 30000).SetAudioVariations(5));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Honk Boost", "GottaHonkFast", "honk_boost"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Hot Wheels", "HotWheelsRacing", "vehicle_size_super_tiny"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Ignite Current Vehicle", "WayTooHot", "set_current_vehicle_on_fire").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Invert Vehicle Speed", "LetsGoBack", "invert_vehicle_speed"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Large Vehicles", "LargePeepoHappy", "vehicle_size_large"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Long Vehicles", "LongPeepoHappy", "vehicle_size_long"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Oh Hey, Tanks!", "OhHeyTanks", "oh_hey_tanks"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Paper Thin Vehicles", "PaperPeepoHappy", "vehicle_size_paper_thin"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "TiresBeGone", "pop_tires_of_all_vehicles"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Pride Vehicles", "AllColorsAreBeautiful", "pride_traffic"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Relative Car Gravity", "SpiderCars", "vehicle_driving_on_walls"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rota(to)ry Engines", "RotatoryEngines", "vehicle_rotation_based_on_speed"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Vehicles (X)", "RotatePeepoHappyX", "vehicle_rotation_continuous_x"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Vehicles (Y)", "RotatePeepoHappyY", "vehicle_rotation_continuous_y"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Rotating Vehicles (Z)", "RotatePeepoHappyZ", "vehicle_rotation_continuous_z"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Send Vehicles To Space", "StairwayToHeaven", "send_vehicles_to_space"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Speed (1994)", "KeepYourPace", "minimum_speed"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Super Wide Vehicles", "WiderPeepoHappy", "vehicle_size_super_wide"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Swap Vehicles On Impact", "SwapVehiclesOnImpact", "swap_vehicles_on_impact"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Tall Vehicles", "TallPeepoHappy", "vehicle_size_tall"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Tiny Vehicles", "TinyPeepoHappy", "vehicle_size_tiny"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnAround", "turn_vehicles_around"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Unflippable Vehicles", "ThereGoesMyBurrito", "unflippable_vehicles"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Vehicle Boost", "FullForceForward", "vehicle_boost"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Vehicle One Hit K.O.", "NoDings", "vehicle_one_hit_ko"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Wide Vehicles", "WidePeepoHappy", "vehicle_size_wide"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Your Car Floats When Hit", "ImTheBubbleCar", "your_car_floats_away_when_hit"));
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "LowrideAllNight", "zero_suspension_damping"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.CustomEffects, "Always Wanted", "ICanSeeStars", "always_wanted").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FakeTeleportEffect("Fake Teleport", "HahaGotYourNose"));
                EffectDatabase.AddEffect(new FunctionEffect(Category.Teleportation, "Random Teleport", "LetsGoSightseeing", "random_teleport").DisableRapidFire());
                EffectDatabase.AddEffect((AbstractEffect)new FunctionEffect(Category.Teleportation, "Teleport To Waypoint", "IKnowJustTheRightPlace", "teleport_to_marker", 30000));
                EffectDatabase.AddEffect(new FunctionEffect(Category.Teleportation, "Teleport To Liberty City", "LetsTalkAboutTheMultiverse", "teleport_to_liberty_city").DisableRapidFire());
                foreach (Location location in Location.Locations)
                    EffectDatabase.AddEffect((AbstractEffect)new TeleportationEffect(location));
            }
            else
            {
                int num = game == "vice_city" ? 1 : 0;
            }
            EffectDatabase.Effects.Sort((Comparison<WeightedRandomBag<AbstractEffect>.Entry>)((first, second) => string.Compare(first.item.GetDisplayName(DisplayNameType.UI), second.item.GetDisplayName(DisplayNameType.UI), StringComparison.CurrentCultureIgnoreCase)));
            foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
            {
                int length = entry.item.GetDisplayName(DisplayNameType.STREAM).Length;
                if (entry.item.Word == "MatrixMode") { int t = 9; }
            }
            AudioPlayer.INSTANCE.CreateAndPrintAudioFileReadme();
            //File.WriteAllText("C:\\Effects.json", JsonConvert.SerializeObject(Effects, Formatting.Indented));
        }

        public static WeightedRandomBag<AbstractEffect> EnabledEffects { get; } = new WeightedRandomBag<AbstractEffect>();

        public static int GetEnabledEffectsCount() => EffectDatabase.EnabledEffects.Count;

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false) => (onlyEnabled ? EffectDatabase.EnabledEffects : EffectDatabase.Effects).Find((Predicate<WeightedRandomBag<AbstractEffect>.Entry>)(e => e.item.GetID().Equals(id))).item;

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false) => (onlyEnabled ? EffectDatabase.EnabledEffects : EffectDatabase.Effects).Find((Predicate<WeightedRandomBag<AbstractEffect>.Entry>)(e => !string.IsNullOrEmpty(e.item.Word) && string.Equals(e.item.Word, word, StringComparison.OrdinalIgnoreCase))).item;

        public static AbstractEffect GetByDescription(
          string description,
          bool onlyEnabled = false)
        {
            return (onlyEnabled ? EffectDatabase.EnabledEffects : EffectDatabase.Effects).Find((Predicate<WeightedRandomBag<AbstractEffect>.Entry>)(e => string.Equals(description, e.item.GetDisplayName(DisplayNameType.UI), StringComparison.OrdinalIgnoreCase))).item;
        }

        public static AbstractEffect GetRandomEffect(
          bool onlyEnabled = false,
          int attempts = 0,
          bool addEffectToCooldown = false,
          bool is_youtube = false,
          string youtube = "")
        {
            WeightedRandomBag<AbstractEffect> weightedRandomBag = onlyEnabled ? EffectDatabase.EnabledEffects : EffectDatabase.Effects;
            if (weightedRandomBag.Count <= 0)
                return (AbstractEffect)null;
            (bool success, AbstractEffect abstractEffect) = weightedRandomBag.GetRandom(RandomHandler.Random, (Func<WeightedRandomBag<AbstractEffect>.Entry, bool>)(entry => !EffectDatabase.EffectCooldowns.ContainsKey(entry.item)), is_youtube: is_youtube, youtube_name: youtube);
            if (!success || abstractEffect == null || attempts++ > 10)
            {
                EffectDatabase.ResetEffectCooldowns();
                return EffectDatabase.GetRandomEffect(onlyEnabled, attempts, addEffectToCooldown);
            }
            if (!onlyEnabled && addEffectToCooldown)
                EffectDatabase.SetCooldownForEffect(abstractEffect);
            return abstractEffect;
        }

        private static void CheckForNonCooldownEffects()
        {
            if (EffectDatabase.Effects.Get().Where<WeightedRandomBag<AbstractEffect>.Entry>((Func<WeightedRandomBag<AbstractEffect>.Entry, bool>)(entry => EffectDatabase.EffectCooldowns.ContainsKey(entry.item))).Count<WeightedRandomBag<AbstractEffect>.Entry>() <= Config.GetEffectCooldowns())
                return;
            EffectDatabase.ResetEffectCooldowns();
        }

        public static void CooldownEffects()
        {
            if (!EffectDatabase.ShouldCooldown)
                return;
            lock (EffectDatabase.EffectCooldowns)
            {
                foreach (KeyValuePair<AbstractEffect, int> keyValuePair in EffectDatabase.EffectCooldowns.ToList<KeyValuePair<AbstractEffect, int>>())
                {
                    if (EffectDatabase.EffectCooldowns[keyValuePair.Key]-- <= 0)
                        EffectDatabase.EffectCooldowns.Remove(keyValuePair.Key);
                }
            }
            EffectDatabase.CheckForNonCooldownEffects();
        }

        public static void ResetEffectCooldowns()
        {
            lock (EffectDatabase.EffectCooldowns)
                EffectDatabase.EffectCooldowns.Clear();
        }

        public static void SetCooldownForEffect(AbstractEffect effect, int cooldown = -1)
        {
            if (!EffectDatabase.ShouldCooldown || effect == null || !effect.IsCooldownable())
                return;
            if (cooldown < 0)
                cooldown = Math.Max(0, Config.GetEffectCooldowns());
            cooldown = Math.Min(cooldown, Config.GetEffectCooldowns());
            lock (EffectDatabase.EffectCooldowns)
                EffectDatabase.EffectCooldowns[effect] = cooldown;
        }

        public static AbstractEffect RunEffect(string id, bool onlyEnabled = true) => EffectDatabase.RunEffect(EffectDatabase.GetByID(id, onlyEnabled));

        public static AbstractEffect RunEffect(
          AbstractEffect effect,
          int seed = -1,
          int duration = -1)
        {
            if (effect != null)
            {
                effect.RunEffect(seed, duration);
                //EffectDatabase.SetCooldownForEffect(effect); // посмотреть
                EffectDatabase.CooldownEffects();
            }
            return effect;
        }

        public static void SetEffectEnabled(AbstractEffect effect, bool enabled)
        {
            if (effect == null)
                return;
            WeightedRandomBag<AbstractEffect>.Entry entry = EffectDatabase.Effects.Find((Predicate<WeightedRandomBag<AbstractEffect>.Entry>)(e => e.item.Equals((object)effect)));
            if (entry.item == null)
                return;
            if (!enabled && EffectDatabase.EnabledEffects.Contains(entry))
            {
                EffectDatabase.EnabledEffects.Remove(entry);
            }
            else
            {
                if (!enabled || EffectDatabase.EnabledEffects.Contains(entry))
                    return;
                EffectDatabase.EnabledEffects.Add(entry);
            }
        }
    }
}
