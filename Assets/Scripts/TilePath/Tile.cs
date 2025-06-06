using UnityEngine;

public class Tile
{
    public string Name;

    public GameObject tileObject;
    public CacheObject cacheObject;

    public Vector3Int position;
    public Vector3Int nextTilePos;

    public bool IsActive => tileObject.activeSelf;
    public bool IsLastInPath = false;


    public Tile(GameObject tileObject, Vector3Int position)
    {
        this.tileObject = tileObject;
        SetPosition(position);
        SetName($"Tile {position}");
    }

    private void SetName(string name)
    {
        Name = name;
        tileObject.name = name;
    }

    private void SetPosition(Vector3Int position)
    {
        this.position = position;
        tileObject.transform.position = position;
    }

    public void Activate(Vector3Int newPosition)
    {
        SetPosition(newPosition);
        SetName($"Tile {position}"); ;
        tileObject.SetActive(true);
        ResetParameters();
    }

    public void ResetParameters()
    {
        cacheObject = null;
        nextTilePos = Vector3Int.zero;
        IsLastInPath = false;
    }

    public void Deactivate()
    {
        tileObject.SetActive(false);
        if (cacheObject != null)
        {
            cacheObject.Deactivate();
            cacheObject.ToRelease();
            cacheObject = null;
        }
    }
}
