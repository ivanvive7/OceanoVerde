using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class WaterManager : MonoBehaviour
{
    public float secondsPerFlip = 0.5f;

    private Tilemap tilemap;
    private float timer;
    private bool flipped;
    private TileBase oldTileTop;
    private TileBase oldTileBottom;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        oldTileTop = Resources.Load<TileBase>("Tilemaps/water_t_6");
        oldTileBottom = Resources.Load<TileBase>("Tilemaps/water_b_6");

        Debug.Log("Top tile: " + oldTileTop);
        Debug.Log("Bottom tile: " + oldTileBottom);
        
        SwitchTile(2);
    }

    void SwitchTile(int tileId)
    {
        TileBase newTileTop = Resources.Load<TileBase>("Tilemaps/water_t_" + tileId);
        TileBase newTileBottom = Resources.Load<TileBase>("Tilemaps/water_b_" + tileId);

        tilemap.SwapTile(oldTileTop, newTileTop);
        tilemap.SwapTile(oldTileBottom, newTileBottom);

        oldTileTop = newTileTop;
        oldTileBottom = newTileBottom;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < secondsPerFlip) return;

        timer -= secondsPerFlip;
        flipped = !flipped;

        transform.localScale = new Vector3(flipped ? -1 : 1, 1, 1);
    }
}