using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVHealthStaminaRework.Config.ConfigOptions
{
    public class StaminaConfig
    {
        public bool Enabled { get; set; } = true;
        public float StaminaPerRegenRate { get; set; } = 2f;
        public int RegenRateInSeconds { get; set; } = 30;
        public int SecondsUntilRegenWhenUsedStamina { get; set; } = 60;
        [Obsolete("This mechanic serves little purpose. Use Health.Enabled instead")]
        //might leave this for compatibility with other mods
        public bool DontCheckConditions { get; set; } = false;

        //leveling
        public bool StaminaLevelingEnabled { get; set; } = true;
        //scaling of how much experience is gained for stamina used.
        public float ExperienceScaling { get; set; } = 0.2f;
        // scaling of how much experience increases stamina.
        public float StaminaScaling { get; set; } = 0.1f;
        public float MaxExperience { get; set; } = 100f;
    }
}