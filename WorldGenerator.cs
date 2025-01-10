using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralGeneration
{
    public sealed class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private NoiseConfig m_NoiseConfig;

        [System.Serializable]
        private struct WorldRegionExtended
        {
            [field: SerializeField] internal WorldRegion Region { get; private set; }
            [field: SerializeField] internal Tilemap Tilemap { get; private set; }
        }

        [SerializeField] private WorldRegionExtended[] m_Regions;

        [SerializeField] private GameObject m_Player;

        [SerializeField] private PolygonCollider2D m_BorderCollider;

        private Dictionary<Vector3Int, bool> _freeTiles = new();

        private void Awake()
        {
            GenerateWorld();
        }

        private void filterFreeTiles()
        {
            // Use a single loop and remove occupied tiles directly
            var occupiedTiles = new List<Vector3Int>();
            foreach (var kvp in _freeTiles)
            {
                if (!kvp.Value)
                {
                    occupiedTiles.Add(kvp.Key);
                }
            }

            foreach (var tile in occupiedTiles)
            {
                _freeTiles.Remove(tile);
            }

            EnemySpawner.instance.PopulatePoints(_freeTiles.Keys.ToArray<Vector3Int>());
        }

        public void GenerateWorld()
        {
            if (m_NoiseConfig.GenerateRandomSeed)
            {
                System.Random prngStart = new();
                m_NoiseConfig.Seed = prngStart.Next(int.MinValue, int.MaxValue);
            }

            var noiseMap = NoiseGenerator.GenerateNoiseMap(m_NoiseConfig);

            // Pre-calculate width and height outside the loop
            int width = m_NoiseConfig.Width;
            int height = m_NoiseConfig.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var position = new Vector3Int(x, y, 0);

                    foreach (var region in m_Regions)
                    {
                        // Consider removing commented-out code for clarity

                        if (noiseMap[x, y] >= region.Region.Height)
                        {
                            region.Tilemap.SetTile(position, region.Region.Tile);
                        }
                    }

                    _freeTiles.Add(position, true);
                }
            }

            //if (m_Player != null)
            //{
                foreach (var region in m_Regions)
                {
                    if (region.Region.Root)
                    {
                        System.Random prng = new(m_NoiseConfig.Seed);
                        Vector3Int randomPosition;

                        do
                        {
                            randomPosition = new(prng.Next(0, width), prng.Next(0, height), 0);
                        }
                        while (region.Tilemap.GetTile(randomPosition) != region.Region.Tile);

                        m_Player.transform.position = (Vector3)randomPosition;

                        // Directly set the value to false instead of removing and adding
                        _freeTiles[randomPosition] = false;
                    }
                }
            //}
			
            foreach (var region in m_Regions)
            {
                foreach (var regionObject in region.Region.Objects)
                {
                    if (regionObject.NoiseConfig.GenerateRandomSeed)
                    {
                        System.Random prngMain = new(m_NoiseConfig.Seed);
                        regionObject.NoiseConfig.Seed = prngMain.Next(int.MinValue, int.MaxValue);
                    }

                    var regionObjectNoiseMap = NoiseGenerator.GenerateNoiseMap(regionObject.NoiseConfig);
                    System.Random prng = new(regionObject.NoiseConfig.Seed);
					
					if (regionObject.Single)
                    {
                        Vector3Int randomPosition;

                        do
                        {
                            randomPosition = new(prng.Next(0, width), prng.Next(0, height), 0);
                        }
                        while (region.Tilemap.GetTile(randomPosition) != region.Region.Tile && _freeTiles.ContainsKey(randomPosition) && _freeTiles[randomPosition]);

                        Instantiate(
							regionObject.Prefabs[prng.Next(0, regionObject.Prefabs.Length)],
							(Vector2)(Vector2Int)randomPosition + regionObject.Offset,
							Quaternion.identity,
							transform);
						
                        _freeTiles[randomPosition] = false;
						
                        continue;
                    }

                    // Pre-calculate region object width and height outside the loop
                    int regionObjectWidth = regionObject.NoiseConfig.Width;
                    int regionObjectHeight = regionObject.NoiseConfig.Height;

                    for (int x = 0; x < regionObjectWidth; x++)
                    {
                        // Check for out of bounds conditions earlier
                        if (x >= width)
                            break;

                        for (int y = 0; y < regionObjectHeight; y++)
                        {
                            if (y >= height)
                                break;

                            if (prng.Next(0, 100) <= regionObject.Density)
                            {
                                if (region.Tilemap.GetTile(new Vector3Int(x, y, 0)) == region.Region.Tile)
                                {
                                    if (regionObjectNoiseMap[x, y] >= regionObject.Height)
                                    {
                                        var position = new Vector3Int(x, y, 0);

                                        // Directly check the value instead of using TryGetValue
                                        if (_freeTiles.ContainsKey(position) && _freeTiles[position])
                                        {
                                            Instantiate(
                                                regionObject.Prefabs[prng.Next(0, regionObject.Prefabs.Length)],
                                                new Vector2(x, y) + regionObject.Offset,
                                                Quaternion.identity,
                                                transform);

                                            // Directly set the value to false
                                            _freeTiles[position] = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            m_BorderCollider.SetPath(0, new Vector2[4] { new Vector2(0, height), new Vector2(0, 0), new Vector2(width, 0), new Vector2(width, height) });
            filterFreeTiles();
        }
    }
}
