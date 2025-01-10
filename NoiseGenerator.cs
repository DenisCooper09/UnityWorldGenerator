using System;
using UnityEngine;

namespace ProceduralGeneration
{
    internal static class NoiseGenerator
    {
        internal static float[,] GenerateNoiseMap(NoiseConfig config)
        {
            float[,] noiseMap = new float[config.Width, config.Height];

            System.Random prng = new(config.Seed);
            Vector2[] octaveOffsets = new Vector2[config.Octaves];

            for (int i = 0; i < config.Octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + config.Offset.x;
                float offsetY = prng.Next(-100000, 100000) + config.Offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (config.NoiseScale <= 0f)
                throw new ArgumentOutOfRangeException(nameof(config.NoiseScale));

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = config.Width / 2;
            float halfHeight = config.Height / 2;
			
			float maxDistance = Math.Min(halfWidth, halfHeight);

            for (int x = 0; x < config.Width; x++)
            {
                for (int y = 0; y < config.Height; y++)
                {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int i = 0; i < config.Octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / config.NoiseScale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / config.NoiseScale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= config.Persistence;
                        frequency *= config.Lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseHeight);
                }
            }
			
			for (int x = 0; x < config.Width; x++)
			{
				for (int y = 0; y < config.Height; y++)
				{
					float distanceX = x - halfWidth;
					float distanceY = y - halfHeight;
					float distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
					
					float normalizedDistance = distance / maxDistance;
		
					float falloff = 1 - (float)Math.Pow(normalizedDistance, config.FalloffExponent);
					if (falloff < 0) falloff = 0;
		
					noiseMap[x, y] *= falloff;
				}
			}

            return noiseMap;
        } 
    }
}
