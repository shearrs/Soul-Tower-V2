using Shears;
using Shears.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    public class TileSubgroupInspector : VisualElement
    {
        private readonly TileSubgroup subgroup;
        private readonly Action<TileSubgroup> deleteCallback;

        public TileSubgroupInspector(TileSubgroup subgroup, Action<TileSubgroup> deleteCallback)
        {
            this.subgroup = subgroup;
            this.deleteCallback = deleteCallback;

            CreateInspector();
        }

        private void CreateInspector()
        {
            var subgroupSO = new SerializedObject(subgroup);

            var tileProp = subgroupSO.FindProperty("tile");
            var countProp = subgroupSO.FindProperty("count");

            var tileField = new PropertyField(tileProp);
            var countField = new PropertyField(countProp);
            tileField.Bind(subgroupSO);
            countField.Bind(subgroupSO);

            var deleteButton = new Button(OnDeleteButtonClicked)
            {
                text = "Delete"
            };

            deleteButton.style.width = 80;
            deleteButton.style.marginTop = 8;
            deleteButton.style.marginLeft = StyleKeyword.Auto;

            this.AddAll(tileField, countField, deleteButton);

            this.AddStyleSheet(ShearsStyles.InspectorStyles);
            AddToClassList(ShearsStyles.DarkContainerClass);
        }

        private void OnDeleteButtonClicked() => deleteCallback?.Invoke(subgroup);
    }
}
