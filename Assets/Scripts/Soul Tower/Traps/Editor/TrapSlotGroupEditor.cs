using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Traps.Editor
{
    [CustomEditor(typeof(TrapSlotGroup))]
    public class TrapSlotGroupEditor : UnityEditor.Editor
    {
        private const float TILE_OFFSET = 1.0f;

        private TrapSlotGroup group;
        private SerializedProperty slotsProp;
        private SerializedProperty placementTypeProp;

        private readonly List<TrapSlot> trapSlots = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            group = serializedObject.targetObject as TrapSlotGroup;

            var scriptProp = serializedObject.FindProperty("m_Script");
            slotsProp = serializedObject.FindProperty("slots");
            placementTypeProp = serializedObject.FindProperty("placementType");
            var slotPrefabProp = serializedObject.FindProperty("slotPrefab");
            var subgroupsProp = serializedObject.FindProperty("subgroups");

            var scriptField = new PropertyField(scriptProp)
            {
                enabledSelf = false
            };

            var slotsField = new PropertyField(slotsProp);
            slotsField.RegisterValueChangeCallback(OnSlotsChanged);

            var placementTypeField = new PropertyField(placementTypeProp);
            placementTypeField.RegisterValueChangeCallback(OnSlotsChanged);

            var slotPrefabField = new PropertyField(slotPrefabProp);
            slotPrefabField.RegisterValueChangeCallback(OnSlotsChanged);

            var subgroupsField = new PropertyField(subgroupsProp);

            var slotPrefabContainer = new Foldout()
            {
                text = "Advanced"
            };
            slotPrefabContainer.AddStyleSheet(Shears.Editor.ShearsStyles.InspectorStyles);
            slotPrefabContainer.AddToClassList(Shears.Editor.ShearsStyles.DarkFoldoutClass);

            slotPrefabContainer.AddAll(slotPrefabField, subgroupsField);
            slotPrefabContainer.style.marginTop = 8;

            root.AddAll(scriptField, slotsField, placementTypeField, slotPrefabContainer);

            return root;
        }

        private void OnSlotsChanged(SerializedPropertyChangeEvent evt)
        {
            if (Application.isPlaying)
                return;

            int slots = slotsProp.intValue;
            var slotPrefab = serializedObject.FindProperty("slotPrefab").objectReferenceValue as TrapSlot;

            if (slotPrefab == null)
            {
                SHLogger.Log("Need to assign Slot Prefab in the inspector!", SHLogLevels.Warning);
                return;
            }

            group.GetComponentsInChildren(trapSlots);

            if (slots < trapSlots.Count)
            {
                int count = trapSlots.Count - slots;

                for (int i = count; i > 0; i--)
                {
                    DestroyImmediate(trapSlots[^1].gameObject);
                    trapSlots.RemoveAt(trapSlots.Count - 1);
                }
            }
            else if (slots > trapSlots.Count)
            {
                int difference = slots - trapSlots.Count;

                for (int i = 0; i < difference; i++)
                {
                    var slot = PrefabUtility.InstantiatePrefab(slotPrefab) as TrapSlot;
                    slot.transform.SetParent(group.transform);
                    slot.transform.localRotation = Quaternion.identity;

                    trapSlots.Add(slot);
                }
            }

            Vector3 start = Vector3.zero;

            for (int i = 0; i < slots; i++)
            {
                var slot = trapSlots[i];
                slot.transform.localPosition = start + (TILE_OFFSET * i * Vector3.right);
            }

            foreach(var slot in trapSlots)
            {
                var slotSO = new SerializedObject(slot);
                var tileSO = new SerializedObject(slot.GetComponent<Tile>());
                var typeProp = slotSO.FindProperty("placementType");
                var isSpecialGroupTileProp = tileSO.FindProperty("isSpecialGroupTile");

                typeProp.enumValueFlag = placementTypeProp.enumValueFlag;
                isSpecialGroupTileProp.boolValue = true;

                slotSO.ApplyModifiedProperties();
                tileSO.ApplyModifiedProperties();
            }
        }
    }
}
