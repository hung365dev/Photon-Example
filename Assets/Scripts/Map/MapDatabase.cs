using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Pun;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;


[CreateAssetMenu(menuName = ("MapDatabase"), fileName = ("MapDatabase"))]
public class MapDatabase : ScriptableObject
{

    [SerializeField] List<Map> maps = new List<Map>();
    public List<Map> Maps { get { return maps; } }
    public int Count { get { return maps.Count; } }

    public static void LoadMap(Map map)
    {
        LoadingScreen.LoadScene(map.SceneIndex);
    }
    void OnValidate()
    {
        //only executed in Unity Editor
#if UNITY_EDITOR
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes; //Store Scene from BuildSettings to get Path/Name
        Regex regex = new Regex(@"([^/]*/)*([\w\d\-]*)\.unity");        //reduce to Name regulation

        for (var i = 0; i < scenes.Length; i++)                         //Loop through stored Scenes
        {                                                               //Get the Name from Scene
            string name = regex.Replace(scenes[i].path, "$2");
            Map t = GetMapWithSceneName(name);                          //Return Map with same stored Scene
            t.SceneIndex = i;                                           //Map stores now the scene Index from BuildSettings
        }
#endif
    }
    /// <summary>
    /// Add a new Map to the Database.
    /// </summary>
    /// <param name="map">new Map.</param>
    public void AddMap(Map map)
    {
        Maps.Add(map);
    }

    public Map GetMapWithSceneName(string name)
    {
        Map map = Maps.Find(x => x.SceneName == name);
        if (map == null)
        {
            Debug.LogWarning(name + " Map not found.");
            return null;
        }
        return map;
    }
    public Map GetMapWithMapName(string name)
    {
        Map map = Maps.Find(x => x.GetMapName == name);
        if (map == null)
        {
            Debug.LogWarning(name + " Map not found.");
            return null;
        }
        return map;
    }
    /// <summary>Return's a List with all Menu Scene names.</summary>
    public List<string> GetMenuSceneNames()
    {
        List<string> menuScenes = new List<string>();

        for (int i = 0; i < Maps.Count; i++)
        {
            Map map = Maps[i];
            if (Maps[i].IsMenuScene)
            {
                menuScenes.Add(Maps[i].SceneName);
            }
        }
        return menuScenes;
    }
    /// <summary>Return's a List with all Map names.</summary>
    public List<string> GetMapNames()
    {
        List<string> mapNames = new List<string>();

        for (int i = 0; i < Maps.Count; i++)
        {
            Map map = Maps[i];
            if (!Maps[i].IsMenuScene)
            {
                mapNames.Add(Maps[i].GetMapName);
            }
        }
        return mapNames;
    }
    /// <summary>Return's a List with all Map SceneAsset names.</summary>
    public List<string> GetMapSceneNames()
    {
        List<string> mapNames = new List<string>();

        for (int i = 0; i < Maps.Count; i++)
        {
            Map map = Maps[i];
            if (!Maps[i].IsMenuScene)
            {
                mapNames.Add(Maps[i].SceneName);
            }
        }
        return mapNames;
    }
}

