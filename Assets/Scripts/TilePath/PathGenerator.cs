using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGenerator: MonoBehaviour
{
    private TilePoolManager _tilePoolManager;

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


        if (_tilePoolManager != null)
        {
            if (_tilePoolManager.FirstActivateTile != null)
            {
                var distantToPathStart = Vector3.Distance(_tilePoolManager.FirstActivateTile.position,
                                            player.transform.position);
                if (distantToPathStart > distanceForDeleteTiles)
                {
                    _tilePoolManager.HideFirstTile();
                }
            }

            if (_tilePoolManager.LastActivateTile != null)
            {
                var distantToEndStart = Vector3.Distance(_tilePoolManager.LastActivateTile.position,
                                            player.transform.position);
                if (distantToEndStart < distanceForCreateTiles)
                {
                    GenerateTilesRandom(1);
                }
            }
        }
    }

    public void GenerateLine(Vector3Int startPos, Vector3Int direction, int length, GameObject prefab)
    {
        for(int i = 0; i < length; i++)
        {
            AddTile(startPos + i * direction, prefab);
        }
    }

    public void CreateTilesHolder()
    {
        var tilesHolder = GameObject.FindGameObjectWithTag("TilePoolManager");
        if(tilesHolder != null)
            Destroy(tilesHolder.gameObject);

        tilesHolder = new GameObject("TilePoolManager")
        {
            tag = "TilePoolManager"
        };

        _tilePoolManager = tilesHolder.AddComponent<TilePoolManager>();
        _tilePoolManager.Innit();
    }

    public void AddTile(Vector3Int pos, GameObject prefab)
    {
        var lastTile = _tilePoolManager.AddTile(pos, prefab);

        if(Random.Range(0, 10) < 3)
        {
            _tilePoolManager.AddCollectItem(lastTile, GetRandomInList(collectableItemPrefabs));
        }
    }

    public void GenerateTilesRandom(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var lastPos = _tilePoolManager.LastActivateTile.position;
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
        _tilePoolManager.Clear();

        _tilePoolManager.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

        Destroy(_tilePoolManager.gameObject);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(_tilePoolManager!= null)
            Destroy(_tilePoolManager.gameObject);

    }
}
