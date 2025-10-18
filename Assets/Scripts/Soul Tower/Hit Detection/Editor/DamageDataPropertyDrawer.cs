using Shears;
using Shears.Logging;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.HitDetection.Editor
{
    [CustomPropertyDrawer(typeof(DamageData))]
    public class DamageDataPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty selectedData;
        private GenericMenu statusMenu;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var typeField = CreateField(property.FindPropertyRelative("type"));
            var damageField = CreateField(property.FindPropertyRelative("damage"));
            var statusesField = CreateField(property.FindPropertyRelative("statuses"));

            if (statusMenu == null)
                CreateStatusMenu();

            var addStatusButton = new Button(() => AddStatus(property))
            {
                text = "Add Status"
            };

            root.AddAll(typeField, damageField, addStatusButton, statusesField);

            return root;
        }

        private void AddStatus(SerializedProperty dataProp)
        {
            selectedData = dataProp;
            statusMenu.ShowAsContext();
        }

        private void CreateStatusMenu()
        {
            statusMenu = new GenericMenu();
            
            foreach (var type in TypeCache.GetTypesDerivedFrom<IStatus>())
            {
                if (type.IsInterface)
                    continue;

                statusMenu.AddItem(new(type.Name), false, () => OnStatusTypeSelected(type));
            }
        }

        private void OnStatusTypeSelected(Type statusType)
        {
            if (selectedData == null)
            {
                SHLogger.Log("Selected property not found!", SHLogLevels.Error);
                return;
            }

            var status = (IStatus)Activator.CreateInstance(statusType);
            status.Name = StringUtil.PascalSpace(statusType.Name);

            var statusesProp = selectedData.FindPropertyRelative("statuses");
            statusesProp.InsertArrayElementAtIndex(statusesProp.arraySize);
            statusesProp.GetArrayElementAtIndex(statusesProp.arraySize - 1).boxedValue = status;

            selectedData.serializedObject.ApplyModifiedProperties();
        }

        private PropertyField CreateField(SerializedProperty property)
        {
            var field = new PropertyField(property);

            return field;
        }
    }
}
