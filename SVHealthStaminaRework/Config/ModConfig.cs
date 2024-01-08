using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVHealthStaminaRework.Config.ConfigOptions;

namespace SVHealthStaminaRework.Config
{
    public class ModConfig
    {
        public HealthConfig Health { get; set; } = new HealthConfig();
        public StaminaConfig Stamina { get; set; } = new StaminaConfig();
        public bool DisableWateringStamina { get; set; } = true;
    }
}