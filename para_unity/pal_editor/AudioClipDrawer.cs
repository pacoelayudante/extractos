using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(AudioClip))]
public class AudioClipDrawer : PropertyDrawer {
    static AudioSource a;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = property.objectReferenceValue != null;
        position = EditorGUI.IndentedRect(position);
        if (GUI.Button(new Rect(position.x,position.y,EditorGUIUtility.singleLineHeight * 1.3f, position.height), "►"))
        {
            if (a == null)
            {
                GameObject o = EditorUtility.CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave);
                a = o.AddComponent<AudioSource>();
                a.spatialBlend = 0;
                EditorApplication.update += OnEditorUpdate;
            }
            a.clip = (AudioClip)property.objectReferenceValue;
            a.Play();
        }
        GUI.enabled = true;
        position.x += EditorGUIUtility.singleLineHeight*1.3f;
        position.width -= EditorGUIUtility.singleLineHeight*1.3f;
        EditorGUI.PropertyField(position, property);
    }

    void OnEditorUpdate()
    {
        if (a != null)
        {
            if (!a.isPlaying)
            {
                GameObject.DestroyImmediate(a.gameObject);
                EditorApplication.update -= OnEditorUpdate;
            }
        }
        else
        {
            EditorApplication.update -= OnEditorUpdate;
        }
    }
}
