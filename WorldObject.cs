using UnityEngine;
using NaughtyAttributes;

namespace ProceduralGeneration
{
    [CreateAssetMenu(fileName = "NewWorldObject", menuName = "Procedural Generation/New World Object", order = 2)]
    internal sealed class WorldObject : ScriptableObject
    {
        [field: SerializeField, ShowAssetPreview] internal GameObject[] Prefabs { get; private set; }
        [field: SerializeField, Range(0, 100)] internal int Density { get; private set; } = 50;
        [field: SerializeField, Range(0f, 1f)] internal float Height { get; private set; } = 0.5f;
        [field: SerializeField] internal Vector2 Offset { get; private set; } = new(0.5f, 0.125f);
        [field: SerializeField] internal bool Single { get; private set; } = false;
        [field: SerializeField] internal NoiseConfig NoiseConfig { get; private set; }
    }
}
