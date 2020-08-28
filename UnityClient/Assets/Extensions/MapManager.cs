using OpenTibiaUnity;
using OpenTibiaUnity.Core;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Transform mapParent;

    private float LEVEL_OFFSET = -1;
    private float XZ_OFFSET = 2.42f;

    private MapTile[][][] tileHolders = new MapTile[Constants.MapSizeX - 1][][];

    private Vector3Int mapSize = new Vector3Int(Constants.MapSizeX - 1, Constants.MapSizeY - 1, Constants.MapSizeZ - 1);

    public static string TILE_PREFABS_FOLDER = "TilePrefabs";

    private void Awake()
    {
        InitTileHolders(mapSize);
    }

    private void InitTileHolders(Vector3Int mapSize)
    {
        tileHolders = new MapTile[mapSize.x][][];

        for (int x = 0; x < mapSize.x; x++)
        {
            tileHolders[x] = new MapTile[mapSize.y][];

            for (int y = 0; y < mapSize.y; y++)
            {
                tileHolders[x][y] = new MapTile[mapSize.z];

                for (int level = 0; level < mapSize.z; level++)
                {
                    var posX = x - (level * LEVEL_OFFSET);
                    var posY = 0 + (level * XZ_OFFSET);
                    var posZ = y - (level * LEVEL_OFFSET);

                    GameObject tile = new GameObject();
                    var mt = tile.AddComponent<MapTile>();
                    tile.name = string.Format("X{0}Y{1}Z{2}", x, y, level);
                    var t = tile.transform;
                    t.SetParent(mapParent);
                    t.localPosition = new Vector3(posX, posY, posZ);
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;

                    tileHolders[x][y][level] = mt;
                }
            }
        }
    }

    public void DrawMap()
    {
        var xLength = tileHolders.Length;
        for (int x = 0; x < xLength; x++)
        {
            var yLength = tileHolders[x].Length;
            for (int y = 0; y < yLength; y++)
            {
                var levels = tileHolders[x][y].Length;
                for (int level = 0; level < levels; level++)
                {
                    OpenTibiaUnity.Core.WorldMap.Field _field = GameManager.Instance.WorldMapStorage.GetField(new Vector3Int(x, y, level));

                    for (int i = 0; i < _field.ObjectsNetwork.Length; i++)
                    {
                        if (_field.ObjectsNetwork[i] != null)
                        {
                            var item = _field.ObjectsNetwork[i];
                            tileHolders[x][y][level].Items[i].Id = item.Id;
                        }
                        else
                            tileHolders[x][y][level].Items[i].Id = 0;

                        tileHolders[x][y][level].RefreshTile();
                    }
                }
            }
        }
    }
}