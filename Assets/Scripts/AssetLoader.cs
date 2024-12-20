using System.IO;
using UnityEngine;

public static class AssetLoader {

    public static Sprite LoadSprite(string spritePath) {
        const string spriteBasePath = "Sprites";
        var spriteFullPath = Path.Combine(Application.streamingAssetsPath, spriteBasePath, spritePath);
        if (!File.Exists(spriteFullPath)) {
            Debug.LogError($"Sprite file '{spriteFullPath}' does not exist.");
            return null!;
        }

        var bytes = File.ReadAllBytes(spriteFullPath);
        var texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        const int pixelsPerUnit = 32;
        var pivot = new Vector2(0.5f, 0.5f);
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