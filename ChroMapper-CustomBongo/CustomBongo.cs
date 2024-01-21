using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

using SimpleJSON;

public class CustomBongo : MonoBehaviour
{
    private const string BothFile = "both.png";
    private const string LeftFile = "left.png";
    private const string RightFile = "right.png";
    private const string NoneFile = "none.png";
    private const string ConfigFile = "config.json";

    public void Start()
    {
        var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyFolder == null)
        {
            Debug.LogWarning("Could not determine plugin location");
            return;
        }

        if (!BongoImagesExist(assemblyFolder))
        {
            Debug.LogWarning("Not all bongo images are present");
            return;
        }
        
        var config = GetConfig(assemblyFolder);

        SetCustomBongo(assemblyFolder, config);
    }

    private static Config GetConfig(string assemblyFolder)
    {
        var configPath = Path.Combine(assemblyFolder, ConfigFile);
        if (!File.Exists(configPath))
        {
            using (var streamWriter = new StreamWriter(configPath, append: false))
                streamWriter.Write(new JSONObject
                {
                    ["ScaleX"] = 1,
                    ["ScaleY"] = 1,
                    ["OffsetX"] = 0,
                    ["OffsetY"] = 0
                }.ToString());
        }

        using var streamReader = new StreamReader(configPath);
        var configJson = JSON.Parse(streamReader.ReadToEnd());
        var config = new Config
        {
            ScaleX = configJson["ScaleX"].AsFloat,
            ScaleY = configJson["ScaleY"].AsFloat,
            OffsetX = configJson["OffsetX"].AsFloat,
            OffsetY = configJson["OffsetY"].AsFloat
        };
        return config;
    }

    private bool BongoImagesExist(string assemblyFolder)
    {
        var bothExists = File.Exists(Path.Combine(assemblyFolder, BothFile));
        var leftExists = File.Exists(Path.Combine(assemblyFolder, LeftFile));
        var rightExists = File.Exists(Path.Combine(assemblyFolder, RightFile));
        var noneExists = File.Exists(Path.Combine(assemblyFolder, NoneFile));

        var allExists  = bothExists && leftExists && rightExists && noneExists;

        if (!allExists)
        {
            var message = new StringBuilder("Missing custom bongo cat images:");
            if (!bothExists) message.Append($"\n  {BothFile}");
            if (!leftExists) message.Append($"\n  {LeftFile}");
            if (!rightExists) message.Append($"\n  {RightFile}");
            if (!noneExists) message.Append($"\n  {NoneFile}");
            PersistentUI.Instance.ShowDialogBox(message.ToString(), null, PersistentUI.DialogBoxPresetType.Ok);
        }

        return allExists;
    }

    private void SetCustomBongo(string assemblyFolder, Config config)
    {
        var bongoCat = FindObjectOfType<BongoCat>();

        var customBongo = ScriptableObject.CreateInstance<BongoCatPreset>();
        customBongo.LeftDownRightDown = CreateSpriteFromPath(Path.Combine(assemblyFolder, BothFile));
        customBongo.LeftDownRightUp = CreateSpriteFromPath(Path.Combine(assemblyFolder, LeftFile));
        customBongo.LeftUpRightDown = CreateSpriteFromPath(Path.Combine(assemblyFolder, RightFile));
        customBongo.LeftUpRightUp = CreateSpriteFromPath(Path.Combine(assemblyFolder, NoneFile));
        customBongo.Scale = new Vector2(config.ScaleX, config.ScaleY);
        customBongo.YOffset = config.OffsetY;

        var bongoCats = bongoCat.GetType().GetField("bongoCats", BindingFlags.NonPublic | BindingFlags.Instance);
        var edit = (BongoCatPreset[])bongoCats.GetValue(bongoCat);
        edit[0] = customBongo;
        bongoCats.SetValue(bongoCat, edit);

        var updateBongoCat = bongoCat.GetType()
            .GetMethod("UpdateBongoCatState", BindingFlags.NonPublic | BindingFlags.Instance);
        updateBongoCat.Invoke(bongoCat, new object[] { true });

        var bongoCatTransform = bongoCat.transform;
        var localPosition = bongoCatTransform.localPosition;
        localPosition = new Vector3(
            config.OffsetX,
            localPosition.y,
            localPosition.z);
        bongoCatTransform.localPosition = localPosition;
    }

    private Sprite CreateSpriteFromPath(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(128, 128);
        texture.LoadImage(bytes);

        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        return sprite;
    }

    private class Config
    {
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
    }
}