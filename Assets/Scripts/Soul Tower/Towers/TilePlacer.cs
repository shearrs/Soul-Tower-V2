using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class TilePlacer : MonoBehaviour
    {
        [SerializeField] private List<TileGroup> groups = new();

        public bool IsValidID(string id)
        {
            int count = 0;

            foreach (var group in groups)
            {
                if (group.ID == id)
                {
                    count++;

                    if (count == 2)
                        return false;
                }
            }

            return true;
        }
    }
}
