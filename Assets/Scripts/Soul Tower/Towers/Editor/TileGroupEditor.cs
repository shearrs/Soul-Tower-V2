using Shears;
using Shears.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    [CustomEditor(typeof(NewTileGroup))]
    public class TileGroupEditor : UnityEditor.Editor
    {
        private NewTileGroup group;
        private GenericMenu addMenu;
        private VisualElement subgroupsContainer;
        private readonly List<TileSubgroup> subgroups = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            group = serializedObject.targetObject as NewTileGroup;

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

            root.AddAll(addButton, subgroupsContainer);

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
                var inspector = new TileSubgroupInspector(subgroup, OnDeleteButtonClicked);

                subgroupsContainer.Add(inspector);
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
            var gameObject = new GameObject($"{type.Name}", type);
            gameObject.transform.SetParent(group.transform);

            Undo.RegisterCreatedObjectUndo(gameObject, "Create Subgroup");
            
            LoadSubgroups();
        }

        private void OnUndoRedo(in UndoRedoInfo info)
        {
            LoadSubgroups();
        }
    }
}
