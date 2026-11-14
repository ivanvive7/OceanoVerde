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

        // Estado 0 (muy sucia) = tile 1 (verde) ... estado 5 (limpia) = tile 6 (azul)
        int estadoInicial = 0;
        if (GameManager.Instancia != null)
        {
            estadoInicial = GameManager.Instancia.EstadoAgua;
            GameManager.Instancia.OnEstadoAguaCambiado += CambiarEstadoAgua;
        }
        SwitchTile(estadoInicial + 1);
    }

    void OnDestroy()
    {
        // Al cambiar de escena hay que dejar de escuchar el evento
        if (GameManager.Instancia != null)
            GameManager.Instancia.OnEstadoAguaCambiado -= CambiarEstadoAgua;
    }

    // Se llama desde el GameManager cuando cambia el estado del agua (0..5)
    private void CambiarEstadoAgua(int estado)
    {
        SwitchTile(estado + 1);
    }

    public void SwitchTile(int tileId)
    {
        TileBase newTileTop = Resources.Load<TileBase>("Tilemaps/water_t_" + tileId);
        TileBase newTileBottom = Resources.Load<TileBase>("Tilemaps/water_b_" + tileId);

        if (newTileTop == null || newTileBottom == null)
        {
            Debug.LogError("Could not load tiles for ID: " + tileId);
            return;
        }

        Debug.Log("Old top: " + oldTileTop.name);
        Debug.Log("New top: " + newTileTop.name);

        Debug.Log("Old bottom: " + oldTileBottom.name);
        Debug.Log("New bottom: " + newTileBottom.name);

        tilemap.SwapTile(oldTileBottom, newTileBottom);
        tilemap.SwapTile(oldTileTop, newTileTop);

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