using UnityEngine;

public class TrashPickup : MonoBehaviour
{
    [SerializeField] Sprite[] variations;
    [SerializeField] int destroyPosibility = 5;
    [SerializeField] int puntos = 10;                      // puntos que da al recogerla
    [HideInInspector] public bool skipDestroyRoll;
    private float secondsPerBob = 0.25f;
    private static float unitsPerPixel = 0.0625f;
    private float timer;
    private bool dir;
    private bool recogida;
    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (variations.Length > 0)
            sr.sprite = variations[Random.Range(0, variations.Length)];

        sr.flipX = Random.value > 0.5f;

        GetComponent<Collider2D>().isTrigger = true;

        timer = -(transform.position.x / 5);
    }

    void Start()
    {
        // Thrown by an enemy: the Enemy already registered it and it skips the spawn roll
        if (skipDestroyRoll) return;

        if (Random.Range(0, destroyPosibility) == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instancia != null)
            GameManager.Instancia.RegistrarBasura();
        else
            Debug.LogWarning("TrashPickup: no hay GameManager en la escena, la basura no se cuenta.");
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < secondsPerBob) return;

        timer -= secondsPerBob;
        dir = !dir;
        transform.position += (dir ? Vector3.up : Vector3.down) * unitsPerPixel*2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Solo la recoge el jugador, y solo una vez
        if (recogida || !other.CompareTag("Player")) return;

        recogida = true;
        Debug.Log("Objeto recogido");

        // Avisa al GameManager: suma puntos, recalcula el agua y, si no queda basura, acaba el nivel
        if (GameManager.Instancia != null)
            GameManager.Instancia.BasuraRecogida(puntos);

        Destroy(gameObject);
    }
}
