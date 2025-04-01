using System.Collections.Generic;
using UnityEngine;

public class TilesHolder: MonoBehaviour 
{
    private Dictionary<string, List<GameObject>> _instantiatedTilesDictionary;
    private Dictionary<Vector3, GameObject> _activeTilesByPositionDictionary;

    private void Start()
    {
        _activeTilesByPositionDictionary = new();
        _instantiatedTilesDictionary = new();
    }

    public GameObject CreateTile(Vector3 position, GameObject prefab)
    {
        var listOfTiles = _instantiatedTilesDictionary[prefab.name];
        GameObject tile;

        if(listOfTiles == null)
        {
            listOfTiles = new List<GameObject>();
            _instantiatedTilesDictionary.Add(prefab.name, listOfTiles);
        }

        if (TryFindInactiveTile(listOfTiles, out var foundTile)) 
        { 
            tile = foundTile;
        }
        else
        {
            tile = InstantiateTile(position, prefab);
            listOfTiles.Add(tile);
        }

        _activeTilesByPositionDictionary.Add(position, tile);
        return tile;
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
            if (tile.activeSelf)
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
