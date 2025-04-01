using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class TilesHolder: MonoBehaviour 
{
    private Dictionary<string, Queue<GameObject>> _inactiveTilesPools;
    private Dictionary<Vector3, GameObject> _activeTilesByPosition;

    public void Innit()
    {
        _activeTilesByPosition = new();
        _inactiveTilesPools = new();
    }

    public GameObject CreateTile(Vector3 position, GameObject prefab)
    {
        //Check if position is busy
        if (_activeTilesByPosition.ContainsKey(position))
        {
            HideTile(position);
        }

        if(!_inactiveTilesPools.TryGetValue(prefab.name, out var pool))
        {
            pool = new Queue<GameObject>();
            _inactiveTilesPools.Add(prefab.name, pool);
        }

        GameObject tile;
        if (TryFindInactiveTile(pool, out var foundTile)) 
        { 
            tile = foundTile;
            pool.Dequeue();
            ActiveTile(position, tile);
        }
        else
        {
            tile = InstantiateTile(position, prefab);
        }

         pool.Enqueue(tile);
        _activeTilesByPosition.Add(position, tile);
        return tile;
    }

    public void ActiveTile(Vector3 position, GameObject tile)
    {
        tile.transform.position = position;
        tile.SetActive(true);
        tile.name = $"Tile: {position}";
    }

    public void HideTile(Vector3 position)
    {
        if (_activeTilesByPosition.TryGetValue(position, out var foundTile))
        {
            _activeTilesByPosition.Remove(position);
            foundTile.SetActive(false);
        }
    }

    private bool TryFindInactiveTile(List<GameObject> tiles, out GameObject foundTile)
    {
        foundTile = null;
        foreach (var tile in tiles) 
        {
            if (!tile.activeSelf)
            {
                foundTile = tile;
                return true;
            }
        }
        return false;
    }

    private bool TryFindInactiveTile(Queue<GameObject> tiles, out GameObject foundTile)
    {
        foundTile = null;
        if (tiles.Count == 0) return false;

        var tile = tiles.Peek();

        if (tile.activeSelf)
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

    private GameObject InstantiateTile(Vector3 position, GameObject prefab)
    {
        var tile = Instantiate(prefab, position, Quaternion.identity, transform);
        tile.name = $"Tile {position}";

        return tile ;
    }

}
