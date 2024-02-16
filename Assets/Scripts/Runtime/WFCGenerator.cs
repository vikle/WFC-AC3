using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class WFCGenerator : MonoBehaviour
{
    [Space]
    public Vector2Int mapSize = new Vector2Int(33, 33);
    public bool periodic;
    public int maxAttempts = -1;

    [Space]
    public WFCTile[] tiles;

    [Space]
    public Tilemap tilemap;

    static ModelAC3Parameters s_tmpParameters;
    static int s_maxAttempts;
    static WFCTile[,] s_solvedGrid;


    public async ValueTask Generate()
    {
        if (tilemap == null) return;
        
        tilemap.ClearAllTiles();

        s_tmpParameters = new ModelAC3Parameters(mapSize.x, mapSize.y, periodic, tiles);
        s_maxAttempts = maxAttempts;

        await Task.Run(() =>
        {
            var tmp_model = new ModelAC3(in s_tmpParameters);
            int seed = Guid.NewGuid().GetHashCode();
            tmp_model.Run(seed, s_maxAttempts);
            s_solvedGrid = tmp_model.GetSolvedGrid();
        });

        int width = mapSize.x;
        int height = mapSize.y;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y), s_solvedGrid[x, y]);
            }
        }

        s_solvedGrid = null;
    }
};
