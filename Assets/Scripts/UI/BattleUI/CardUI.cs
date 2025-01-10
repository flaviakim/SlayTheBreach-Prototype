using System;
using UnityEngine;

public class CardUI : MonoBehaviour, IMouseHoverTarget, IMouseDragTarget {

    [SerializeField] private TMPro.TextMeshPro cardNameText;
    [SerializeField] private TMPro.TextMeshPro cardDescriptionText;

    [field: SerializeField] public float Width { get; private set; }
    [field: SerializeField] public float Height { get; private set; }

    public Card Card { get; private set; }
    public int HandIndex { get; private set; }

    public void Initialize(Card card, int handIndex) {
        Card = card;
        HandIndex = handIndex;
        cardNameText.text = card.CardName;
        cardDescriptionText.text = card.Description;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }


    public void OnMouseHoverEnter() {
        // Debug.Log($"Mouse enter {gameObject.name}");
    }
    public void OnMouseHoverExit() {
        // Debug.Log($"Mouse exit {gameObject.name}");
    }

    public void OnMouseDragStart() {
        Debug.Log($"Mouse drag start {gameObject.name}");
    }
    public void OnMouseDragContinuous(Vector2 delta) {
        transform.position += (Vector3)delta;
    }
    public void OnMouseDragEnd() {
        Debug.Log($"Mouse drag end {gameObject.name}");
        Debug.Assert(Battle.CurrentBattle != null, "Battle.CurrentBattle != null");
        var mouseWorldPos = CameraController.Instance.GetMouseWorldPosition();
        Battle.CurrentBattle.TryPlayCard(HandIndex, mouseWorldPos);
    }
}