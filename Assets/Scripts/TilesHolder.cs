using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class TilesHolder: MonoBehaviour 
{
    private Dictionary<string, Queue<Tile>> _initializedTilesPools;
    private Dictionary<string, List<GameObject>> _initializedCollectableItemLists;

    private Dictionary<Vector3, Tile> _activeTilesByPosition;

    private Tile firstActivateTile;
    private Tile lastActivateTile;

    public Tile FirstActivateTile {  get { return firstActivateTile; } }
    public Tile LastActivateTile {  get { return lastActivateTile; } }


    public void Innit()
    {
        _activeTilesByPosition = new();
        _initializedTilesPools = new();
        _initializedCollectableItemLists = new();
    }

    public Tile AddTile(Vector3 position, GameObject prefab)
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

    public GameObject AddCollectItem(Tile tile, GameObject prefab)
    {
        if (!_initializedCollectableItemLists.TryGetValue(prefab.name, out var list))
        {
            list = new();
            _initializedCollectableItemLists.Add(prefab.name, list);
        }

        if (TryFindInactiveCollectableItem(list, out var collectableItem))
        {
            collectableItem.transform.position = tile.position + Vector3.up;
            collectableItem.SetActive(true);
        }
        else
        {
            collectableItem = InstantiateCollectItem(tile.position + Vector3.up, prefab);
            list.Add(collectableItem);
        }

        tile.collectableItem = collectableItem;

        return collectableItem;
    }

    public void HideTile(Vector3 position)
    {
        if (_activeTilesByPosition.TryGetValue(position, out var foundTile))
        {
            _activeTilesByPosition.Remove(position);
            foundTile.Deactivate();
        }
    }


    private bool TryFindInactiveCollectableItem(List<GameObject> collectableItems, 
        out GameObject foundCollectableItem)
    {
        foundCollectableItem = null;
        foreach (var collectableItem in collectableItems)
        {
            if (!collectableItem.activeSelf)
            {
                foundCollectableItem = collectableItem;
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

    private Tile InstantiateTile(Vector3 position, GameObject prefab)
    {
        var tileObject = Instantiate(prefab, position, Quaternion.identity, transform);
        var tile = new Tile(tileObject, position);
        return tile;
    }

    public GameObject InstantiateCollectItem(Vector3 position, GameObject prefab)
    {
        var tileObject = Instantiate(prefab, position, Quaternion.identity, transform);
        return tileObject;
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

    public void DestroyCollectableItems()
    {
        foreach (var keys in _initializedCollectableItemLists.Keys)
        {
            foreach (var collectableItem in _initializedCollectableItemLists[keys])
            {
                if (collectableItem != null)
                    Destroy(collectableItem);
            }
        }
        _initializedCollectableItemLists.Clear();
    }

    public void Clear()
    {
        DestroyTiles();
        DestroyCollectableItems();

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
