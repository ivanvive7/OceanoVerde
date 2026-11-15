using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la partida: basura del nivel, puntuación, estado del agua y cambio de escenas.
/// Es un singleton: solo existe uno y no se destruye al cambiar de escena (DontDestroyOnLoad).
/// Se pone en un GameObject vacío llamado "GameManager" en la escena del menú
/// (y si se quiere probar un nivel directamente, también en el nivel).
/// </summary>
public class GameManager : MonoBehaviour
{
    // Acceso global: GameManager.Instancia.Metodo()
    public static GameManager Instancia { get; private set; }

    // Número de estados del agua: 0 = muy sucia ... 5 = limpia
    public const int ESTADOS_AGUA = 6;
    public const int ESTADO_LIMPIA = ESTADOS_AGUA - 1;

    [Header("Escenas")]
    public string escenaMenu = "MenuPrincipal";
    public float segundosAntesDeSiguienteNivel = 2f;   // para que se vea el agua limpia

    // Avisa a quien le interese (por ejemplo WaterManager) de que el agua ha cambiado de estado
    public event Action<int> OnEstadoAguaCambiado;

    // Datos del nivel actual
    private int basuraTotal;        // basura que ha aparecido en este nivel
    private int basuraRecogida;     // basura que el jugador ya ha recogido
    private int estadoAgua;         // 0 .. 5

    // Puntuación
    private int puntuacion;              // puntos de toda la partida
    private int puntuacionInicioNivel;   // puntos que tenía al empezar el nivel (para volver a ellos al morir)
    private bool nivelTerminado;
    private int basuraRecogidaTotal;

    // Propiedades de solo lectura para la interfaz (HUD) u otros scripts
    public int BasuraTotal { get { return basuraTotal; } }
    public int TotalRecogidas { get { return basuraRecogida; } }
    public int BasuraPendiente { get { return basuraTotal - basuraRecogida; } }
    public int EstadoAgua { get { return estadoAgua; } }
    public int Puntuacion { get { return puntuacion; } }
    public bool NivelTerminado { get { return nivelTerminado; } }
    public int BasuraRecogidaTotal { get { return basuraRecogidaTotal; } }

    private void Awake()
    {
        // Si ya hay un GameManager (venimos de otra escena), este sobra
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            Instancia = null;
        }
    }

    // Se llama cada vez que se carga una escena (nivel nuevo o el mismo al morir).
    // Ocurre antes de los Start() de la escena, así que la basura se registra después.
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        ReiniciarContadores();
        puntuacionInicioNivel = puntuacion;

        if (FindFirstObjectByType<PlayerControllerV2>() != null)
            ContadorBasuraHUD.Crear();
    }

    private void ReiniciarContadores()
    {
        basuraTotal = 0;
        basuraRecogida = 0;
        estadoAgua = 0;
        nivelTerminado = false;
    }

    // ---------- Basura ----------

    /// <summary>La llama cada basura que SÍ aparece al cargar el nivel.</summary>
    public void RegistrarBasura()
    {
        basuraTotal++;
    }

    /// <summary>La llama una basura cuando el jugador la recoge.</summary>
    public void BasuraRecogida(int puntos)
    {
        if (nivelTerminado) return;

        basuraRecogida++;
        puntuacion += puntos;

        CalcularEstadoAgua();

        if (BasuraPendiente <= 0)
        {
            FinNivel();
        }
    }

    // Reparte los 6 estados según la basura que haya en este nivel.
    // Ejemplo con 10 basuras: 0-1 recogidas = estado 0, 2-3 = estado 1, ... 10 = estado 5.
    private void CalcularEstadoAgua()
    {
        int nuevoEstado;

        if (basuraTotal <= 0 || BasuraPendiente <= 0)
            nuevoEstado = ESTADO_LIMPIA;   // ya no queda basura: agua limpia
        else
            nuevoEstado = Mathf.FloorToInt(basuraRecogida * (float)ESTADO_LIMPIA / basuraTotal);

        if (nuevoEstado != estadoAgua)
        {
            estadoAgua = nuevoEstado;
            Debug.Log("Estado del agua: " + estadoAgua);

            // El ?. evita error si nadie se ha suscrito al evento
            OnEstadoAguaCambiado?.Invoke(estadoAgua);
        }
    }

    // ---------- Fin de nivel y escenas ----------

    /// <summary>Se llama cuando ya no queda basura en el nivel.</summary>
    public void FinNivel()
    {
        nivelTerminado = true;
        basuraRecogidaTotal += basuraRecogida;
        Debug.Log("¡Nivel completado! Puntuación: " + puntuacion);

        if (EsUltimoNivel())
        {
            MensajesHUD.MostrarJuegoCompletado(basuraRecogidaTotal);
            return;
        }

        MensajesHUD.MostrarMensaje("NIVEL COMPLETADO");
        Invoke(nameof(CargarSiguienteNivel), segundosAntesDeSiguienteNivel);
    }

    private bool EsUltimoNivel()
    {
        return SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings;
    }

    private void CargarSiguienteNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Perder()
    {
        if (nivelTerminado) return;

        nivelTerminado = true;
        MensajesHUD.MostrarMensaje("HAS PERDIDO");
        Invoke(nameof(ReiniciarNivel), segundosAntesDeSiguienteNivel);
    }

    /// <summary>Vuelve a cargar el nivel actual (por ejemplo al morir).</summary>
    public void ReiniciarNivel()
    {
        CancelInvoke();
        puntuacion = puntuacionInicioNivel;   // se pierden los puntos conseguidos en este intento
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Empieza una partida nueva desde el primer nivel.</summary>
    public void NuevaPartida(string primerNivel)
    {
        CancelInvoke();
        puntuacion = 0;
        basuraRecogidaTotal = 0;
        SceneManager.LoadScene(primerNivel);
    }

    public void IrAlMenu()
    {
        CancelInvoke();
        SceneManager.LoadScene(escenaMenu);
    }
}
