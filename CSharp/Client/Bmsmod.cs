using System.Reflection;
using Barotrauma.Extensions;
using Microsoft.Xna.Framework;
using HarmonyLib;
using System.Linq;
using Barotrauma;
using BmsUtils;

// debug
// using System.Diagnostics;

namespace Bms_Harmony
{
    partial class BmsHarmony : ACsMod
    {
        const string harmony_id = "com.Bms.Harmony";
        private readonly Harmony harmony;

        public override void Stop()
        {
            harmony.UnpatchAll(harmony_id);
        }

        public BmsHarmony()
        {
            harmony = new Harmony(harmony_id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Barotrauma.DebugConsole.AddWarning("Loaded BmsHarmony");
        }

        [HarmonyPatch(typeof(Barotrauma.Items.Components.ItemContainer))]
        class Patch_MergeStacks
        {
            static MethodBase TargetMethod()
            {
                Barotrauma.DebugConsole.AddWarning("Patch_MergeStacks TargetMethod");
                return AccessTools.Method(typeof(Barotrauma.Items.Components.ItemContainer), "MergeStacks");
            }
            static bool Prefix(Barotrauma.Items.Components.ItemContainer __instance)
            {
                for (int i = 0; i < __instance.Inventory.Capacity - 1; i++)
                {
                    var items = __instance.Inventory.GetItemsAt(i).ToList();
                    if (items.None()) { continue; }
                    if (items.First().Prefab.Identifier.ToString() == "StackBox" &&
                        items.First().OwnInventories.First().AllItemsMod.Any())
                    {
                        for (int j = 0; j < __instance.Inventory.Capacity - 1; j++)
                        {
                            var items2 = __instance.Inventory.GetItemsAt(j).ToList();
                            if (items2.None()) { continue; }
                            if (items2.First().Prefab.Identifier.ToString() != "StackBox")
                            {
                                items2.ForEach(it => __instance.Inventory.TryPutItem(it, i, allowSwapping: false, allowCombine: true, user: null, createNetworkEvent: false));
                                continue;
                            }

                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Barotrauma.Items.Components.ItemContainer))]
        class Patch_CreateGUI
        {
            static MethodBase TargetMethod()
            {
                Barotrauma.DebugConsole.AddWarning("Patch_CreateGUI TargetMethod");
                return AccessTools.Method(typeof(Barotrauma.Items.Components.ItemContainer), "CreateGUI");
            }

            static void Postfix(Barotrauma.Items.Components.ItemContainer __instance)
            {
                if (__instance.Inventory.Capacity > 1)
                {
                    var layoutGroup = __instance.GuiFrame.FindChild(c => c is Barotrauma.GUILayoutGroup, recursive: true);
                    new GUIButton(new RectTransform(Vector2.One, layoutGroup.RectTransform, scaleBasis: ScaleBasis.Smallest), style: "PushButton")
                    {
                        ToolTip = TextManager.Get("bms.pushicon"),
                        OnClicked = (btn, userdata) =>
                        {
                            Util.PushItems(true);
                            return true;
                        }
                    };
                }
            }
        }
    }
}
