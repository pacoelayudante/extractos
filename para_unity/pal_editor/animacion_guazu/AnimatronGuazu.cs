using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class AnimatronGuazu : EditorWindow
{
    static float frameRate = 24;

    List<AnimationClip> clipsMarcados = new List<AnimationClip>();
    bool crearNuevoAnimator, crearNuevoClip, listaClipsAbierta;
    string crearNuevoAnimatorNombre,crearNuevoClipNombre;
    Vector2 scroll;
    AnimatorController animatorActivo;
    SerializedObject animatorActivoSerial;

    [MenuItem("Guazu/Animatron")]
    public static void AbrirVentana()
    {
        GetWindow<AnimatronGuazu>();
    }

    void OnEnable()
    {
    }
    void OnDestroy()
    {
    }
    void OnLostFocus()
    {
        crearNuevoClip = crearNuevoAnimator = false;
    }

    void OnGUI()
    {
        GUILayout.Label("ANIMATOR : ");
        CrearNuevoAnimator();
        if (animatorActivo == null) animatorActivoSerial = null;
        EditorGUI.BeginChangeCheck();
        animatorActivo = (AnimatorController)EditorGUILayout.ObjectField(animatorActivo, typeof(AnimatorController), false);
        if (EditorGUI.EndChangeCheck())
        {
            if (animatorActivo == null) animatorActivoSerial = null;
            else
            {
                animatorActivoSerial = new SerializedObject(animatorActivo);
            }
        }
        //listaClipsAbierta = EditorGUILayout.Foldout(listaClipsAbierta, "CLIPS : ");
        GUI.enabled = clipsMarcados.Count > 0;
        if (GUILayout.Button("Abrir Alineator")) GetWindow<SpriteAlineatorGuazu>().CargarAnimaciones(clipsMarcados.ToArray());
        GUI.enabled = animatorActivo != null;
        EditorGUILayout.BeginHorizontal();
        listaClipsAbierta = EditorGUILayout.Foldout(listaClipsAbierta, "CLIPS : ") && animatorActivo != null;
        GUI.enabled = listaClipsAbierta;
        if (GUILayout.Button("Todas"))
        {
            clipsMarcados.Clear();
            foreach (AnimationClip clip in animatorActivo.animationClips) { if (!clipsMarcados.Contains(clip)) clipsMarcados.Add(clip); }
        }
        if (GUILayout.Button("Ninguna"))
        {
            clipsMarcados.Clear();
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
        if (listaClipsAbierta && animatorActivo != null)
        {
            ListaClips();
        }
    }

    void ListaClips()
    {
        CrearNuevoClip();
        List<AnimationClip> norepita = new List<AnimationClip>();
        scroll=EditorGUILayout.BeginScrollView(scroll);
        for(int i=animatorActivo.animationClips.Length-1; i>=0; i--)
        //foreach (AnimationClip clip in animatorActivo.animationClips)
        {
            AnimationClip clip = animatorActivo.animationClips[i];
            if (norepita.Contains(clip)) continue;
            else norepita.Add(clip);

            GUILayout.BeginHorizontal();
            bool activo = clipsMarcados.Contains(clip);
            if (EditorGUILayout.Toggle(activo, GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight)) != activo)
            {
                if (activo) clipsMarcados.Remove(clip);
                else clipsMarcados.Add(clip);
            }
            GUI.enabled = false;
            EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    void CrearNuevoClip()
    {
        if (crearNuevoClip)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                CrearNuevoClipEjecutar(crearNuevoClipNombre);
                crearNuevoClip = false;
                Repaint();
            }
            else if (Event.current.isMouse || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
            {
                crearNuevoClip = false;
                Repaint();
            }
            GUI.SetNextControlName("TextoCrearNuevo");
            crearNuevoClipNombre = GUILayout.TextField(crearNuevoClipNombre);
            if (!GUI.GetNameOfFocusedControl().Equals("TextoCrearNuevo")) GUI.FocusControl("TextoCrearNuevo");
        }
        else if (GUILayout.Button("Crear Nuevo"))
        {
            crearNuevoClip = true;
            crearNuevoClipNombre = "";
        }
    }
    void CrearNuevoClipEjecutar(string nombre)
    {
        if (nombre == null) return;
        else if (nombre.Length == 0) return;
        string[] nombresUsados = AssetDatabase.FindAssets(nombre + " t:AnimationClip");
        foreach (string guid in nombresUsados)
        {
            if (System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)).Equals(nombre))
            {
                EditorUtility.DisplayDialog("Repetido", "El nombre \"" + nombre + "\" ya esta utilizado", "¡Pucha!");
                return;
            }
        }

        List<Sprite> seleccionados = new List<Sprite>();
        string[] listaSeleccionSinFiltro = Selection.assetGUIDs;
        for (int i = 0; i < listaSeleccionSinFiltro.Length; i++) listaSeleccionSinFiltro[i] = AssetDatabase.GUIDToAssetPath(listaSeleccionSinFiltro[i]);
        System.Array.Sort(listaSeleccionSinFiltro,new AlphanumComparatorFast());//sam­@dotnetperls.com
        foreach (string path in listaSeleccionSinFiltro)
        {
            foreach (Object o in AssetDatabase.LoadAllAssetsAtPath( path ) )
            {
                if (o.GetType() == typeof(Sprite)) seleccionados.Add((Sprite)o);
            }
        }
        if (seleccionados.Count == 0)
        {
            EditorUtility.DisplayDialog("Sin Seleccion","Tenés que elegir al menos un sprite en la pestaña \"Project\" viste", "¡Claro! Que boludo");
            return;
        }

        string carpeta = System.IO.Path.GetDirectoryName( AssetDatabase.GetAssetOrScenePath(animatorActivo) );

        AnimationClip nuevo = new AnimationClip();
        nuevo.frameRate = frameRate;
        EditorCurveBinding bind = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] kf = new ObjectReferenceKeyframe[seleccionados.Count];
        for (int i=0; i<kf.Length; i++)
        {
            kf[i] = new ObjectReferenceKeyframe();
            kf[i].time = i / frameRate;
            kf[i].value = seleccionados[i];
        }
        AnimationUtility.SetObjectReferenceCurve(nuevo, bind, kf);
        AssetDatabase.CreateAsset(nuevo, carpeta + "/" + nombre + ".anim");
        animatorActivo.AddMotion(nuevo);
        AssetDatabase.SaveAssets();
        //animatorActivo = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animaciones/" + nombre + "/" + nombre + ".controller");
        //animatorActivoSerial = new SerializedObject(animatorActivo);
    }

    void CrearNuevoAnimator()
    {
        if (crearNuevoAnimator)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                CrearNuevoAnimatorEjecutar(crearNuevoAnimatorNombre);
                crearNuevoAnimator = false;
                Repaint();
            }
            else if (Event.current.isMouse || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape))
            {
                crearNuevoAnimator = false;
                Repaint();
            }
            GUI.SetNextControlName("TextoCrearNuevo");
            crearNuevoAnimatorNombre = GUILayout.TextField(crearNuevoAnimatorNombre);
            if (!GUI.GetNameOfFocusedControl().Equals("TextoCrearNuevo")) GUI.FocusControl("TextoCrearNuevo");
        }
        else if (GUILayout.Button("Crear Nuevo"))
        {
            crearNuevoAnimator = true;
            crearNuevoAnimatorNombre = "";
        }
    }
    void CrearNuevoAnimatorEjecutar(string nombre)
    {
        if (nombre == null) return;
        else if (nombre.Length == 0) return;
        string[] nombresUsados = AssetDatabase.FindAssets(nombre + " t:AnimatorController");
        foreach (string guid in nombresUsados)
        {
            if (System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)).Equals(nombre))
            {
                EditorUtility.DisplayDialog("Repetido", "El nombre \"" + nombre + "\" ya esta utilizado", "¡Pucha!");
                return;
            }
        }
        if (!AssetDatabase.IsValidFolder("Assets/Animaciones")) AssetDatabase.CreateFolder("Assets", "Animaciones");
        if (!AssetDatabase.IsValidFolder("Assets/Animaciones/" + nombre)) AssetDatabase.CreateFolder("Assets/Animaciones", nombre);
        animatorActivo = AnimatorController.CreateAnimatorControllerAtPath("Assets/Animaciones/" + nombre + "/" + nombre + ".controller");
        animatorActivoSerial = new SerializedObject(animatorActivo);
    }
}