using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager
{

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public T LoadResources<T>(string path) where T : Object
    {
        Object obj = Resources.Load(path);
        if (obj == null)
        {
            return null;
        }
        return (T)obj;
    }

}
