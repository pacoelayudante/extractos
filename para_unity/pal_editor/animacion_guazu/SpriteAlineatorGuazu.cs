using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;

public class SpriteAlineatorGuazu : EditorWindow
{
    AnimationClip[] clips;
    List<Sprite> sprites;
    bool usaGrisClaro;
    Color grisOscuro = new Color(.2f, .2f, .2f, 1);
    Color grisClaro = new Color(.8f, .8f, .8f, 1);
    public void CargarAnimaciones(AnimationClip[] cargar)
    {
        clips = cargar;
        sprites = new List<Sprite>();
        foreach(AnimationClip c in clips)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(c, EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
        }
    }

    void OnGUI()
    {
        if (clips == null)
        {
            this.Close();
            return;
        }
        foreach(AnimationClip clip in clips)
        {
            EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);
        }
        Rect r = EditorGUILayout.GetControlRect(GUILayout.MaxHeight(position.height));
        EditorGUI.DrawRect(r, usaGrisClaro?grisClaro:grisOscuro);
        //GUI.DrawTextureWithTexCoords(r, null, Rect.MinMaxRect(0,0,0,0));
    }
}
