using OpenTibiaUnity;
using OpenTibiaUnity.Core;
using OpenTibiaUnity.Core.WorldMap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{
    public Transform parentObj;
    public List<GameObject> objList = new List<GameObject>();
    int centerX = 8;
    int centerY = 6;
    int maxRangeX = 7;
    int maxRangeY = 5;
    float timer = 0;
    bool timeReached = false;
    public int ObjList;
    public static NewGameManager inst;

    public static Text txt;
    private static List<ObjItem> ObjItemList = new List<ObjItem>();

    protected OpenTibiaUnity.Core.Creatures.Player Player { get => OpenTibiaUnity.OpenTibiaUnity.Player; }

    private int _maxZPlane = 0;
    private int _playerZPlane = 0;
    private int[] _minZPlane;
    float heightStage = 2.42f;
    int offsetPositionUpStage = -1;

    private void Awake()
    {
        inst = this;

        int mapCapacity = Constants.MapSizeX * Constants.MapSizeY;
        _minZPlane = new int[mapCapacity];
    }

    public static void AddObjToList(ObjItem item)
    {
        print(item);
        ObjItemList.Add(item);
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.X))
        {
            int[] array = GettRowGroundId(rowType.right, true);

            for (int i = 0; i < array.Length; i++)
            {
                print(array[i]);
            }
            DrawRowGround(array, rowType.right);

        }
                */
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //x kolumny
            //y wiersz
            //int[,] array = GetGroundAroundMe();
            //for (int i = 0; i < 12 - 1; i++)
            //{
            //    for (int j = 0; j < 16 - 1; j++)
            //    {
            //        //print(array[j, i]);
            //    }
            //}
            RenderGroundAroundMe();

        }


    }

    public void RenderGroundAroundMe2()
    {
        //_playerZPlane = GameManager.Instance.WorldMapStorage.ToMap(Player.Position).z;

        //_maxZPlane = Constants.MapSizeZ - 1;
        //while (_maxZPlane > _playerZPlane && GameManager.Instance.WorldMapStorage.GetObjectPerLayer(_maxZPlane) <= 0)
        //    _maxZPlane--;

        //for (int x = Constants.PlayerOffsetX - 1; x <= Constants.PlayerOffsetX + 1; x++)
        //{
        //    for (int y = Constants.PlayerOffsetY - 1; y <= Constants.PlayerOffsetY + 1; y++)
        //    {
        //        if (!(x != Constants.PlayerOffsetX && y != Constants.PlayerOffsetY || !GameManager.Instance.WorldMapStorage.IsLookPossible(x, y, _playerZPlane)))
        //        {
        //            int z = _playerZPlane + 1;
        //            while (z - 1 < _maxZPlane && x + _playerZPlane - z >= 0 && y + _playerZPlane - z >= 0)
        //            {
        //                var @object = GameManager.Instance.WorldMapStorage.GetObject(x + _playerZPlane - z, y + _playerZPlane - z, z, 0);
        //                if (!!@object && !!@object.Type && @object.Type.IsGround && !@object.Type.IsDontHide)
        //                {
        //                    _maxZPlane = z - 1;
        //                    continue;
        //                }

        //                @object = GameManager.Instance.WorldMapStorage.GetObject(x, y, z, 0);
        //                if (!!@object && !!@object.Type && (@object.Type.IsGround || @object.Type.IsBottom) && !@object.Type.IsDontHide)
        //                {
        //                    _maxZPlane = z - 1;
        //                    continue;
        //                }
        //                z++;
        //            }
        //        }
        //    }
        //}
    }
    
    ObjItem[,,] array;
    public void RenderGroundAroundMe()
    {
        int count = objList.Count;

        for (int i = 0; i < count; i++)
        {
            if (objList[i] != null)
                Destroy(objList[i]);
        }
        objList.Clear();

       array = AllDataAroundMe();

        parentObj.localScale = new Vector3(1, 1, 1);

        _playerZPlane = GameManager.Instance.WorldMapStorage.ToMap(Player.Position).z;

        UpdateMinMaxZPlane();

        for (int z = 0; z <= _maxZPlane; z++)
        {
            //InternalUpdateFloor(z);
            //InternalDrawFields(commandBuffer, z);

            var absolutePosition = GameManager.Instance.WorldMapStorage.ToAbsolute(new Vector3Int(0, 0, z));
            int size = Constants.MapSizeX + Constants.MapSizeY;
            for (int i = 0; i < size; i++)
            {
                int y = Math.Max(i - Constants.MapSizeX + 1, 0);
                int x = Math.Min(i, Constants.MapSizeX - 1);
                while (x >= 0 && y < Constants.MapSizeY)
                {
                    InternalDrawField(
                        (x + 1) * Constants.FieldSize,
                        (y + 1) * Constants.FieldSize,
                        absolutePosition.x + x,
                        absolutePosition.y + y,
                        absolutePosition.z,
                        x, y, z,
                        true);

                    x--;
                    y++;
                }

                //if (OptionStorage.HighlightMouseTarget && HighlightTile.HasValue && HighlightTile.Value.z == z)
                //    _tileCursor.Draw(commandBuffer, (HighlightTile.Value.x + 1) * Constants.FieldSize, (HighlightTile.Value.y + 1) * Constants.FieldSize, OpenTibiaUnity.TicksMillis);
            }
        }

        //for (int stage = 0; stage < 8; stage++)
        //{
        //    for (int i = 0; i < 12 - 1; i++)
        //    {
        //        for (int j = 0; j < 16 - 1; j++)
        //        {
        //            if (array[j, i, stage]._shouldRecalculateTRS == false)
        //            {
        //                for (int e = 0; e < array[j, i, stage].items.Count; e++)
        //                {
        //                    Vector3 pos = new Vector3(j - (stage * offsetPositionUpStage), 0 + (stage * heightStage), i - (stage * offsetPositionUpStage));
        //                    Vector3.Scale(new Vector3(1, 1, 1), new Vector3(-1, 1, 1));
        //                    try
        //                    {
        //                        GameObject obj = Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}", MapManager.TILE_PREFABS_FOLDER, array[j, i, stage].items[e].ToString())), pos, Quaternion.identity);
        //                        obj.name = array[j, i, stage].items[e].ToString() + " " + pos;
        //                        objList.Add(obj);
        //                        obj.transform.SetParent(parentObj);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        print("not loaded object: " + array[j, i, stage].items[e].ToString());
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        parentObj.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateMinMaxZPlane()
    {

        _maxZPlane = Constants.MapSizeZ - 1;
        while (_maxZPlane > _playerZPlane && GameManager.Instance.WorldMapStorage.GetObjectPerLayer(_maxZPlane) <= 0)
            _maxZPlane--;

        for (int x = Constants.PlayerOffsetX - 1; x <= Constants.PlayerOffsetX + 1; x++)
        {
            for (int y = Constants.PlayerOffsetY - 1; y <= Constants.PlayerOffsetY + 1; y++)
            {
                if (!(x != Constants.PlayerOffsetX && y != Constants.PlayerOffsetY || !GameManager.Instance.WorldMapStorage.IsLookPossible(x, y, _playerZPlane)))
                {
                    int z = _playerZPlane + 1;
                    while (z - 1 < _maxZPlane && x + _playerZPlane - z >= 0 && y + _playerZPlane - z >= 0)
                    {
                        var @object = GameManager.Instance.WorldMapStorage.GetObject(x + _playerZPlane - z, y + _playerZPlane - z, z, 0);
                        if (!!@object && !!@object.Type && @object.Type.IsGround && !@object.Type.IsDontHide)
                        {
                            _maxZPlane = z - 1;
                            continue;
                        }

                        @object = GameManager.Instance.WorldMapStorage.GetObject(x, y, z, 0);
                        if (!!@object && !!@object.Type && (@object.Type.IsGround || @object.Type.IsBottom) && !@object.Type.IsDontHide)
                        {
                            _maxZPlane = z - 1;
                            continue;
                        }
                        z++;
                    }
                }
            }
        }

        if (!GameManager.Instance.WorldMapStorage.CacheFullbank)
        {
            for (int y = 0; y < Constants.MapSizeY; y++)
            {
                for (int x = 0; x < Constants.MapSizeX; x++)
                {
                    int index = y * Constants.MapSizeX + x;

                    _minZPlane[index] = 0;
                    for (int z = _maxZPlane; z > 0; z--)
                    {
                        bool covered = true, done = false;
                        for (int ix = 0; ix < 2 && !done; ix++)
                        {
                            for (int iy = 0; iy < 2 && !done; iy++)
                            {
                                int mx = x + ix;
                                int my = y + iy;
                                if (mx < Constants.MapSizeX && my < Constants.MapSizeY)
                                {
                                    OpenTibiaUnity.Core.Appearances.ObjectInstance @object = GameManager.Instance.WorldMapStorage.GetObject(mx, my, z, 0);
                                    if (!@object || (!!@object.Type && !@object.Type.IsFullGround))
                                    {
                                        covered = false;
                                        done = true;
                                    }
                                }
                            }
                        }

                        if (covered)
                            _minZPlane[index] = z;
                    }
                }
            }

            GameManager.Instance.WorldMapStorage.CacheFullbank = true;
        }
    }

    private void InternalDrawField(int rectX, int rectY, int absoluteX, int absoluteY, int absoluteZ, int positionX, int positionY, int positionZ, bool drawLyingObjects)
    {

        Field field = GameManager.Instance.WorldMapStorage.GetField(positionX, positionY, positionZ);

        int objectsCount = field.ObjectsCount;
        int fieldIndex = positionY * Constants.MapSizeX + positionX;

        bool isCovered = positionZ > _minZPlane[fieldIndex]
            || (positionX == 0 || positionZ >= _minZPlane[fieldIndex - 1])
            || (positionY == 0 || positionZ >= _minZPlane[fieldIndex - Constants.MapSizeX])
            || (positionX == 0 && positionY == 0 || positionZ >= _minZPlane[fieldIndex - Constants.MapSizeX - 1]);

        int objectsHeight = 0;
        // draw ground/bottom objects
        if (drawLyingObjects && objectsCount > 0 && isCovered)
        {
            //InternalDrawFieldObjects(commandBuffer, rectX, rectY, absoluteX, absoluteY, absoluteZ, positionX, positionY, positionZ, field, ref objectsHeight);
            //Draw(absoluteX, absoluteY, absoluteZ);

            var inter = GameManager.Instance.WorldMapStorage.ToMap(new Vector3Int(absoluteX, absoluteY, absoluteZ));
            absoluteX = inter.x;
            absoluteY = inter.y;
            absoluteZ = inter.z;
            //UnityEngine.Debug.Log($"x:{absoluteX} y:{absoluteY} z: {absoluteZ}");

            try
            {
                if (array[absoluteX, absoluteY, absoluteZ] != null && array[absoluteX, absoluteY, absoluteZ]._shouldRecalculateTRS == false)
                {
                    for (int e = 0; e < array[absoluteX, absoluteY, absoluteZ].items.Count; e++)
                    {
                        Vector3 pos = new Vector3(absoluteX - (absoluteZ * offsetPositionUpStage), 0 + (absoluteZ * heightStage), absoluteY - (absoluteZ * offsetPositionUpStage));
                        Vector3.Scale(new Vector3(1, 1, 1), new Vector3(-1, 1, 1));
                        try
                        {
                            GameObject obj = Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}", MapManager.TILE_PREFABS_FOLDER, array[absoluteX, absoluteY, absoluteZ].items[e].ToString())), pos, Quaternion.identity);
                            obj.name = array[absoluteX, absoluteY, absoluteZ].items[e].ToString() + " " + pos;
                            objList.Add(obj);
                            obj.transform.SetParent(parentObj);
                        }
                        catch (Exception ex)
                        {
                            //print("not loaded object: " + array[absoluteX, absoluteY, absoluteZ].items[e].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //print($"not object on array: {absoluteX},{absoluteY},{absoluteZ}");
            }
        }

        // draw creatures
        //InternalDrawFieldCreatures(commandBuffer, positionX, positionY, positionZ, drawLyingObjects, isCovered, objectsHeight);

        // draw effects
        //InternalDrawFieldEffects(commandBuffer, rectX, rectY, absoluteX, absoluteY, absoluteZ, positionZ, drawLyingObjects, field, objectsHeight);

        // draw top objects
        //if (drawLyingObjects)
        //    InternalDrawFieldTopObjects(commandBuffer, rectX, rectY, absoluteX, absoluteY, absoluteZ, field);
    }

    //public enum rowType { left, top, right, bottom }
    private int[,] GetGroundAroundMe()
    {
        int[,] arrayMap = new int[16, 12];
        string miniMap = "";
        for (int i = 1; i < 12; i++)
        {
            miniMap += "[";
            for (int j = 1; j < 16; j++)
            {
                {
                    OpenTibiaUnity.Core.WorldMap.Field _field = GameManager.Instance.WorldMapStorage.GetField(new Vector3Int(j, i, 0));
                    int countItem = _field.ObjectsNetwork.Where(c => c != null).Count();

                    int groundId = 0;
                    if (_field.ObjectsNetwork[0] != null)
                        groundId = (int)_field.ObjectsNetwork[0].Id;

                    arrayMap[j - 1, i - 1] = groundId;

                    miniMap += countItem + ",";
                }
            }
            miniMap += "]" + "\n";
        }
        return arrayMap;
    }
    private ObjItem[,,] AllDataAroundMe()
    {
        int[,] arrayMap = new int[16, 12];

        ObjItem[,,] arrayObjItem = new ObjItem[16, 12, 8];

        string miniMap = "";
        for (int stage = 0; stage < 8; stage++)
        {
            miniMap += "stage: " + stage + "\n";

            for (int i = 1; i < 12; i++)
            {
                miniMap += "[";
                for (int j = 1; j < 16; j++)
                {
                    {
                        OpenTibiaUnity.Core.WorldMap.Field _field = GameManager.Instance.WorldMapStorage.GetField(new Vector3Int(j, i, stage));

                        //UnityEngine.Debug.Log($"Result: {_field.CacheUnsight} - x:{j} y:{i} z: {stage}");

                        //if (!_field.CacheUnsight)
                        //    continue;

                        int countItem = _field.ObjectsNetwork.Where(c => c != null).Count();

                        int groundId = 0;
                        if (_field.ObjectsNetwork[0] != null)
                            groundId = (int)_field.ObjectsNetwork[0].Id;

                        ObjItem _objItem = new ObjItem();

                        bool done = false;
                        for (int e = 0; e < _field.ObjectsNetwork.Length; e++)
                        {
                            if (_field.ObjectsNetwork[e] != null)
                            {
                                if (!done)
                                {
                                    _objItem._shouldRecalculateTRS = _field.ObjectsNetwork[0]._shouldRecalculateTRS;
                                    done = true;

                                    if (stage == 1)
                                    {
                                        _objItem._shouldRecalculateTRS = _field.ObjectsNetwork[0]._shouldRecalculateTRS;
                                    }
                                }
                                _objItem.items.Add((int)_field.ObjectsNetwork[e].Id);
                            }
                        }
                        arrayObjItem[j - 1, i - 1, stage] = _objItem;
                        arrayMap[j - 1, i - 1] = groundId;

                        int value = done == false ? 0 : 1;
                        miniMap += value + ",";
                    }
                }
                miniMap += "]" + "\n";
            }
        }
        //print(miniMap);
        return arrayObjItem;
    }
    public class ObjItem
    {
        public List<long> items;
        public bool _shouldRecalculateTRS;
        public ObjItem()
        {
            _shouldRecalculateTRS = false;
            items = new List<long>();
        }
    }
}
