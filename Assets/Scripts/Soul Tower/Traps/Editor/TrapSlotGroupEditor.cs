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

        private readonly List<TrapSlot> trapSlots = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            group = serializedObject.targetObject as TrapSlotGroup;

            slotsProp = serializedObject.FindProperty("slots");
            var slotPrefabProp = serializedObject.FindProperty("slotPrefab");

            var slotsField = new PropertyField(slotsProp);
            slotsField.RegisterValueChangeCallback(OnSlotsChanged);

            var slotPrefabField = new PropertyField(slotPrefabProp);
            slotPrefabField.RegisterValueChangeCallback(OnSlotsChanged);

            var slotPrefabContainer = new Foldout()
            {
                text = "Reference Setup"
            };
            slotPrefabContainer.AddStyleSheet(Shears.Editor.ShearsStyles.InspectorStyles);
            slotPrefabContainer.AddToClassList(Shears.Editor.ShearsStyles.DarkFoldoutClass);
            slotPrefabContainer.Add(slotPrefabField);

            root.AddAll(slotsField, slotPrefabContainer);

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
        }
    }
}
