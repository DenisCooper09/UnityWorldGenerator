using UnityEngine;

namespace ProceduralGeneration
{
    [CreateAssetMenu(fileName = "NewNoiseConfig", menuName = "Procedural Generation/New Noise UnitConfig", order = 0)]
    internal sealed class NoiseConfig : ScriptableObject
    {
        [field: SerializeField, Min(5)] internal int Width { get; private set; } = 50;
        [field: SerializeField, Min(5)] internal int Height { get; private set; } = 50;
        [field: SerializeField] internal bool GenerateRandomSeed { get; private set; } = true;
        [field: SerializeField] internal int Seed { get; set; }
        [field: SerializeField] internal float NoiseScale { get; private set; } = 25;
        [field: SerializeField, Min(0)] internal int Octaves { get; private set; } = 5;
        [field: SerializeField, Range(0f, 1f)] internal float Persistence { get; private set; } = 0.5f;
        [field: SerializeField, Min(1f)] internal float Lacunarity { get; private set; } = 2f;
		[field: SerializeField] internal float FalloffExponent { get; private set; } = 3f;
        [field: SerializeField] internal Vector2 Offset { get; private set; }

        [SerializeField] private bool m_AutoUpdate = false;

        internal Texture2D PreviewTexture { get; private set; }

        internal void GeneratePreviewTexture()
        {
            var noiseMap = NoiseGenerator.GenerateNoiseMap(this);

            Color32[] colorMap = new Color32[Width * Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++) 
                {
                    colorMap[y * Width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }

            PreviewTexture = new Texture2D(Width, Height);
            PreviewTexture.SetPixels32(colorMap);
            PreviewTexture.Apply();
        }

        private void OnValidate()
        {
            if (!m_AutoUpdate) 
                return;

            GeneratePreviewTexture();
        }
    }
}
