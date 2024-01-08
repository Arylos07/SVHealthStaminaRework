﻿using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SVHealthStaminaRework.Config;
using StardewModdingAPI.Utilities;
using SVHealthStaminaRework;
using HarmonyLib;

//Modified from JessiebotX/StardewValleyMods/HealthStaminaRegen
// took the regen design and applied other features to it.

namespace SVHealthStaminaRework
{
    public partial class ModEntry : Mod
    {
        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;

        private bool DebugLogging;

        private int SecondsUntilHealthRegen = 0;
        private int SecondsUntilStaminaRegen = 0;

        private PerScreen<int> LastHealth = new PerScreen<int>();
        private PerScreen<float> LastStamina = new PerScreen<float>();

        public override void Entry(IModHelper helper)
        {
            /* Read config */
            Config = helper.ReadConfig<ModConfig>();

            //attach instances
            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            /* Hook events */
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;

            /* Console Commands */
            this.Helper.ConsoleCommands.Add("healthstaminaregen_confighelp", "shows config.json document", ConfigHelpCommand);
            this.Helper.ConsoleCommands.Add("healthstaminaregen_debuglogging", "Note that you have to restart in order to apply this change.\n" +
                "This command is for when you have found a bug and want to report it " +
                "(including the parsed log(see https://log.smapi.io))",
                this.DebugLoggingCommand);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // integration with Generic Mod Config Menu            
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                this.Monitor.Log("SVHealthStaminaRework: Generic Mod Config Menu not installed. No integration needed", LogLevel.Info);
                return;
            }

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Disable energy consumption on watering",
                getValue: () => Config.DisableWateringStamina,
                setValue: value => Config.DisableWateringStamina = value,
                tooltip: () => "If enabled, watering plants with the watering can will no longer consume stamina.\n" +
                "If disabled, normal stamina deduction takes place based on farming level."
            );

            this.Monitor.Log("SVHealthStaminaRework: Generic Mod Config Menu loaded successfully.", LogLevel.Info);
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!Context.IsPlayerFree)
                return;

            this.HealthRegen(e);
            this.StaminaRegen(e);

            if (e.IsMultipleOf(60) && this.DebugLogging == true)
                this.Monitor.Log("1 second has passed", LogLevel.Trace);
        }

        private void HealthRegen(UpdateTickedEventArgs e)
        {
            if (Config.Health.Enabled)
            {
                if (e.IsMultipleOf((uint)Config.Health.RegenRateInSeconds * 60))
                {
                    if (!Config.Health.DontCheckConditions)
                    {
                        // if player took damage
                        if (Game1.player.health < this.LastHealth.Value)
                            this.SecondsUntilHealthRegen = Config.Health.SecondsUntilRegenWhenTakenDamage;
                        //timer
                        else if (this.SecondsUntilHealthRegen > 0)
                            this.SecondsUntilHealthRegen--;
                        //regen
                        else if (this.SecondsUntilHealthRegen <= 0)
                            if (Game1.player.health < Game1.player.maxHealth)
                                Game1.player.health = Math.Min(Game1.player.maxHealth, Game1.player.health + Config.Health.HealthPerRegenRate);

                        this.LastHealth.Value = Game1.player.health;

                        if (this.DebugLogging)
                        {
                            this.Monitor.Log("Health Updated", LogLevel.Debug);
                            this.Monitor.Log($"Last Health: {LastHealth.ToString()} ");
                        }
                    }

                    else
                    {
                        Game1.player.health += Config.Health.HealthPerRegenRate;

                        if (this.DebugLogging)
                            this.Monitor.Log("Health Updated (No Regen Delay)", LogLevel.Debug);
                    }
                }
            }
        }

        private void StaminaRegen(UpdateTickedEventArgs e)
        {
            if (Config.Stamina.Enabled)
            {
                if (e.IsMultipleOf((uint)Config.Stamina.RegenRateInSeconds * 60))
                {
                    if (!Config.Stamina.DontCheckConditions)
                    {
                        // if player used stamina
                        if (Game1.player.Stamina < this.LastStamina.Value)
                            this.SecondsUntilStaminaRegen = Config.Stamina.SecondsUntilRegenWhenUsedStamina;
                        //timer
                        else if (this.SecondsUntilStaminaRegen > 0)
                            this.SecondsUntilStaminaRegen--;
                        // regen
                        else if (this.SecondsUntilStaminaRegen <= 0)
                            if (Game1.player.Stamina < Game1.player.MaxStamina)
                                Game1.player.Stamina = Math.Min(Game1.player.MaxStamina, Game1.player.Stamina + Config.Stamina.StaminaPerRegenRate);

                        this.LastStamina.Value = Game1.player.Stamina;

                        if (this.DebugLogging)
                        {
                            this.Monitor.Log("Stamina Updated", LogLevel.Debug);
                            this.Monitor.Log($"Last Stamina: {LastStamina.ToString()}");
                        }
                    }

                    else
                    {
                        Game1.player.Stamina += Config.Stamina.StaminaPerRegenRate;

                        if (this.DebugLogging)
                            this.Monitor.Log("Stamina Updated (No Regen Delay)", LogLevel.Debug);
                    }
                }
            }
        }

        private void ConfigHelpCommand(string command, string[] args)
        {
            //TODO: edit help command
            /*
            this.Monitor.Log(
                "See https://github.com/JessebotX/StardewMods/tree/master/HealthStaminaRegen#configure for the full config.json documentation.\n\n" +
                "(If you dont see the config.json in the HealthStaminaRegen folder, you have to run the game once with this mod installed for it to generate)",
                LogLevel.Info
            );
            */
        }

        private void DebugLoggingCommand(string command, string[] args)
        {
            this.DebugLogging = true;
        }

        /*
        private void HealthConfigImplementation(IGenericModConfigMenuApi api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Health Regeneration", "Allows your health to be modified by HealthPerRegenRate",
                () => Config.Health.Enabled, (bool val) => Config.Health.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Health Per Regen Rate", "The amount of health you get every <Regen Rate> amount of seconds. Must not contain any decimal values",
                () => Config.Health.HealthPerRegenRate, (int val) => Config.Health.HealthPerRegenRate = val);
            api.RegisterSimpleOption(this.ModManifest, "Health Regen Rate", "The seconds in between regeneration. Number must be greater than 0 and must not contain decimal values",
                () => Config.Health.RegenRateInSeconds, (int val) => Config.Health.RegenRateInSeconds = val);
            api.RegisterSimpleOption(this.ModManifest, "Seconds Until Health Regen After Taking Damage",
                "the cooldown for regen to start again after taking damage, set it to 0 if you don't want a regen cooldown",
                () => Config.Health.SecondsUntilRegenWhenTakenDamage, (int val) => Config.Health.SecondsUntilRegenWhenTakenDamage = val);
            api.RegisterSimpleOption(this.ModManifest, "Don't Check Health Regen Conditions",
                "Keep regenerating regardless if it goes past max health, ignores SecondsUntilRegen... etc. " +
                "\n(eg. this allows you to be able to create some sort of hunger mod where you have a negative number set for Health Per Regen Rate; " +
                "therefore forces you to eat or you may die)",
                () => Config.Health.DontCheckConditions, (bool val) => Config.Health.DontCheckConditions = val);
        }

        private void StaminaConfigImplementation(IGenericModConfigMenuApi api)
        {
            api.RegisterSimpleOption(this.ModManifest, "Enable Stamina Regeneration", "Allows your stamina to be modified by StaminaPerRegenRate",
                () => Config.Stamina.Enabled, (bool val) => Config.Stamina.Enabled = val);
            api.RegisterSimpleOption(this.ModManifest, "Stamina Per Regen Rate", "The amount of stamina you get every <Regen Rate> seconds. Decimal values accepted",
                () => Config.Stamina.StaminaPerRegenRate, (float val) => Config.Stamina.StaminaPerRegenRate = val);
            api.RegisterSimpleOption(this.ModManifest, "Stamina Regen Rate", "The seconds in between regeneration. Number must be greater than 0 and must not contain decimal values",
                () => Config.Stamina.RegenRateInSeconds, (int val) => Config.Stamina.RegenRateInSeconds = val);
            api.RegisterSimpleOption(this.ModManifest, "Seconds Until Stamina Regen After Using Stamina",
                "the cooldown for regen to start again after using stamina, set it to 0 if you don't want a regen cooldown",
                () => Config.Stamina.SecondsUntilRegenWhenUsedStamina, (int val) => Config.Stamina.SecondsUntilRegenWhenUsedStamina = val);
            api.RegisterSimpleOption(this.ModManifest, "Don't Check Stamina Regen Conditions",
                "Keep regenerating regardless if it goes past max stamina, ignores SecondsUntilRegen... etc. " +
                "\n(eg. this allows you to be able to create some sort of hunger mod where you have a negative number set for Stamina Per Regen Rate; " +
                "therefore forces you to eat or you will run out of stamina and get over-exertion)",
                () => Config.Stamina.DontCheckConditions, (bool val) => Config.Stamina.DontCheckConditions = val);
        }
        */
    }
}
