using UnityEngine;
using UnityEngine.Tilemaps;
using ColliderType = UnityEngine.Tilemaps.Tile.ColliderType;

[CreateAssetMenu(fileName = "New Tile", menuName = "WFC/Tile", order = 51)]
public sealed class WFCTile : TileBase
{
    [Space]
    public double weight = 1d;
    
    [Space]
    public int forward;
    public int right;
    public int back;
    public int left;
    
    [Space]
    public Sprite sprite;
    
    public bool CanAppendTile(WFCTile other, EDirection direction)
        => direction switch
        {
            EDirection.Forward => forward == other.back, 
            EDirection.Right => right == other.left, 
            EDirection.Back => back == other.forward,
            EDirection.Left => left == other.right, 
            _ => false
        };
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.color = Color.white;
        tileData.gameObject = null;
        tileData.transform = Matrix4x4.identity;
        tileData.flags = TileFlags.LockColor;
        tileData.colliderType = ColliderType.Sprite;
    }
};
