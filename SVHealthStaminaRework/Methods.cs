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
        private static Dictionary<int, Tool> GetTools(Farmer f)
        {
            Dictionary<int, Tool> tools = new();
            for (int i = 0; i < 12; i++)
            {
                if (f.Items[i] is Tool)
                    tools.Add(i, (Tool)f.Items[i]);
            }
            return tools;
        }

        private static bool CheckTool(Farmer f, Type type)
        {
            if (f.CurrentTool is null)
                return false;

            if (type == null)
            {
                return f.CurrentTool.GetType() == typeof(MeleeWeapon) && (f.CurrentTool as MeleeWeapon).isScythe();
            }
            if (type == typeof(MeleeWeapon))
            {
                return f.CurrentTool.GetType() == typeof(MeleeWeapon) && !(f.CurrentTool as MeleeWeapon).isScythe();
            }
            return f.CurrentTool.GetType() == type;
        }

        private static void CalculateStamina(Farmer f, int power)
        {
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
