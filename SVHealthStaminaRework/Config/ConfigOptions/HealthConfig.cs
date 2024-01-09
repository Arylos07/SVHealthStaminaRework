using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVHealthStaminaRework.Config.ConfigOptions
{
    public class HealthConfig
    {
        public bool Enabled { get; set; } = true;
        public int HealthPerRegenRate { get; set; } = 2;
        public int RegenRateInSeconds { get; set; } = 30;
        public int SecondsUntilRegenWhenTakenDamage { get; set; } = 60;
        [Obsolete("This mechanic serves little purpose. Use Health.Enabled instead")]
        public bool DontCheckConditions { get; set; } = false;
    }
}