using System.Collections.Generic;
using UnityEngine;

public class TilesHolder: MonoBehaviour 
{
    private Dictionary<string, List<GameObject>> _instantiatedTilesDictionary;
    private Dictionary<Vector3, GameObject> _activeTilesByPositionDictionary;

    public void Innit()
    {
        _activeTilesByPositionDictionary = new();
        _instantiatedTilesDictionary = new();
    }

    public GameObject CreateTile(Vector3 position, GameObject prefab)
    {
        //Check if position is busy
        if (_activeTilesByPositionDictionary.ContainsKey(position))
        {
            HideTile(position);
        }

        var listOfTiles = _instantiatedTilesDictionary.GetValueOrDefault(prefab.name);
        GameObject tile;

        if(listOfTiles == null)
        {
            listOfTiles = new List<GameObject>();
            _instantiatedTilesDictionary.Add(prefab.name, listOfTiles);
        }

        if (TryFindInactiveTile(listOfTiles, out var foundTile)) 
        { 
            tile = foundTile;
            ActiveTile(position, tile);
        }
        else
        {
            tile = InstantiateTile(position, prefab);
            listOfTiles.Add(tile);
        }

        _activeTilesByPositionDictionary.Add(position, tile);
        return tile;
    }

    public void ActiveTile(Vector3 position, GameObject tile)
    {
        tile.transform.position = position;
        tile.SetActive(true);
    }

    public void HideTile(Vector3 position)
    {
        var tile = _activeTilesByPositionDictionary[position];
        _activeTilesByPositionDictionary.Remove(position);

        tile.SetActive(false);
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

    private GameObject InstantiateTile(Vector3 position, GameObject prefab)
    {
        var tile = Instantiate(prefab, position, Quaternion.identity, transform);
        tile.name = $"Tile {position}";

        return tile ;
    }

}
