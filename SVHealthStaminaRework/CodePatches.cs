using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using xTile.Dimensions;


namespace SVHealthStaminaRework
{
    public partial class ModEntry
    {
        [HarmonyPatch(typeof(WateringCan), nameof(WateringCan.DoFunction))]
        public class WateringCan_DoFunction_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                SMonitor.Log($"Transpiling WateringCan.DoFunction");

                var codes = new List<CodeInstruction>(instructions);

                bool found = false;
                int index = -1;

                for (int i = 0; i < codes.Count; i++)
                {
                    //SMonitor.Log(codes[i].ToString());
                    if ((codes[i].opcode == OpCodes.Callvirt &&
                            (MethodInfo)codes[i].operand == AccessTools.Method("StardewValley.Farmer:get_Stamina")) &&
                        (codes[i + 8].opcode == OpCodes.Callvirt &&
                            (MethodInfo)codes[i + 8].operand == AccessTools.Method("StardewValley.Farmer:get_FarmingLevel"))) //detect (float)(2 * (power + 1)) - (float)who.FarmingLevel * 0.1f; here
                    {
                        index = i;
                        found = true;
                        SMonitor.Log("Replacing Watering Can Stamina calculation");
                        codes.RemoveRange(i-1, 16); //remove current calculation instructions
                        // insert new instructions from Calculate Stamina

                        //TODO: this does not take the tool power parameter from the DoFunction method; instead it gets tool power again from player.
                        //This means that if there are any modifications to tool power, this function ignores it.
                        //The best fix is to figure out why power is not being passed to this function and use that parameter. Other mods might modify toolPower directly
                        // but those that modify it inline might not work.
                        codes.Insert(i-1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ModEntry), nameof(ModEntry.CalculateStamina))));
                        // end
                        break;
                    }
                }

                /*
                 * Debug log to view IL stack
                for (int i = index - 5; i < index + 15; i++)
                {
                    SMonitor.Log($"Index {i}: {codes[i]}");
                }
                */

                if (found) SMonitor.Log($"WateringCan.DoFunction Transpile = {found}");
                else SMonitor.Log($"Failed Transpile: WateringCan.DoTranspile", LogLevel.Error);

                return codes.AsEnumerable();
            }
        }
    }
}
