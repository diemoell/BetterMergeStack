using System.Linq;
using Barotrauma;
using Barotrauma.Items.Components;

using Barotrauma.Extensions;
using System.Collections.Generic;

namespace BmsUtils
{
    public static class Util
    {
        private static List<int> GetStackBoxIndex(ItemInventory inv)
        {
            var stackBoxsIndex = new List<int>();
            for (var i = 0; i < inv.Capacity; i++)
            {
                var items = inv.GetItemsAt(i).ToList();
                if (items.None()) { continue; }
                if (items.First().Prefab.Identifier.ToString() == "StackBox" && items.First().OwnInventories.First().AllItemsMod.Any())
                {
                    stackBoxsIndex.Add(i);
                }
            }
            return stackBoxsIndex;
        }
        public static void PushItems(bool toStackBox = false)
        {

            var controlCharacter = Character.Controlled;
            var selectedContainer = controlCharacter.SelectedItem?.GetComponent<ItemContainer>();
            var leftHandItems = controlCharacter.Inventory.GetItemsAt(5).FirstOrDefault()?.OwnInventory;
            var rightHandItems = controlCharacter.Inventory.GetItemsAt(6).FirstOrDefault()?.OwnInventory;

            if (leftHandItems != null)
            {
                for (var i = 0; i < leftHandItems.Capacity; i++)
                {
                    foreach (var item in leftHandItems.GetItemsAt(i).ToList())
                    {
                        if (toStackBox)
                        {
                            foreach (var boxIndex in GetStackBoxIndex(selectedContainer.Inventory))
                            {
                                selectedContainer.Inventory.TryPutItem(item, boxIndex, allowSwapping: false, allowCombine: true, user: null, createNetworkEvent: true);
                            }

                        }
                        else
                        {
                            selectedContainer.Inventory.TryPutItem(item, controlCharacter, createNetworkEvent: true, ignoreCondition: true);
                        }
                    }
                }
            }
            if (rightHandItems != null)
            {
                for (var i = 0; i < rightHandItems.Capacity; i++)
                {
                    foreach (var item in rightHandItems.GetItemsAt(i).ToList())
                    {
                        if (toStackBox)
                        {
                            foreach (var boxIndex in GetStackBoxIndex(selectedContainer.Inventory))
                            {
                                selectedContainer.Inventory.TryPutItem(item, boxIndex, allowSwapping: false, allowCombine: true, user: null, createNetworkEvent: true);
                            }

                        }
                        else
                        {
                            selectedContainer.Inventory.TryPutItem(item, controlCharacter, createNetworkEvent: true, ignoreCondition: true);
                        }
                    }
                }
            }
        }
    }
}
