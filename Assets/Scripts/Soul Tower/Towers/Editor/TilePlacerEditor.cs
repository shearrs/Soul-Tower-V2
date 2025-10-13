using System.Collections.Generic;
using Shears;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    [CustomEditor(typeof(TilePlacer))]
    public class TilePlacerEditor : UnityEditor.Editor
    {
        private TilePlacer placer;
        private SerializedProperty groupsProp;

        private readonly List<Tile> tiles = new();
        private readonly List<SpecialTileGroup> specialTiles = new();

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var scriptProp = serializedObject.FindProperty("m_Script");
            placer = serializedObject.targetObject as TilePlacer;
            groupsProp = serializedObject.FindProperty("groups");

            var scriptField = new PropertyField(scriptProp)
            {
                enabledSelf = false
            };

            var groupsField = new PropertyField(groupsProp);
            groupsField.RegisterValueChangeCallback(OnGroupsChanged);
            OnGroupsChanged(null);

            root.AddAll(scriptField, groupsField);

            return root;
        }

        private void OnGroupsChanged(SerializedPropertyChangeEvent evt)
        {
            tiles.Clear();
            placer.GetComponentsInChildren(tiles);

            specialTiles.Clear();
            placer.GetComponentsInChildren(specialTiles);

            tiles.RemoveAll((tile) => HasID(tile.GroupID) || tile.IsSpecialGroupTile);
            specialTiles.RemoveAll((tile) => HasID(tile.GroupID));

            foreach (var tile in tiles)
            {
                Debug.Log("delete tile: " + tile.name);
                DestroyImmediate(tile.gameObject);
            }

            foreach (var tile in specialTiles)
                DestroyImmediate(tile.gameObject);
        }

        private bool HasID(string id)
        {
            for (int i = 0; i < groupsProp.arraySize; i++)
            {
                var groupProp = groupsProp.GetArrayElementAtIndex(i);
                var idProp = groupProp.FindPropertyRelative("id");

                if (idProp.stringValue == id)
                    return true;
            }

            return false;
        }
    }
}
