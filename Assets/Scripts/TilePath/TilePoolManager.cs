using System.Collections.Generic;
using UnityEngine;


public class CacheObject
{
    private bool isFree;
    private GameObject gameObject;

    public GameObject GameObject => gameObject;
    public bool IsFree => isFree;

    public void SetGameObject(GameObject newGameObject)
    {
        gameObject = newGameObject;
    }

    public void Update(string name, Vector3 position) 
    {
        gameObject.name = name;
        gameObject.transform.position = position;
    }

    public void ToRelease()
    {
        isFree = true;
    }

    public void ToReserve()
    {
        isFree = false;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}


public class TilePoolManager: MonoBehaviour 
{
    private Dictionary<string, Queue<Tile>> _initializedTilesPools;
    private Dictionary<string, List<CacheObject>> _initializedCacheObjectsLists;
    private Dictionary<Vector3Int, Tile> _activeTilesByPosition;

    private Transform _cacheObjectsParent;
    private Transform _tilesParent;

    private Tile firstActivateTile;
    private Tile lastActivateTile;

    public Tile FirstActivateTile {  get { return firstActivateTile; } }
    public Tile LastActivateTile {  get { return lastActivateTile; } }


    public void Innit()
    {
        _activeTilesByPosition = new();
        _initializedTilesPools = new();
        _initializedCacheObjectsLists = new();

        _tilesParent = new GameObject("Tiles parent").transform;
        _cacheObjectsParent = new GameObject("Cache Objects parent").transform;

        _tilesParent.parent = this.transform;
        _cacheObjectsParent.parent = this.transform;
    }

    public Tile AddTile(Vector3Int position, GameObject prefab)
    {
        //Check if position is busy
        if (_activeTilesByPosition.ContainsKey(position))
        {
            HideTile(position);
        }

        //Check if pool for this type of Tiles exist, and initialize one if not
        if (!_initializedTilesPools.TryGetValue(prefab.name, out var pool))
        {
            pool = new Queue<Tile>();
            _initializedTilesPools.Add(prefab.name, pool);
        }

        //Checking if pool has a inactive tile. this is for re use a cached tiles
        if (TryFindInactiveTile(pool, out var tile)) 
        { 
            pool.Dequeue();
            tile.Activate(position);
        }
        else
        {
            tile = InstantiateTile(position, prefab);
        }

         pool.Enqueue(tile);
        _activeTilesByPosition.Add(position, tile);

        if (firstActivateTile == null)
        {
            firstActivateTile = tile;
        }
        if(lastActivateTile != null)
        {
            lastActivateTile.nextTilePos = tile.position;
        }

        lastActivateTile = tile;

        return tile;
    }

    public CacheObject AddCacheObject(Tile tile, GameObject prefab)
    {
        if (!_initializedCacheObjectsLists.TryGetValue(prefab.name, out var list))
        {
            list = new();
            _initializedCacheObjectsLists.Add(prefab.name, list);
        }

        if (TryFindFreeCacheObject(list, out var cacheObject))
        {
            ActivateCacheObject(cacheObject, tile.position);
        }
        else
        {
            cacheObject = CreateCacheObject(tile.position, prefab);
            list.Add(cacheObject);
        }
        ReserveCacheObject(tile, cacheObject);

        return cacheObject;
    }

    public void ReserveCacheObject(Tile tile, CacheObject cacheObject)
    {
        tile.cacheObject = cacheObject;
        cacheObject.ToReserve();
    }

    public void HideTile(Vector3Int position)
    {
        if (_activeTilesByPosition.TryGetValue(position, out var foundTile))
        {
            _activeTilesByPosition.Remove(position);
            foundTile.Deactivate();
        }
    }

    private bool TryFindFreeCacheObject(List<CacheObject> cacheObjects, 
        out CacheObject foundCacheObject)
    {
        foundCacheObject = null;
        foreach (var cacheObject in cacheObjects)
        {
            if (cacheObject.IsFree)
            {
                foundCacheObject = cacheObject;
                return true;
            }
        }
        return false;
    }

    private bool TryFindInactiveTile(Queue<Tile> tiles, out Tile foundTile)
    {
        foundTile = null;
        if (tiles.Count == 0) return false;

        var tile = tiles.Peek();

        if (tile.IsActive)
        {
            foundTile = null;
            return false;
        }
        else
        {
            foundTile = tile;
            return true;
        }
    }

    private Tile InstantiateTile(Vector3Int position, GameObject prefab)
    {
        var tileObject = Instantiate(prefab, position, Quaternion.identity, _tilesParent);
        var tile = new Tile(tileObject, position);
        return tile;
    }

    public CacheObject CreateCacheObject(Vector3Int position, GameObject prefab)
    {
        var gameObject = Instantiate(prefab, _cacheObjectsParent);
        var cacheObject = new CacheObject();

        cacheObject.SetGameObject(gameObject);
        ActivateCacheObject(cacheObject, position);

        return cacheObject;
    }

    public void ActivateCacheObject(CacheObject cacheObject, Vector3Int position)
    {
        cacheObject.Activate();
        cacheObject.Update($"CacheObject: {position}", position + Vector3Int.up);
    }

    public void DestroyTiles()
    {
        if (_initializedTilesPools == null) return;

        foreach (var tilePools in _initializedTilesPools)
        {
            if (tilePools.Value == null) continue;

            foreach (var tile in tilePools.Value)
            {
                if(tile != null)
                    DestroyTile(tile);
            }
        }
    }

    public void HideFirstTile()
    {
        if(firstActivateTile != null)
        {
            var tile = firstActivateTile;

            if (_activeTilesByPosition.ContainsKey(tile.nextTilePos))
            {
                firstActivateTile = _activeTilesByPosition[tile.nextTilePos];
                HideTile(tile.position);
            }
        }
    }

    public void DestroyCacheObjects()
    {
        foreach (var keys in _initializedCacheObjectsLists.Keys)
        {
            foreach (var cacheObject in _initializedCacheObjectsLists[keys])
            {
                if (cacheObject != null){
                    if( cacheObject.GameObject != null)
                    {
                        Destroy(cacheObject.GameObject);
                    }
                }
            }
        }
        _initializedCacheObjectsLists.Clear();
    }

    public void Clear()
    {
        DestroyTiles();
        DestroyCacheObjects();

        _initializedTilesPools.Clear();
        _initializedTilesPools.TrimExcess();

        _activeTilesByPosition.Clear();
        _activeTilesByPosition.TrimExcess();
    }

    public void DestroyTile(Tile tile)
    {
        Destroy(tile.tileObject);
    }

}
