using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

[Plugin("ChroMapper CustomBongo")]
public class Plugin
{
    private GameObject _customBongo;
    
    [Init]
    private void Init()
    {
        Debug.Log("Init");
        SceneManager.sceneLoaded += SceneLoaded;
        
        var button = new ExtensionButton
        {
            Tooltip = "Reload Bongo Cat",
            Click = ReloadBongo,
            Icon = CreateIcon()
        };
        ExtensionButtons.AddButton(button);
    }

    [Exit]
    private void Exit()
    {

    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 3)
        {
            _customBongo = new GameObject("CustomBongo", typeof(CustomBongo));
        }
    }

    private void ReloadBongo()
    {
        if (_customBongo != null)
        {
            _customBongo.GetComponent<CustomBongo>().Start();
        }
    }

    private static Sprite CreateIcon()
    {
        var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChroMapper-CustomBongo.refresh-icon.png")!;
        var iconData = new byte[iconStream.Length];
        iconStream.Read(iconData, 0, (int)iconStream.Length);
        
        var texture = new Texture2D(16, 16);
        texture.LoadImage(iconData);

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
    }
}