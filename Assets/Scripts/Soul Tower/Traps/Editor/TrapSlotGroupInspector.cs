using Shears;
using Shears.Editor;
using Shears.Logging;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Traps.Editor
{
    public class TrapSlotGroupInspector : VisualElement
    {
        private readonly TrapSlot defaultSlot;
        private readonly TrapSlotGroup group;
        private readonly Action<TileSubgroup> deleteCallback;
        private readonly List<TrapSlot> slots = new();

        private SerializedProperty slotProp;
        private SerializedProperty countProp;
        private SerializedProperty tileTypeProp;
        private SerializedProperty rotationProp;

        public TrapSlotGroupInspector(TrapSlot defaultSlot, TrapSlotGroup group, Action<TileSubgroup> deleteCallback)
        {
            this.defaultSlot = defaultSlot;
            this.group = group;
            this.deleteCallback = deleteCallback;

            CreateInspector();
        }

        private void CreateInspector()
        {
            var groupSO = new SerializedObject(group);

            slotProp = groupSO.FindProperty("slot");
            countProp = groupSO.FindProperty("count");
            tileTypeProp = groupSO.FindProperty("tileType");
            rotationProp = groupSO.FindProperty("rotation");

            if (slotProp.objectReferenceValue == null && defaultSlot != null)
            {
                slotProp.objectReferenceValue = defaultSlot;
                slotProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            var slotField = CreateField(slotProp, groupSO);
            var countField = CreateField(countProp, groupSO);
            var tileTypeField = CreateField(tileTypeProp, groupSO);
            var rotationField = CreateField(rotationProp, groupSO);

            var deleteButton = new Button(OnDeleteButtonClicked)
            {
                text = "Delete"
            };

            deleteButton.style.width = 80;
            deleteButton.style.marginTop = 8;
            deleteButton.style.marginLeft = StyleKeyword.Auto;

            this.AddAll(slotField, countField, tileTypeField, rotationField, deleteButton);
            this.AddStyleSheet(ShearsStyles.InspectorStyles);
            AddToClassList(ShearsStyles.DarkContainerClass);
        }

        private PropertyField CreateField(SerializedProperty property, SerializedObject so)
        {
            var field = new PropertyField(property);
            field.Bind(so);

            field.RegisterValueChangeCallback(OnSettingsChanged);

            return field;
        }

        private void OnSettingsChanged(SerializedPropertyChangeEvent evt)
        {
            group.GetComponentsInChildren(slots);

            var slotPrefab = slotProp.objectReferenceValue as TrapSlot;

            // If prefab reference is not set, don't continue
            if (slotPrefab == null)
            {
                slots.ForEach((tile) => GameObject.DestroyImmediate(tile.gameObject));

                SHLogger.Log("You need to set Tile in the inspector!", SHLogLevels.Warning);

                return;
            }

            // If current instanced prefab is not the same as our set value, destroy all instances
            if (slots.Count > 0)
            {
                string setPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(slotPrefab);
                string instancedPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(slots[0]);

                if (setPrefab != instancedPrefab)
                {
                    slots.ForEach((tile) => GameObject.DestroyImmediate(tile.gameObject));
                    slots.Clear();
                }
            }

            // Unify count
            int count = countProp.intValue;

            if (count < slots.Count)
            {
                int diff = slots.Count - count;

                for (int i = diff; i > 0; i--)
                {
                    GameObject.DestroyImmediate(slots[^1].gameObject);
                    slots.RemoveAt(slots.Count - 1);
                }
            }
            else if (count > slots.Count)
            {
                int diff = count - slots.Count;

                for (int i = 0; i < diff; i++)
                {
                    var slot = PrefabUtility.InstantiatePrefab(slotPrefab) as TrapSlot;
                    slot.transform.SetParent(group.transform);

                    slots.Add(slot);
                }
            }

            Vector3 start = Vector3.zero;
            Quaternion rotation = Quaternion.Euler(rotationProp.vector3Value);

            for (int i = 0; i < count; i++)
            {
                var slot = slots[i];
                slot.transform.SetLocalPositionAndRotation
                (
                    start + (Tile.TILE_OFFSET * i * Vector3.right),
                    rotation
                );

                var slotSO = new SerializedObject(slot);
                var tileSO = new SerializedObject(slot.Tile);

                var groupProp = slotSO.FindProperty("group");
                var typeProp = tileSO.FindProperty("type");

                groupProp.objectReferenceValue = group;
                typeProp.enumValueFlag = tileTypeProp.enumValueFlag;

                slotSO.ApplyModifiedProperties();
                tileSO.ApplyModifiedProperties();
            }
        }

        private void OnDeleteButtonClicked() => deleteCallback?.Invoke(group);
    }
}
