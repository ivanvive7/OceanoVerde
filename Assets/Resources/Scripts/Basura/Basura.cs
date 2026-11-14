using UnityEngine;

/// <summary>
/// Objeto de basura que el jugador recoge al tocarlo.
/// Al cargar el nivel decide con Random si aparece o no.
/// Necesita un Collider2D marcado como "Is Trigger".
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Basura : MonoBehaviour
{
    [Range(0f, 1f)]
    public float probabilidadAparecer = 0.7f;   // 1 = siempre aparece, 0 = nunca
    public int puntos = 10;                      // puntos que da al recogerla

    private bool recogida;

    private void Start()
    {
        // Random.value devuelve un número entre 0 y 1.
        // Si sale mayor que la probabilidad, esta basura no aparece en esta partida.
        if (Random.value > probabilidadAparecer)
        {
            Destroy(gameObject);
            return;
        }

        // Ha aparecido: se lo decimos al GameManager para que la cuente.
        // Se hace en Start (y no en Awake) porque el GameManager pone los contadores
        // a 0 al cargar la escena, y eso ocurre antes de los Start.
        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.RegistrarBasura();
        }
        else
        {
            Debug.LogWarning("Basura: no hay GameManager en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo la recoge el jugador, y solo una vez
        if (recogida || !other.CompareTag("Player")) return;

        recogida = true;

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.BasuraRecogida(puntos);
        }

        Destroy(gameObject);
    }
}
