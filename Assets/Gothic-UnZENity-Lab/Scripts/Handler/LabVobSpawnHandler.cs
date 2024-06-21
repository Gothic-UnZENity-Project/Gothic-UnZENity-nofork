using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GUZ.Core.Caches;
using GUZ.Core.Context;
using GUZ.Core.Creator.Meshes.V2;
using GUZ.Core.Globals;
using GUZ.Core.Vm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZenKit.Daedalus;

namespace GUZ.Lab.Handler
{
    public class LabVobSpawnHandler: MonoBehaviour, ILabHandler
    {
        public TMP_Dropdown vobCategoryDropdown;
        public TMP_Dropdown vobItemDropdown;
        public GameObject itemSpawnSlot;
        public bool deletePreviousItem;
        public Toggle deletePreviousItemToggle;

        private string currentItemName;

        private Dictionary<string, ItemInstance> items = new();

        public void Bootstrap()
        {
            /*
             * 1. Load Vdfs
             * 2. Load VobItemAttachPoints json
             * 3. Load Vob name list
             * 4. Fill dropdown
             */
            var itemNames = GameData.GothicVm.GetInstanceSymbols("C_Item").Select(i => i.Name).ToList();

            items = itemNames
                .ToDictionary(itemName => itemName, AssetCache.TryGetItemData);

            vobCategoryDropdown.options = items
                .Select(item => ((VmGothicEnums.ItemFlags)item.Value.MainFlag).ToString())
                .Distinct()
                .Select(flag => new TMP_Dropdown.OptionData(flag))
                .ToList();

            CategoryDropdownValueChanged();
        }

        public void CategoryDropdownValueChanged()
        {
            Enum.TryParse<VmGothicEnums.ItemFlags>(vobCategoryDropdown.options[vobCategoryDropdown.value].text, out var category);
            var items = this.items.Where(item => item.Value.MainFlag == (int)category).ToList();
            vobItemDropdown.options = items.Select(item => new TMP_Dropdown.OptionData(item.Key)).ToList();
        }

        public void LoadVobOnClick()
        {
            // We want to have one element only.
            if (itemSpawnSlot.transform.childCount != 0 && deletePreviousItem)
                Destroy(itemSpawnSlot.transform.GetChild(0).gameObject);

            StartCoroutine(LoadVobOnClickDelayed());
        }

        public void SetDeletePreviousItem()
        {
            deletePreviousItem = deletePreviousItemToggle.isOn;
        }

        private IEnumerator LoadVobOnClickDelayed()
        {
            // Wait 1 frame for GOs to be destroyed.
            yield return null;

            currentItemName = vobItemDropdown.options[vobItemDropdown.value].text;
            var item = CreateItem(currentItemName);
        }

        private GameObject CreateItem(string itemName)
        {
            var itemPrefab = PrefabCache.TryGetObject(PrefabCache.PrefabType.VobItem);
            var item = AssetCache.TryGetItemData(itemName);
            var mrm = AssetCache.TryGetMrm(item.Visual);
            var itemGo = MeshFactory.CreateVob(item.Visual, mrm, default, default, true,
                rootGo: itemPrefab, parent: itemSpawnSlot, useTextureArray: false);

            GUZContext.InteractionAdapter.AddItemComponent(itemGo, true);

            return gameObject;
        }
    }
}