using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class TileManager: MonoBehaviour
{
    private TilesHolder _tilesHolder;

    [SerializeField] private List<GameObject> tilePrefabs;
    [SerializeField] private List<GameObject> collectableItemPrefabs;
    [SerializeField] private Player player;

    [SerializeField] private float distanceForDeleteTiles = 6f;
    [SerializeField] private float distanceForCreateTiles = 8f;


    private readonly Vector3Int[] _moveDirections =
    {
        Vector3Int.right,
        Vector3Int.forward,
    };

    public void OnEnable()
    {
        CreateTilesHolder();

        var startPos = Vector3Int.RoundToInt(player.transform.position);
        startPos.y = 0;

        GenerateLine(startPos, _moveDirections[1], 10, GetRandomInList(tilePrefabs));
        GenerateTilesRandom(16);
    }

    public void Update()
    {
        var playerTilePos = Vector3Int.RoundToInt(player.transform.position);
        playerTilePos.y = 0;


        if (_tilesHolder != null)
        {
            if (_tilesHolder.FirstActivateTile != null)
            {
                var distantToPathStart = Vector3.Distance(_tilesHolder.FirstActivateTile.position,
                                            player.transform.position);
                if (distantToPathStart > distanceForDeleteTiles)
                {
                    _tilesHolder.HideFirstTile();
                }
            }

            if (_tilesHolder.LastActivateTile != null)
            {
                var distantToEndStart = Vector3.Distance(_tilesHolder.LastActivateTile.position,
                                            player.transform.position);
                if (distantToEndStart < distanceForCreateTiles)
                {
                    GenerateTilesRandom(1);
                }
            }
        }
    }

    public void GenerateLine(Vector3 startPos, Vector3Int direction, int length, GameObject prefab)
    {
        for(int i = 0; i < length; i++)
        {
            AddTile(startPos + i * direction, prefab);
        }
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
        var lastTile = _tilesHolder.AddTile(pos, prefab);

        if(Random.Range(0, 10) < 3)
        {
            _tilesHolder.AddCollectItem(lastTile, GetRandomInList(collectableItemPrefabs));
        }
    }

    public void GenerateTilesRandom(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var lastPos = _tilesHolder.LastActivateTile.position;
            var newPos = lastPos + _moveDirections[Random.Range(0, 2)];

            AddTile(newPos, GetRandomInList(tilePrefabs));
        }
    }

    public T GetRandomInList<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public void Destroy()
    {
        _tilesHolder.Clear();

        _tilesHolder.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
