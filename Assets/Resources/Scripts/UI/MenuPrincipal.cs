using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Botones del menú principal. Se pone en el Canvas de la escena MenuPrincipal
/// y cada botón llama a su método desde el evento OnClick() del Inspector.
/// </summary>
public class MenuPrincipal : MonoBehaviour
{
    public string primerNivel = "PrimerNivel";
    public GameObject panelCreditos;   // panel que se enseña/oculta con el botón Créditos

    private void Start()
    {
        if (panelCreditos != null)
            panelCreditos.SetActive(false);
    }

    public void Jugar()
    {
        // El GameManager pone la puntuación a 0 y carga el nivel
        if (GameManager.Instancia != null)
            GameManager.Instancia.NuevaPartida(primerNivel);
        else
            SceneManager.LoadScene(primerNivel);
    }

    public void MostrarCreditos(bool mostrar)
    {
        if (panelCreditos != null)
            panelCreditos.SetActive(mostrar);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();   // en el editor no hace nada, solo funciona en el juego compilado

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // para poder probarlo en el editor
#endif
    }
}
