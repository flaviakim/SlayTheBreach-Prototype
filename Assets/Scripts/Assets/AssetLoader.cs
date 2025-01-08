using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class AssetLoader {

    public static string AssetPath => Application.streamingAssetsPath;

    public static List<T> LoadAllJson<T>(string directoryPath) {
        var jsonFullPath = Path.Combine(AssetPath, directoryPath);
        if (!Directory.Exists(jsonFullPath)) {
            Debug.LogError($"Json directory '{jsonFullPath}' does not exist.");
            return null!;
        }

        var files = Directory.GetFiles(jsonFullPath, "*.json");
        Debug.Log($"Found {files.Length} json files in {jsonFullPath}");
        var jsonList = new List<T>();
        foreach (var jsonFile in files) {
            var jsonItem = LoadJson<T>(jsonFile);
            Debug.Assert(jsonItem != null, "Failed to load json item");
            jsonList.Add(jsonItem);
        }
        return jsonList;
    }

    public static T LoadJson<T>(string jsonPath) {
        var jsonFullPath = Path.Combine(Application.streamingAssetsPath, jsonPath);
        if (!File.Exists(jsonFullPath)) {
            Debug.LogError($"Json file '{jsonFullPath}' does not exist.");
            return default!;
        }

        var json = File.ReadAllText(jsonFullPath);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static Sprite LoadSprite(string spritePath) { // TODO: store sprites in a dictionary to avoid loading the same sprite multiple times
        const string spriteBasePath = "Sprites";
        if (string.IsNullOrEmpty(spritePath)) {
            Debug.LogError("Sprite path is null or empty.");
            return null!;
        }
        var spriteFullPath = Path.Combine(Application.streamingAssetsPath, spriteBasePath, spritePath);
        if (!File.Exists(spriteFullPath)) {
            Debug.LogError($"Sprite file '{spriteFullPath}' does not exist.");
            return null!;
        }

        var bytes = File.ReadAllBytes(spriteFullPath);
        var texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        const int pixelsPerUnit = 32;
        var pivot = new Vector2(0, 0);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
        sprite.name = Path.GetFileNameWithoutExtension(spritePath);
        sprite.texture.filterMode = FilterMode.Point;
        sprite.texture.wrapMode = TextureWrapMode.Clamp;
        sprite.texture.Apply();
        return sprite;
    }

    public static string LoadText(string textPath) {
        var textFullPath = Path.Combine(Application.streamingAssetsPath, textPath);
        if (!File.Exists(textFullPath)) {
            Debug.LogError($"Text file '{textFullPath}' does not exist.");
            return null!;
        }

        return File.ReadAllText(textFullPath);
    }

}