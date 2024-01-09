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
        public bool DontCheckConditions { get; set; } = false;
    }
}