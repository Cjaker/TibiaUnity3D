using OpenTibiaUnity.Core;
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

    private void Awake()
    {
        inst = this;
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
            int[,] array = GetGroundAroundMe();
            for (int i = 0; i < 12 - 1; i++)
            {
                for (int j = 0; j < 16 - 1; j++)
                {
                    //print(array[j, i]);
                }
            }
            RenderGroundAroundMe();

        }


    }

    public void RenderGroundAroundMe()
    {
        int count = objList.Count;

        for (int i = 0; i < count; i++)
        {
            if (objList[i] != null)
                Destroy(objList[i]);
        }
        objList.Clear();

        ObjItem[,,] array = AllDataAroundMe();

        float heightStage = 2.42f;
        int offsetPositionUpStage = -1;

        parentObj.localScale = new Vector3(1, 1, 1);

        for (int stage = 0; stage < 8; stage++)
        {
            for (int i = 0; i < 12 - 1; i++)
            {
                for (int j = 0; j < 16 - 1; j++)
                {
                    if (array[j, i, stage]._shouldRecalculateTRS == false)
                    {
                        for (int e = 0; e < array[j, i, stage].items.Count; e++)
                        {
                            Vector3 pos = new Vector3(j - (stage * offsetPositionUpStage), 0 + (stage * heightStage), i - (stage * offsetPositionUpStage));
                            Vector3.Scale(new Vector3(1, 1, 1), new Vector3(-1, 1, 1));
                            try
                            {
                                GameObject obj = Instantiate(Resources.Load<GameObject>(string.Format("{0}/{1}", MapManager.TILE_PREFABS_FOLDER, array[j, i, stage].items[e].ToString())), pos, Quaternion.identity);
                                obj.name = array[j, i, stage].items[e].ToString() + " " + pos;
                                objList.Add(obj);
                                obj.transform.SetParent(parentObj);
                            }
                            catch (Exception ex)
                            {
                                print("not loaded object: " + array[j, i, stage].items[e].ToString());
                            }
                        }
                    }
                }
            }
        }
        parentObj.localScale = new Vector3(-1, 1, 1);
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
        print(miniMap);
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
