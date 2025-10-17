using Shears;
using Shears.Editor;
using SoulTower.HitDetection;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Traps.Editor
{
    [CustomEditor(typeof(TrapData))]
    public class TrapDataEditor : UnityEditor.Editor
    {
        private SerializedProperty damageDataProp;
        private VisualElement damageDataContainer;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var isPassiveProp = FindProperty("isPassive");
            var cooldownProp = FindProperty("cooldown");
            var sizeProp = FindProperty("size");
            var placementTypeProp = FindProperty("placementType");
            damageDataProp = FindProperty("damageData");

            var isPassiveField = CreateField(isPassiveProp);
            var cooldownField = CreateField(cooldownProp);
            var sizeField = CreateField(sizeProp);
            var placementTypeField = CreateField(placementTypeProp);

            var dataHeader = VisualElementUtil.CreateHeader("Damage Data");
            damageDataContainer = new VisualElement();
            damageDataContainer.AddStyleSheet(ShearsStyles.InspectorStyles);
            damageDataContainer.AddToClassList(ShearsStyles.DarkContainerClass);

            LoadDamageDataList();

            root.AddAll(isPassiveField, cooldownField, sizeField, placementTypeField, dataHeader, damageDataContainer);

            return root;
        }

        private void LoadDamageDataList()
        {
            damageDataContainer.Clear();

            var addButton = new Button(AddDamageData)
            {
                text = "Add"
            };
            addButton.style.marginBottom = 4;
            addButton.style.marginRight = StyleKeyword.Auto;

            damageDataContainer.Add(addButton);

            for (int i = 0; i < damageDataProp.arraySize; i++)
            {
                var data = damageDataProp.GetArrayElementAtIndex(i);

                AddDamageDataField(data);
            }
        }

        private void AddDamageData()
        {
            var data = new DamageData(1, default);

            damageDataProp.InsertArrayElementAtIndex(damageDataProp.arraySize);
            damageDataProp.GetArrayElementAtIndex(damageDataProp.arraySize - 1).boxedValue = data;
            serializedObject.ApplyModifiedProperties();

            LoadDamageDataList();
        }

        private void RemoveDamageData(SerializedProperty data)
        {
            for (int i = 0; i < damageDataProp.arraySize; i++)
            {
                var prop = damageDataProp.GetArrayElementAtIndex(i);

                if (SerializedProperty.EqualContents(prop, data))
                {
                    damageDataProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();

                    LoadDamageDataList();
                    return;
                }
            }
        }

        private void AddDamageDataField(SerializedProperty data)
        {
            var container = new VisualElement();
            container.AddToClassList(ShearsStyles.LightContainerClass);
            container.style.marginTop = 2;
            container.style.paddingLeft = 16;

            var typeField = CreateField(data.FindPropertyRelative("type"));
            var damageField = CreateField(data.FindPropertyRelative("damage"));
            var deleteButton = new Button(() => RemoveDamageData(data))
            {
                text = "Delete"
            };
            deleteButton.style.marginLeft = StyleKeyword.Auto;

            container.AddAll(typeField, damageField, deleteButton);

            damageDataContainer.Add(container);
        }

        private SerializedProperty FindProperty(string name) => serializedObject.FindProperty(name);
        private PropertyField CreateField(SerializedProperty property)
        {
            var field = new PropertyField(property);
            field.Bind(serializedObject);

            return field;
        }
    }
}
