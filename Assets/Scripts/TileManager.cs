using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class TileManager: MonoBehaviour
{
    private Queue<Vector3> _tilesPos;

    private TilesHolder _tilesHolder;
    private Vector3 _lastTilePosition;

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

        CreateTilesHolder();

        var startPos = Vector3Int.RoundToInt(player.transform.position);
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
        _tilesHolder.Innit();
    }

    public void AddTile(Vector3 pos, GameObject prefab)
    {
        _tilesPos.Enqueue(pos);
        _tilesHolder.CreateTile(pos, prefab);
        _lastTilePosition = pos;
    }

    public void DeleteFirstTile()
    {
        if(_tilesPos == null || _tilesPos.Count == 0)
        {
            return;
        }

        var tilePos = _tilesPos.Dequeue();
        _tilesHolder.HideTile(tilePos);
    }

    public void Generate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var lastPos = _lastTilePosition;
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
            else if(Vector3.Distance(_lastTilePosition, player.transform.position) < distanceForCreateTiles)
            {
                Generate(1);
            }

        }
    }
}
