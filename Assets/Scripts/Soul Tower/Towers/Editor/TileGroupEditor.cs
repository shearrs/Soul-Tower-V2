using Shears;
using Shears.Editor;
using SoulTower.Traps;
using SoulTower.Traps.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    [CustomEditor(typeof(TileGroup))]
    public class TileGroupEditor : UnityEditor.Editor
    {
        private TileGroup group;
        private GenericMenu addMenu;
        private VisualElement subgroupsContainer;
        private readonly List<TileSubgroup> subgroups = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            group = serializedObject.targetObject as TileGroup;

            var defaultTilesProp = serializedObject.FindProperty("defaultTiles");
            var defaultTilesField = new PropertyField(defaultTilesProp);
            defaultTilesField.TrackSerializedObjectValue(serializedObject, (obj) => LoadSubgroups());

            var addButton = new Button(OnAddButtonClicked)
            {
                text = "Add Subgroup"
            };

            subgroupsContainer = new VisualElement();
            subgroupsContainer.AddStyleSheet(ShearsStyles.InspectorStyles);
            subgroupsContainer.AddToClassList(ShearsStyles.DarkContainerClass);

            LoadSubgroups();

            addMenu = new();
            addMenu.AddItem(new(nameof(TileSubgroup)), false, () => AddSubgroupType(typeof(TileSubgroup)));

            foreach (var type in TypeCache.GetTypesDerivedFrom<TileSubgroup>())
                addMenu.AddItem(new(type.Name), false, () => AddSubgroupType(type));

            root.AddAll(defaultTilesField, addButton, subgroupsContainer);

            Undo.undoRedoEvent += OnUndoRedo;

            return root;
        }

        private void OnDestroy()
        {
            Undo.undoRedoEvent -= OnUndoRedo;
        }

        private void LoadSubgroups()
        {
            subgroupsContainer.Clear();
            group.GetComponentsInChildren(subgroups);

            Debug.Log("load");

            foreach (var subgroup in subgroups)
            {
                VisualElement inspector = null;
                var subgroupSO = new SerializedObject(subgroup);

                if (subgroup is TrapSlotGroup trapSlotGroup)
                    inspector = new TrapSlotGroupInspector(group.GetDefaultTile<TrapSlot>(), trapSlotGroup, OnDeleteButtonClicked);
                else
                    inspector = new TileSubgroupInspector(group.GetDefaultTile<Tile>(), subgroup, OnDeleteButtonClicked);

                inspector.TrackSerializedObjectValue(subgroupSO, (obj) => SetSubgroupPositions());

                inspector.style.marginTop = 4;

                subgroupsContainer.Add(inspector);
            }
        }

        private void SetSubgroupPositions()
        {
            group.GetComponentsInChildren(subgroups);

            int tileCount = 0;

            foreach (var subgroup in subgroups)
            {
                subgroup.transform.localPosition = tileCount * Vector3.right;
                tileCount += subgroup.Count;
            }
        }

        private void OnAddButtonClicked()
        {
            addMenu.ShowAsContext();
        }

        private void OnDeleteButtonClicked(TileSubgroup subgroup)
        {
            Undo.DestroyObjectImmediate(subgroup.gameObject);
            LoadSubgroups();
            SetSubgroupPositions();
        }

        private void AddSubgroupType(Type type)
        {
            var gameObject = new GameObject($"{type.Name}", type);
            gameObject.transform.SetParent(group.transform);
            gameObject.transform.localRotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(gameObject, "Create Subgroup");
            
            LoadSubgroups();
        }

        private void OnUndoRedo(in UndoRedoInfo info)
        {
            LoadSubgroups();
        }
    }
}
