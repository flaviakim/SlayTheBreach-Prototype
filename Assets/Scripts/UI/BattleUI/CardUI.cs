using System;
using UnityEngine;

public class CardUI : MonoBehaviour, IMouseHoverTarget, IMouseDragTarget {
    private const float CARD_MOVEMENT_SPEED = 10f;

    [SerializeField] private TMPro.TextMeshPro cardNameText;
    [SerializeField] private TMPro.TextMeshPro cardDescriptionText;

    [field: SerializeField] public float Width { get; private set; }
    [field: SerializeField] public float Height { get; private set; }

    public Vector2 TargetPosition { get; private set; }
    private Vector2? _targetPositionAfterDrag = null;

    public Card Card { get; private set; }

    private void Update() {
        // Don't use Lerp, it doesn't work with continuous updates. This moves the card smoothly:
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, TargetPosition, CARD_MOVEMENT_SPEED * Time.deltaTime);
    }

    public void Initialize(Card card) {
        Card = card;
        cardNameText.text = card.CardName;
        cardDescriptionText.text = card.Description;
    }

    public void SetTargetPosition(Vector2 position) {
        if (!_isDragging) {
            TargetPosition = position;
        }
        else {
            _targetPositionAfterDrag = position;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.localPosition, new Vector3(Width, Height, 0));
    }


    public void OnMouseHoverEnter() {
        // Debug.Log($"Mouse enter {gameObject.name}");
    }
    public void OnMouseHoverExit() {
        // Debug.Log($"Mouse exit {gameObject.name}");
    }

    private bool _isDragging = false;

    public void OnMouseDragStart() {
        Debug.Log($"Mouse drag start {gameObject.name}");
        Debug.Assert(!_isDragging, "OnMouseDragStart called while already dragging");
        TargetPosition = transform.localPosition;
        _isDragging = true;
    }
    public void OnMouseDragContinuous(Vector2 delta) {
        Debug.Assert(_isDragging, "OnMouseDragContinuous called without OnMouseDragStart");
        TargetPosition += delta;
    }
    public void OnMouseDragEnd() {
        Debug.Assert(_isDragging, "OnMouseDragEnd called without OnMouseDragStart");
        Debug.Log($"Mouse drag end {gameObject.name}");
        _isDragging = false;
        if (_targetPositionAfterDrag.HasValue) {
            TargetPosition = _targetPositionAfterDrag.Value;
            _targetPositionAfterDrag = null;
        }
    }
}