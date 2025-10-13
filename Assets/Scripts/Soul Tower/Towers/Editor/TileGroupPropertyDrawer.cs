using Shears;
using Shears.Editor;
using Shears.Logging;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    [CustomPropertyDrawer(typeof(TileGroup))]
    public class TileGroupPropertyDrawer : PropertyDrawer
    {
        private const float TILE_OFFSET = 1.0f;

        private readonly List<Tile> tiles = new();
        private readonly List<SpecialTileGroup> specialTiles = new();

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var placer = property.serializedObject.targetObject as TilePlacer;
            var isSpecialGroupProp = property.FindPropertyRelative("isSpecialGroup");
            var specialGroupProp = property.FindPropertyRelative("specialGroup");
            var idProp = property.FindPropertyRelative("id");
            var tileProp = property.FindPropertyRelative("tile");
            var countProp = property.FindPropertyRelative("count");

            if (!placer.IsValidID(idProp.stringValue))
            {
                idProp.stringValue = System.Guid.NewGuid().ToString();
                property.serializedObject.ApplyModifiedProperties();
            }

            if (countProp.intValue == 0)
            {
                countProp.intValue = 1;
                property.serializedObject.ApplyModifiedProperties();
            }

            var isSpecialGroupField = new PropertyField(isSpecialGroupProp);
            var specialGroupField = new PropertyField(specialGroupProp);
            var tileField = new PropertyField(tileProp);
            var countField = new PropertyField(countProp);

            isSpecialGroupField.RegisterValueChangeCallback((evt) => OnGroupChanged(property));
            specialGroupField.RegisterValueChangeCallback((evt) => OnGroupChanged(property));
            tileField.RegisterValueChangeCallback((evt) => OnGroupChanged(property));
            countField.RegisterValueChangeCallback((evt) => OnGroupChanged(property));

            root.AddAll(isSpecialGroupField, specialGroupField, tileField, countField);

            return root;
        }

        private void OnGroupChanged(SerializedProperty property)
        {
            if (Application.isPlaying)
                return;

            string id = property.FindPropertyRelative("id").stringValue;
            var placer = property.serializedObject.targetObject as TilePlacer;
            var group = (TileGroup)property.boxedValue;

            if (property.FindPropertyRelative("isSpecialGroup").boolValue)
            {
                UpdateSpecialGroup(property, placer, group);

                return;
            }

            int count = property.FindPropertyRelative("count").intValue;
            var tilePrefab = property.FindPropertyRelative("tile").objectReferenceValue as Tile;

            tiles.Clear();
            placer.GetComponentsInChildren(tiles);
            tiles.RemoveAll((tile) => tile.GroupID != id);

            if (tilePrefab == null)
            {
                SHLogger.Log("Need to assign Tile in the inspector!", SHLogLevels.Warning);

                foreach (var tile in tiles)
                    GameObject.DestroyImmediate(tile.gameObject);

                return;
            }

            if (tiles.Count > 0)
            {
                var currentPrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tiles[0]);
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tilePrefab);

                if (currentPrefabPath != prefabPath)
                {
                    tiles.ForEach((tile) => GameObject.DestroyImmediate(tile.gameObject));
                    tiles.Clear();
                }
            }

            specialTiles.Clear();
            placer.GetComponentsInChildren(specialTiles);

            foreach (var tile in specialTiles)
            {
                if (tile.GroupID == id)
                    GameObject.DestroyImmediate(tile.gameObject);
            }

            if (count < tiles.Count)
            {
                int diff = tiles.Count - count;

                for (int i = diff; i > 0; i--)
                {
                    GameObject.DestroyImmediate(tiles[^1].gameObject);
                    tiles.RemoveAt(tiles.Count - 1);
                }
            }
            else if (count > tiles.Count)
            {
                int diff = count - tiles.Count;

                for (int i = 0; i < diff; i++)
                {
                    var tile = PrefabUtility.InstantiatePrefab(tilePrefab) as Tile;
                    tile.transform.SetParent(placer.transform);
                    tile.transform.localRotation = Quaternion.identity;

                    tiles.Add(tile);
                }
            }

            var groups = property.FindParentProperty();
            int startOffset = GetStartOffset(groups, group);
            Vector3 start = TILE_OFFSET * startOffset * Vector3.right;

            for (int i = 0; i < count; i++)
            {
                var slot = tiles[i];
                slot.transform.localPosition = start + (TILE_OFFSET * i * Vector3.right);
            }

            foreach (var tile in tiles)
            {
                var tileSO = new SerializedObject(tile);
                var groupProp = tileSO.FindProperty("groupID");
                groupProp.stringValue = id;

                tileSO.ApplyModifiedProperties();
            }
        }

        private void UpdateSpecialGroup(SerializedProperty property, TilePlacer placer, TileGroup group)
        {
            placer.GetComponentsInChildren(tiles);

            foreach (var tile in tiles)
            {
                if (tile.GroupID == group.ID && !tile.IsSpecialGroupTile)
                    GameObject.DestroyImmediate(tile.gameObject);
            }

            var specialPrefab = property.FindPropertyRelative("specialGroup").objectReferenceValue as SpecialTileGroup;
            specialTiles.Clear();
            placer.GetComponentsInChildren(specialTiles);
            var specialTile = specialTiles.Find((tile) => tile.GroupID == group.ID);

            if (specialPrefab == null)
            {
                SHLogger.Log("Need to assign Special Group in the inspector!", SHLogLevels.Warning);

                if (specialTile != null)
                    GameObject.DestroyImmediate(specialTile.gameObject);

                return;
            }

            if (specialTile != null)
            {
                var currentPrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(specialTile);
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(specialPrefab);

                if (currentPrefabPath != prefabPath)
                    GameObject.DestroyImmediate(specialTile.gameObject);
            }

            if (specialTile == null)
            {
                var tile = PrefabUtility.InstantiatePrefab(specialPrefab) as SpecialTileGroup;
                tile.transform.SetParent(placer.transform);
                tile.transform.localRotation = Quaternion.identity;

                var groups = property.FindParentProperty();
                int startOffset = GetStartOffset(groups, group);
                tile.transform.localPosition = Vector3.right * startOffset;
                tile.GroupID = group.ID;
            }
        }

        private int GetStartOffset(SerializedProperty groups, TileGroup group)
        {
            int startOffset = 0;
            var gameObject = ((Component)groups.serializedObject.targetObject).gameObject;

            for (int i = 0; i < groups.arraySize; i++)
            {
                var currentGroup = (TileGroup)groups.GetArrayElementAtIndex(i).boxedValue;

                if (currentGroup == group)
                    break;

                startOffset += currentGroup.GetCount(gameObject);
            }

            return startOffset;
        }
    }
}
