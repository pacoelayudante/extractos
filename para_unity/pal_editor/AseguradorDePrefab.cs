using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AseguradorDePrefab : ScriptableObject
{
    public bool activo = true;
    [SerializeField]
    public GameObject[] prefabs;
}

public class AseguradorDePrefabEditor : EditorWindow
{
    static readonly string defaultName = "prefabsnecesarios";
    static readonly string defaultPath = "Assets/"+defaultName+ ".asset";
    static bool ejecutado;

    AseguradorDePrefab lista;
    Editor editor;
    
    void OnGUI()
    {
        if (lista && editor)
        {
            editor.DrawDefaultInspector();
        }
        else
        {
            lista = CargarLista(true);
        }
    }
    void OnEnable()
    {
        lista = CargarLista(true);
        editor = Editor.CreateEditor(lista);
    }

    [MenuItem("Guazu/Prefabs Necesarios")]
    public static void Mostrar()
    {
        GetWindow<AseguradorDePrefabEditor>();
    }
    [InitializeOnLoadMethod]
    static void CargaListener()
    {
        EditorApplication.playmodeStateChanged += StateChangedCallback;
    }
    static AseguradorDePrefab CargarLista(bool generar = false)
    {
        AseguradorDePrefab cargada = AssetDatabase.LoadAssetAtPath<AseguradorDePrefab>(defaultPath);
        if (!cargada) {
            string[] encontrados = AssetDatabase.FindAssets(defaultName);
            if (encontrados.Length > 0) cargada = AssetDatabase.LoadAssetAtPath<AseguradorDePrefab>( AssetDatabase.GUIDToAssetPath( encontrados[0] ) );
        }
        if (!cargada)
        {
            Debug.Log(defaultName + " no encontrado");
            if (generar)
            {
                Debug.Log("Generando asset");
                cargada = ScriptableObject.CreateInstance<AseguradorDePrefab>();
                AssetDatabase.GenerateUniqueAssetPath(defaultPath);
                AssetDatabase.CreateAsset(cargada, defaultPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        return cargada;
    }
    public static void StateChangedCallback()
    {
        if (EditorApplication.isPlaying && !ejecutado)
        {
            ejecutado = true;
            AseguradorDePrefab lista = CargarLista();
            if (lista)
            {
                if (lista.activo)
                {
                    List<Object> prefs = new List<Object>(lista.prefabs);
                    GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
                    foreach (GameObject go in todos)
                    {
                        if (PrefabUtility.GetPrefabType(go) == PrefabType.None) continue;
                        Object suPrefab = PrefabUtility.GetPrefabParent(go);
                        if (prefs.Contains(suPrefab)) prefs.Remove(suPrefab);
                        if (prefs.Count == 0) break;
                    }
                    foreach (Object o in prefs)
                    {
                        if (o) PrefabUtility.DisconnectPrefabInstance(PrefabUtility.InstantiatePrefab(o));
                    }
                }
            }
        }
        else if (!EditorApplication.isPlaying && ejecutado)
        {
            ejecutado = false;
        }
    }
}
