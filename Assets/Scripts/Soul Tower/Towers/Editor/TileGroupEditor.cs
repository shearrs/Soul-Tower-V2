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
        private SerializedProperty tileTypeProp;
        private readonly List<TileSubgroup> subgroups = new();
        private readonly List<Tile> tiles = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            group = serializedObject.targetObject as TileGroup;

            var defaultTilesProp = serializedObject.FindProperty("defaultTiles");
            var defaultTileGroupsProp = serializedObject.FindProperty("defaultTileGroups");
            tileTypeProp = serializedObject.FindProperty("tileType");

var defaultsContainer = new Foldout()
            {
                value = false,
                text = "Defaults"
            };
            defaultsContainer.AddStyleSheet(ShearsStyles.InspectorStyles);
            defaultsContainer.AddToClassList(ShearsStyles.DarkFoldoutClass);

            defaultsContainer.style.marginBottom = 4;

            var defaultTilesField = new PropertyField(defaultTilesProp);
            var defaultTileGroupsField = new PropertyField(defaultTileGroupsProp);

            defaultTilesField.TrackSerializedObjectValue(serializedObject, (obj) => LoadSubgroups());

            defaultsContainer.AddAll(defaultTilesField, defaultTileGroupsField);

            var tileTypeField = new PropertyField(tileTypeProp);
            tileTypeField.RegisterValueChangeCallback((evt) => SetTileTypes());

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

            root.AddAll(defaultsContainer, tileTypeField, addButton, subgroupsContainer);

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

            foreach (var subgroup in subgroups)
            {
                VisualElement inspector = null;
                var subgroupSO = new SerializedObject(subgroup);

                if (subgroup is TrapSlotGroup trapSlotGroup)
                    inspector = new TrapSlotGroupInspector(group, group.GetDefaultTile<TrapSlot>(), trapSlotGroup, OnDeleteButtonClicked, LoadSubgroups);
                else
                    inspector = new TileSubgroupInspector(group, group.GetDefaultTile<Tile>(), subgroup, OnDeleteButtonClicked, LoadSubgroups);

                inspector.TrackSerializedObjectValue(subgroupSO, (obj) => SetSubgroupPositions());

                inspector.style.marginTop = 4;

                subgroupsContainer.Add(inspector);
            }

            SetSubgroupPositions();
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
        }

        private void AddSubgroupType(Type type)
        {
            GameObject gameObject;
            var defaultGroup = group.GetDefaultTileGroup(type);

            if (defaultGroup != null)
                gameObject = (PrefabUtility.InstantiatePrefab(defaultGroup) as Component).gameObject;
            else
                gameObject = new GameObject($"{type.Name}", type);

            gameObject.transform.SetParent(group.transform);
            gameObject.transform.localRotation = Quaternion.identity;

            Undo.RegisterCreatedObjectUndo(gameObject, "Create Subgroup");
            
            LoadSubgroups();
        }

        private void SetTileTypes()
        {
            group.GetComponentsInChildren(tiles);

            foreach (var tile in tiles)
            {
                var tileSO = new SerializedObject(tile);
                var typeProp = tileSO.FindProperty("type");

                typeProp.enumValueFlag = tileTypeProp.enumValueFlag;
                tileSO.ApplyModifiedProperties();
            }
        }

        private void OnUndoRedo(in UndoRedoInfo info)
        {
            LoadSubgroups();
        }
    }
}
