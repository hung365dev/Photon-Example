using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu()]
public class Map : ScriptableObject
{


    [SerializeField] Object sceneObject;    //Object because a Scene isnt Serialized in Inspector
    [SerializeField] Sprite mapImage;
    [SerializeField] string mapName;        //Name for GUI
    [SerializeField, TextArea(5, 10)] string mapDescription;
    [SerializeField] bool isMenuScene = false;
    [SerializeField] int m_sceneIndex;

    public Sprite MapImage { get { return mapImage; } }
    public Object Scene { get { return sceneObject; } }
    public string SceneName { get { return sceneObject.name; } }
    public string GetMapName { get { return mapName; } }
    public string GetMapDescription { get { return mapDescription; } }
    public bool IsMenuScene { get { return isMenuScene; } }

    public int SceneIndex { get { return m_sceneIndex; } set { m_sceneIndex = value; } }

    [System.Serializable]
    public class AllowedGamemode
    {
        [HideInInspector] public string name;
        public bool allow;
        public Mode mode;
    }

    //add each new GameMode
    //so it will be filter in the Menu Map Selection
    public List<AllowedGamemode> allowedGamemode = new List<AllowedGamemode>()
    {
        new AllowedGamemode{name = Mode.DeatchMatch.ToString(),allow = false, mode  = Mode.DeatchMatch},
        new AllowedGamemode{name = Mode.TeamDeathMatch.ToString(),allow = false, mode  = Mode.TeamDeathMatch},
    };


}
