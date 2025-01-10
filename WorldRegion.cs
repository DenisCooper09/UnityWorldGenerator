using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralGeneration
{
    [CreateAssetMenu(fileName = "NewWorldRegion", menuName = "Procedural Generation/New World Region", order = 1)]
    internal sealed class WorldRegion : ScriptableObject
    {
        [field: SerializeField] internal TileBase Tile { get; private set; }
        [field: SerializeField, Range(0f, 1f)] internal float Height { get; private set; }
        [field: SerializeField] internal WorldObject[] Objects { get; private set; }
        [field: SerializeField] internal bool Root { get; private set; }
    }
}
