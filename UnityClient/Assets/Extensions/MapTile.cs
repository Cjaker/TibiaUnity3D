using OpenTibiaUnity;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public struct Item
    {
        public uint Id;
        public GameObject SpawnedGO;

        public Item(uint id, GameObject spawnedGO)
        {
            Id = id;
            SpawnedGO = spawnedGO;
        }
    }

    public Item[] Items = new Item[Constants.MapSizeW];

    public void RefreshTile()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            var newSpawned = SpawnItem(Items[i].Id, transform);

            if (Items[i].SpawnedGO != null)
                Destroy(Items[i].SpawnedGO);

            Items[i].SpawnedGO = newSpawned;
        }
    }

    private GameObject SpawnItem(uint id, Transform parent)
    {
        var loaded = Resources.Load<GameObject>(string.Format("{0}/{1}", MapManager.TILE_PREFABS_FOLDER, id));
        if (loaded == null) return null;

        var item = GameObject.Instantiate(loaded);
        item.transform.SetParent(parent);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        return item;
    }
}
