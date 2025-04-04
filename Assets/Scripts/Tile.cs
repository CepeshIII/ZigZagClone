using UnityEngine;

public class Tile
{
    public string Name;

    public GameObject tileObject;
    public GameObject collectableItem;

    public Vector3 position;

    public Vector3 nextTilePos;

    public bool IsActive => tileObject.activeSelf;
    public bool IsLastInPath = false;


    public Tile(GameObject tileObject, Vector3 position)
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

    private void SetPosition(Vector3 position)
    {
        this.position = position;
        tileObject.transform.position = position;
    }

    public void Activate(Vector3 newPosition)
    {
        SetPosition(newPosition);
        SetName($"Tile {position}"); ;
        tileObject.SetActive(true);
    }

    public void Deactivate()
    {
        tileObject.SetActive(false);
        if (collectableItem != null)
            collectableItem.SetActive(false);
    }
}
