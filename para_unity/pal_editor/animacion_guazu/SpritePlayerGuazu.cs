using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SpritePlayerGuazu : EditorWindow
{
    AnimationClip clipSeleccionado;
    ObjectReferenceKeyframe[] keyframes;
    List<Sprite> seleccionados;

    double ultimoTiempo, deltaTiempo;
    float tiempo;

    bool usaGrisClaro;
    float frameRate = 24;
    Color grisOscuro = new Color(.2f, .2f, .2f, 1);
    Color grisClaro = new Color(.8f, .8f, .8f, 1);

    [MenuItem("Guazu/Vista Previa Sprites")]
    public static void Abrir()
    {
        GetWindow<SpritePlayerGuazu>();
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        bool clipMostrable = false, grupoSpritesMostrable = false;
        AnimationClip clipPrevio = clipSeleccionado;
        List<Sprite> selPrevios = seleccionados;

        if (Selection.activeObject != null)
        {
            if (Selection.activeObject.GetType() == typeof(AnimationClip))
            {
                clipMostrable = CargarClip((AnimationClip)Selection.activeObject);
            }
        }

        if (!clipMostrable)
        {
            seleccionados = new List<Sprite>();
            string[] listaSeleccionSinFiltro = Selection.assetGUIDs;
            for (int i = 0; i < listaSeleccionSinFiltro.Length; i++) listaSeleccionSinFiltro[i] = AssetDatabase.GUIDToAssetPath(listaSeleccionSinFiltro[i]);
            System.Array.Sort(listaSeleccionSinFiltro, new AlphanumComparatorFast());//sam­@dotnetperls.com
            foreach (string path in listaSeleccionSinFiltro)
            {
                foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (o.GetType() == typeof(Sprite)) seleccionados.Add((Sprite)o);
                }
            }
            grupoSpritesMostrable = seleccionados.Count > 0;
        }

        if (!clipMostrable && !grupoSpritesMostrable)
        {
            if (clipPrevio != null) clipMostrable = CargarClip(clipPrevio);
            if (!clipMostrable && selPrevios != null)
            {
                seleccionados = selPrevios;
                grupoSpritesMostrable = seleccionados.Count > 0;
            }
        }

        GUILayout.BeginHorizontal();
        usaGrisClaro = GUILayout.Toggle(usaGrisClaro, usaGrisClaro ? "Fondo Claro" : "Fondo Oscuro");
        GUI.enabled = clipMostrable;
        if (GUILayout.Button("Editar")) SpriteAnimatorGuazu.Editar(clipSeleccionado);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        if (clipMostrable)
        {
            MostrarClip();
        }
        else if (grupoSpritesMostrable)
        {
            clipSeleccionado = null;
            MostrarGrupoSprites();
        }
        else
        {
            GUILayout.Label("Sin nada para mostrar");
        }

    }

    void MostrarClip()
    {
        if (keyframes == null || clipSeleccionado == null) { GUILayout.Label("ERROR 2"); return; }
        else if (keyframes.Length == 0) { GUILayout.Label("ERROR 3"); return; }
        else if (clipSeleccionado.length == 0) { GUILayout.Label("ERROR 4"); return; }

        if (Event.current.type == EventType.ContextClick)
        {
            GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Seleccionar Sprites"), false, SeleccionarKeyframes);
            gm.ShowAsContext();
            Event.current.Use();
        }

        deltaTiempo = EditorApplication.timeSinceStartup - ultimoTiempo;
        ultimoTiempo = EditorApplication.timeSinceStartup;
        tiempo += (float)(deltaTiempo);
        tiempo %= clipSeleccionado.length;
        ObjectReferenceKeyframe kf = keyframes[0];
        for (int i = 0; i < keyframes.Length; i++)
        {
            if (keyframes[i].time > tiempo) break;
            else kf = keyframes[i];
        }
        Sprite sprite = (Sprite)kf.value;

        GUI.enabled = false;
        EditorGUILayout.ObjectField(clipSeleccionado, typeof(AnimationClip), false);
        GUI.enabled = true;
        Rect r = sprite == null ? GUILayoutUtility.GetAspectRect(1) : GUILayoutUtility.GetAspectRect(sprite.rect.width / sprite.rect.height);
        Rect texR = sprite == null ? r : new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height, sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
        EditorGUI.DrawRect(r, usaGrisClaro ? grisClaro : grisOscuro);
        if (sprite != null) GUI.DrawTextureWithTexCoords(r, sprite.texture, texR);
    }

    void MostrarGrupoSprites()
    {
        if (seleccionados == null) { GUILayout.Label("ERROR 0"); return; }
        else if (seleccionados.Count == 0) { GUILayout.Label("ERROR 1"); return; }

        deltaTiempo = EditorApplication.timeSinceStartup - ultimoTiempo;
        ultimoTiempo = EditorApplication.timeSinceStartup;
        tiempo += (float)(deltaTiempo * frameRate);
        tiempo %= seleccionados.Count;
        int frame = Mathf.FloorToInt(tiempo);
        Sprite sprite = seleccionados[frame];

        GUILayout.Label(seleccionados.Count + " sprites seleccionados. Actual : " + frame);
        Rect r = sprite==null? GUILayoutUtility.GetAspectRect(1): GUILayoutUtility.GetAspectRect(sprite.rect.width / sprite.rect.height);
        Rect texR = sprite==null?r:new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height, sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
        EditorGUI.DrawRect(r, usaGrisClaro ? grisClaro : grisOscuro);
        if(sprite!=null)GUI.DrawTextureWithTexCoords(r, sprite.texture, texR);
    }

    bool CargarClip(AnimationClip nuevo)
    {
        clipSeleccionado = nuevo;
        keyframes = AnimationUtility.GetObjectReferenceCurve(clipSeleccionado, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
        if (keyframes == null) return false;
        else return keyframes.Length > 0;
    }
    void SeleccionarKeyframes()
    {
        if (keyframes != null)
        {
            List<Texture> texturas = new List<Texture>();
            foreach (ObjectReferenceKeyframe kf in keyframes)
            {
                Sprite sprite = (Sprite)kf.value;
                if (sprite != null)
                {
                    if (!texturas.Contains(sprite.texture)) texturas.Add(sprite.texture);
                }
            }
            Selection.objects = texturas.ToArray();
        }
    }
}
