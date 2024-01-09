using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using StardewValley.Locations;

namespace SVHealthStaminaRework
{
    public partial class ModEntry
    {
        private static void CalculateStamina(Farmer f)
        {
            int power = f.toolPower;
            SMonitor.Log($"Calculate Stamina (f = {f.Name}, {power}, {Config.DisableWateringStamina})");
            if (Config.DisableWateringStamina)
            {
                //explicit do nothing
                SMonitor.Log("Nothing done");
                return;
            }
            else
            {
                //default calculation in-game

                float cost = (float)(2 * (power + 1)) - (float)f.FarmingLevel * 0.1f;
                SMonitor.Log($"(float)(2 * ({power} + 1)) - (float){f.FarmingLevel} * 0.1f = {cost}");

                f.Stamina -= cost;
            }
        }
    }
}
