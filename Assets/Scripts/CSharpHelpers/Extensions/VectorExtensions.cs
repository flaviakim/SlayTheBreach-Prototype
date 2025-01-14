using UnityEngine;

/// <summary>
/// Credit to Freya Holmér for this code snippet: https://youtu.be/LSNQuFEDOyQ?si=Pgk1eusfeqI47o9M&t=2982
/// </summary>
public static class VectorExtensions {
    public static Vector2 ExpDecay(this Vector2 a, Vector2 b, float decay, float deltaTime) {
        return b + (a - b) * Mathf.Exp(-decay * deltaTime);
    }

    public static Vector3 ExpDecay(this Vector3 a, Vector3 b, float decay, float deltaTime) {
        return b + (a - b) * Mathf.Exp(-decay * deltaTime);
    }
}