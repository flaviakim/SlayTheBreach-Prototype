using System;
using UnityEngine;

public class CardUI : MonoBehaviour, IMouseHoverTarget, IMouseDragTarget {
    private const float CARD_MOVEMENT_TIME = 0.5f;
    private const float MAX_CARD_MOVEMENT_SPEED = Mathf.Infinity;

    [SerializeField] private TMPro.TextMeshPro cardNameText;
    [SerializeField] private TMPro.TextMeshPro cardDescriptionText;

    [field: SerializeField] public float Width { get; private set; }
    [field: SerializeField] public float Height { get; private set; }

    public Vector2 TargetPosition { get; private set; }

    private Vector2? _targetPositionAfterDrag = null;
    private bool _isDragging = false;

    private Vector2 _currentVelocity = Vector2.zero;

    public Card Card { get; private set; }

    private const int DECAY_DRAG = 16;
    private const int DECAY_BACK_INTO_HAND = 8;


    private void Update() {
        // Don't use Lerp, it doesn't work with continuous updates. This moves the card linearly:
        // transform.localPosition = Vector2.MoveTowards(transform.localPosition, TargetPosition, CARD_MOVEMENT_SPEED * Time.deltaTime);
        // Use SmoothDamp for smooth movement:
        // transform.localPosition = Vector2.SmoothDamp(transform.localPosition, TargetPosition, ref _currentVelocity, CARD_MOVEMENT_TIME, MAX_CARD_MOVEMENT_SPEED);
        // Use Exponential Decay for smooth movement:
        transform.localPosition = transform.localPosition.ExpDecay(TargetPosition, _isDragging ? DECAY_DRAG : DECAY_BACK_INTO_HAND, Time.deltaTime);
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