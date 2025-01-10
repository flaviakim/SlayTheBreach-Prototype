using System;
using UnityEngine;

public class MapTileVisual : MonoBehaviour, IMouseHoverTarget {
    public void OnMouseHoverEnter() {
        Debug.Log($"Mouse enter {gameObject.name}");
    }
    public void OnMouseHoverExit() {
        Debug.Log($"Mouse exit {gameObject.name}");
    }
}