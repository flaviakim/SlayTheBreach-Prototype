using System;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get; private set; } = null!;

    private Camera _camera = null!;

    private void Awake() {
        if (Instance != null) {
            throw new Exception("CameraController already initialized");
        }
        Instance = this;
        _camera = GetComponent<Camera>();
    }

    private void Start() {
        var map = Battle.CurrentBattle.BattleMap;
        if (map == null) {
            throw new Exception("BattleMap not found");
        }

        var mapWidth = map.Width;
        var mapHeight = map.Height;

        transform.position = new Vector3(mapWidth / 2f, mapHeight / 2f, -10);
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPosition) {
        return _camera.ScreenToWorldPoint(screenPosition);
    }

    public Vector2 GetMouseWorldPosition() {
        var mouseScreenPosition = Input.mousePosition;
        var mouseWorldPosition = ScreenToWorldPoint(mouseScreenPosition);
        return new Vector2(mouseWorldPosition.x, mouseWorldPosition.y);
    }
}