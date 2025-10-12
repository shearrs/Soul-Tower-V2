using Shears;
using Shears.Logging;
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

            slotsProp = serializedObject.FindProperty("slots");
            placementTypeProp = serializedObject.FindProperty("placementType");
            var slotPrefabProp = serializedObject.FindProperty("slotPrefab");
            var subgroupsProp = serializedObject.FindProperty("subgroups");

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

            root.AddAll(slotsField, placementTypeField, slotPrefabContainer);

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
                    var slot = Instantiate(slotPrefab);
                    slot.transform.SetParent(group.transform);

                    trapSlots.Add(slot);
                }
            }

            Vector3 start = (TILE_OFFSET * Mathf.Floor(0.5f * slots)) * Vector3.left;

            for (int i = 0; i < slots; i++)
            {
                var slot = trapSlots[i];
                slot.transform.localPosition = start + (TILE_OFFSET * i * Vector3.right);
            }

            group.SetPlacementType((TrapPlacementType)placementTypeProp.enumValueFlag);

            foreach(var slot in trapSlots)
            {
                SerializedObject slotProp = new SerializedObject(slot);
                slotProp.ApplyModifiedProperties();
                slotProp.Update();
            }
        }
    }
}
