#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Crea automáticamente el Animator Controller del jugador:
///  Parámetros: Speed (float), IsGrounded (bool), IsJumping (bool), IsSwimming (bool)
///  Estados:    Idle, Run, Jump, Swim
/// DEBE estar dentro de una carpeta llamada "Editor" (Assets/Editor).
///
/// Uso: (opcional) selecciona en el Project tus AnimationClips y ve a
/// menú Tools > Crear Animator Controller del Jugador.
/// Los clips se asignan solos si su nombre contiene: idle/quieto/parado,
/// run/walk/andar/correr, jump/salt, swim/nad.
/// </summary>
public static class CreatePlayerAnimator
{
    [MenuItem("Tools/Crear Animator Controller del Jugador")]
    public static void Create()
    {
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/PlayerAnimator.controller");
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        // ---- Parámetros ----
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsSwimming", AnimatorControllerParameterType.Bool);

        // ---- Estados ----
        AnimatorStateMachine sm = controller.layers[0].stateMachine;

        AnimatorState idle = sm.AddState("Idle", new Vector3(300, 200, 0));
        AnimatorState run  = sm.AddState("Run",  new Vector3(300, 40, 0));
        AnimatorState jump = sm.AddState("Jump", new Vector3(560, 120, 0));
        AnimatorState swim = sm.AddState("Swim", new Vector3(300, 360, 0));
        sm.defaultState = idle;

        // ---- Asignar clips seleccionados (si hay) ----
        foreach (AnimationClip clip in Selection.GetFiltered<AnimationClip>(SelectionMode.Assets))
        {
            string n = clip.name.ToLower();
            if (n.Contains("idle") || n.Contains("quiet") || n.Contains("parad")) idle.motion = clip;
            else if (n.Contains("run") || n.Contains("walk") || n.Contains("anda") || n.Contains("corre")) run.motion = clip;
            else if (n.Contains("jump") || n.Contains("salt")) jump.motion = clip;
            else if (n.Contains("swim") || n.Contains("nad")) swim.motion = clip;
        }

        // ---- Transiciones ----
        // Idle <-> Run según Speed
        AnimatorStateTransition t;

        t = AddTransition(idle, run);
        t.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
        t.AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");

        t = AddTransition(run, idle);
        t.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");

        // Idle/Run -> Jump
        t = AddTransition(idle, jump);
        t.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");

        t = AddTransition(run, jump);
        t.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");

        // Jump -> Idle
        t = AddTransition(jump, idle);
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");

        // Any State -> Swim (desde cualquier estado al entrar en el agua)
        t = sm.AddAnyStateTransition(swim);
        ConfigureTransition(t);
        t.canTransitionToSelf = false;
        t.AddCondition(AnimatorConditionMode.If, 0, "IsSwimming");

        // Swim -> Idle al salir del agua
        t = AddTransition(swim, idle);
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "IsSwimming");

        AssetDatabase.SaveAssets();
        Selection.activeObject = controller;
        EditorGUIUtility.PingObject(controller);
        Debug.Log("Animator Controller creado en: " + path);
    }

    private static AnimatorStateTransition AddTransition(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition t = from.AddTransition(to);
        ConfigureTransition(t);
        return t;
    }

    // Sin Has Exit Time y con transición muy corta para que responda al instante
    private static void ConfigureTransition(AnimatorStateTransition t)
    {
        t.hasExitTime = false;
        t.hasFixedDuration = true;
        t.duration = 0.05f;
    }
}
#endif
