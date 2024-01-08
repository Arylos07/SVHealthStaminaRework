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

        private static float CalculateStamina(Farmer f, int power)
        {
            if (Config.DisableWateringStamina)
            {
                return 0;
            }
            else
            {
                return (float)(2 * (power + 1)) - (float)f.FarmingLevel * 0.1f;
            }
        }
    }
}
