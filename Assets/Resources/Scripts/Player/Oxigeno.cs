using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerControllerV2))]
public class Oxigeno : MonoBehaviour
{
    public int burbujasMaximas = 8;
    public float segundosPorBurbuja = 4f;

    private PlayerControllerV2 jugador;
    private BurbujasHUD hud;
    private int burbujas;
    private float temporizador;
    private bool ahogado;
    private AudioClip sonidoAhogarse;
    [Range(0f, 1f)] public float volumen = 1f;

    void Start()
    {
        sonidoAhogarse = Resources.Load<AudioClip>("sounds/sound");
        if (sonidoAhogarse == null)
            Debug.LogWarning("No se encontró Resources/sounds/sound");
        jugador = GetComponent<PlayerControllerV2>();
        hud = BurbujasHUD.Crear(burbujasMaximas);
        RecuperarBurbujas();
    }

    void Update()
    {
        if (ahogado) return;
        if (GameManager.Instancia != null && GameManager.Instancia.NivelTerminado) return;

        if (!jugador.EstaBajoElAgua)
        {
            if (burbujas < burbujasMaximas || temporizador > 0f)
                RecuperarBurbujas();
            return;
        }

        temporizador += Time.deltaTime;
        if (temporizador < segundosPorBurbuja) return;

        temporizador -= segundosPorBurbuja;
        burbujas--;
        hud.Mostrar(burbujas);

        if (burbujas <= 0)
            Ahogarse();
    }

    private void RecuperarBurbujas()
    {
        burbujas = burbujasMaximas;
        temporizador = 0f;
        hud.Mostrar(burbujas);
    }

    private void Ahogarse()
    {
        ahogado = true;
        Debug.Log("Te has quedado sin oxígeno");
        if (sonidoAhogarse != null)
            Sfx.Play(sonidoAhogarse, volumen);

        jugador.enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        if (GameManager.Instancia != null)
            GameManager.Instancia.Perder();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
