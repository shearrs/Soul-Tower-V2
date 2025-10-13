using Shears;
using Shears.Editor;
using Shears.Logging;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SoulTower.Towers.Editor
{
    public class TileSubgroupInspector : VisualElement
    {
        private readonly Tile defaultTile;
        private readonly TileGroup group;
        private readonly TileSubgroup subgroup;
        private readonly Action<TileSubgroup> deleteCallback;
        private readonly Action orderChangedCallback;
        private readonly List<Tile> tiles = new();

        private SerializedProperty tileProp;
        private SerializedProperty countProp;
        private SerializedProperty rotationProp;

        public TileSubgroupInspector(TileGroup group, Tile defaultTile, TileSubgroup subgroup, 
            Action<TileSubgroup> deleteCallback, Action orderChangedCallback)
        {
            this.group = group;
            this.subgroup = subgroup;
            this.deleteCallback = deleteCallback;
            this.defaultTile = defaultTile;
            this.orderChangedCallback = orderChangedCallback;

            CreateInspector();
        }

        private void CreateInspector()
        {
            var groupSO = new SerializedObject(subgroup);

            tileProp = groupSO.FindProperty("tile");
            countProp = groupSO.FindProperty("count");
            rotationProp = groupSO.FindProperty("rotation");

            if (tileProp.objectReferenceValue == null && defaultTile != null)
            {
                tileProp.objectReferenceValue = defaultTile;
                tileProp.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            var tileField = CreateField(tileProp, groupSO);
            var countField = CreateField(countProp, groupSO);
            var rotationField = CreateField(rotationProp, groupSO);

            var controlContainer = new VisualElement();
            controlContainer.style.flexDirection = FlexDirection.Row;

            var upButton = new Button(OnUpButtonClicked)
            {
                text = "↑"
            };

            var downButton = new Button(OnDownButtonClicked)
            {
                text = "↓"
            };

            var deleteButton = new Button(OnDeleteButtonClicked)
            {
                text = "Delete"
            };

            deleteButton.style.width = 80;
            deleteButton.style.marginTop = 8;
            deleteButton.style.marginLeft = StyleKeyword.Auto;

            controlContainer.AddAll(upButton, downButton, deleteButton);

            this.AddAll(tileField, countField, rotationField, controlContainer);
            this.AddStyleSheet(ShearsStyles.InspectorStyles);
            AddToClassList(ShearsStyles.DarkContainerClass);
        }

        private PropertyField CreateField(SerializedProperty property, SerializedObject so)
        {
            var field = new PropertyField(property);
            field.Bind(so);

            field.RegisterValueChangeCallback(OnSettingsChanged);

            return field;
        }

        private void OnSettingsChanged(SerializedPropertyChangeEvent evt)
        {
            subgroup.GetComponentsInChildren(tiles);

            var tilePrefab = tileProp.objectReferenceValue as Tile;

            // If prefab reference is not set, don't continue
            if (tilePrefab == null)
            {
                tiles.ForEach((tile) => GameObject.DestroyImmediate(tile.gameObject));

                SHLogger.Log("You need to set Tile in the inspector!", SHLogLevels.Warning);

                return;
            }

            // If current instanced prefab is not the same as our set value, destroy all instances
            if (tiles.Count > 0)
            {
                string setPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tilePrefab);
                string instancedPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(tiles[0]);

                if (setPrefab != instancedPrefab)
                {
                    tiles.ForEach((tile) => GameObject.DestroyImmediate(tile.gameObject));
                    tiles.Clear();
                }
            }

            // Unify count
            int count = countProp.intValue;

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
                    tile.transform.SetParent(subgroup.transform);

                    tiles.Add(tile);
                }
            }

            Vector3 start = Vector3.zero;
            Quaternion rotation = Quaternion.Euler(rotationProp.vector3Value);

            for (int i = 0; i < count; i++)
            {
                var tile = tiles[i];
                tile.transform.SetLocalPositionAndRotation
                (
                    start + (Tile.TILE_OFFSET * i * Vector3.right), 
                    rotation
                );

                var tileSO = new SerializedObject(tile);
                var typeProp = tileSO.FindProperty("type");

                typeProp.enumValueFlag = (int)group.TileType;

                tileSO.ApplyModifiedProperties();
            }
        }

        private void OnUpButtonClicked()
        {
            int siblingIndex = subgroup.transform.GetSiblingIndex();

            if (siblingIndex == 0)
                return;

            subgroup.transform.SetSiblingIndex(siblingIndex - 1);
            orderChangedCallback?.Invoke();
        }

        private void OnDownButtonClicked()
        {
            int siblingIndex = subgroup.transform.GetSiblingIndex();

            if (siblingIndex == subgroup.transform.parent.childCount - 1)
                return;

            subgroup.transform.SetSiblingIndex(siblingIndex + 1);
            orderChangedCallback?.Invoke();
        }

        private void OnDeleteButtonClicked() => deleteCallback?.Invoke(subgroup);
    }
}
