using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using static UnityEditor.PlayerSettings;

public class TileManager: MonoBehaviour
{
    private Queue<Vector3> _tilesPos;
    private Dictionary<Vector3, GameObject> _tiles;

    private TilesHolder _tilesHolder;

    [SerializeField] private GameObject prefab;
    [SerializeField] private Player player;

    [SerializeField] private float distanceForDeleteTiles = 6f;
    [SerializeField] private float distanceForCreateTiles = 8f;



    private readonly Vector3[] _moveDirections =
    {
        Vector3.right,
        Vector3.forward,
    };

    public void OnEnable()
    {
        _tilesPos = new Queue<Vector3>();
        _tiles = new Dictionary<Vector3, GameObject>();

        CreateTilesHolder();

        var startPos = Vector3Int.CeilToInt(player.transform.position);
        startPos.y = 0;

        AddTile(startPos, prefab);
        Generate(16);
    }

    public void CreateTilesHolder()
    {
        var tilesHolder = GameObject.FindGameObjectWithTag("TilesHolder");
        if(tilesHolder != null)
            Destroy(tilesHolder);

        tilesHolder = new GameObject("TilesHolder")
        {
            tag = "TilesHolder"
        };

        _tilesHolder = tilesHolder.AddComponent<TilesHolder>();
    }

    public void AddTile(Vector3 pos, GameObject prefab)
    {
        var tile = Instantiate(prefab, pos, Quaternion.identity, _tilesHolder.transform);
        tile.name = $"Tile {pos}";

        _tilesPos.Enqueue(pos);
        _tiles.Add(pos, tile);
    }

    public void DeleteFirstTile()
    {
        if(_tilesPos == null || _tilesPos.Count == 0)
        {
            return;
        }

        var tilePos = _tilesPos.Dequeue();
        var tile = _tiles[tilePos];
        _tiles.Remove(tilePos);

        Destroy(tile);
    }

    public void Generate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var lastPos = _tilesPos.Last();
            var newPos = lastPos + _moveDirections[Random.Range(0, 2)];

            AddTile(newPos, prefab);
        }
    }

    public void Update()
    {
        if(_tilesPos != null && _tilesPos.Count != 0)
        {
            if (Vector3.Distance(_tilesPos.Peek(), player.transform.position) > distanceForDeleteTiles) 
            {
                DeleteFirstTile();
            }

            if(_tilesPos.Count == 0)
            {

            }
            else if(Vector3.Distance(_tilesPos.Last(), player.transform.position) < distanceForCreateTiles)
            {
                Generate(1);
            }

        }
    }
}
