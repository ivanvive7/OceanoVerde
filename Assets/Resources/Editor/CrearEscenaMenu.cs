#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Herramienta de editor que crea la escena del menú principal automáticamente
/// y la pone como escena 0 en Build Settings (PrimerNivel queda como escena 1).
/// Uso: menú Tools > Crear escena Menu Principal.
/// </summary>
public static class CrearEscenaMenu
{
    const string RUTA_MENU = "Assets/Resources/Scenes/MenuPrincipal.unity";
    const string RUTA_NIVEL1 = "Assets/Resources/Scenes/PrimerNivel.unity";

    [MenuItem("Tools/Crear escena Menu Principal")]
    public static void Crear()
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---- Cámara 2D ----
        GameObject camara = new GameObject("Main Camera");
        camara.tag = "MainCamera";
        Camera cam = camara.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.25f, 0.45f);   // azul océano
        camara.transform.position = new Vector3(0, 0, -10);

        // ---- GameManager (vive durante toda la partida) ----
        new GameObject("GameManager").AddComponent<GameManager>();

        // ---- EventSystem compatible con el Input System nuevo ----
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();

        // ---- Canvas ----
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();
        MenuPrincipal menu = canvasGO.AddComponent<MenuPrincipal>();

        // ---- Título y botones ----
        CrearTexto(canvasGO.transform, "Titulo", "Océano Verde", 110, new Vector2(0, 300), new Vector2(1200, 180));

        Button jugar = CrearBoton(canvasGO.transform, "BotonJugar", "Jugar", new Vector2(0, 60));
        UnityEventTools.AddPersistentListener(jugar.onClick, menu.Jugar);

        Button creditos = CrearBoton(canvasGO.transform, "BotonCreditos", "Créditos", new Vector2(0, -80));
        UnityEventTools.AddBoolPersistentListener(creditos.onClick, menu.MostrarCreditos, true);

        Button salir = CrearBoton(canvasGO.transform, "BotonSalir", "Salir", new Vector2(0, -220));
        UnityEventTools.AddPersistentListener(salir.onClick, menu.Salir);

        // ---- Panel de créditos (empieza oculto) ----
        GameObject panel = new GameObject("PanelCreditos", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform rtPanel = panel.GetComponent<RectTransform>();
        rtPanel.anchorMin = Vector2.zero;
        rtPanel.anchorMax = Vector2.one;
        rtPanel.offsetMin = Vector2.zero;
        rtPanel.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);

        CrearTexto(panel.transform, "TextoCreditos", "Créditos\n\nJuego hecho por el equipo de Océano Verde\n2º DAM", 50, new Vector2(0, 80), new Vector2(1400, 500));
        Button volver = CrearBoton(panel.transform, "BotonVolver", "Volver", new Vector2(0, -300));
        UnityEventTools.AddBoolPersistentListener(volver.onClick, menu.MostrarCreditos, false);
        menu.panelCreditos = panel;

        // ---- Guardar escena y ponerla en Build Settings ----
        EditorSceneManager.SaveScene(escena, RUTA_MENU);
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(RUTA_MENU, true),
            new EditorBuildSettingsScene(RUTA_NIVEL1, true)
        };
        AssetDatabase.SaveAssets();
        Debug.Log("Escena del menú creada en " + RUTA_MENU);
    }

    // Crea un texto centrado (UI Text clásico, no necesita importar nada)
    static Text CrearTexto(Transform padre, string nombre, string contenido, int tamano, Vector2 posicion, Vector2 medida)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = posicion;
        rt.sizeDelta = medida;

        Text texto = go.GetComponent<Text>();
        texto.text = contenido;
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = tamano;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;
        return texto;
    }

    // Crea un botón con fondo y un texto hijo
    static Button CrearBoton(Transform padre, string nombre, string etiqueta, Vector2 posicion)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = posicion;
        rt.sizeDelta = new Vector2(500, 110);
        go.GetComponent<Image>().color = new Color(0.1f, 0.6f, 0.35f);   // verde

        Text texto = CrearTexto(go.transform, "Texto", etiqueta, 60, Vector2.zero, new Vector2(500, 110));
        texto.raycastTarget = false;
        return go.GetComponent<Button>();
    }
}
#endif
