using UnityEngine;

/// <summary>
/// Zona que mata al jugador (por ejemplo debajo del mapa, pinchos, etc.).
/// Se pone en un objeto con un Collider2D marcado como "Is Trigger".
/// Al morir se recarga el nivel y la basura vuelve a decidir si aparece.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZonaMuerte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("El jugador ha muerto. Reiniciando nivel...");

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.ReiniciarNivel();
        }
    }
}
