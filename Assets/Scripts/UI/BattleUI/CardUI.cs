using System;
using UnityEngine;

public class CardUI : MonoBehaviour {

    [SerializeField] private TMPro.TextMeshPro cardNameText;
    [SerializeField] private TMPro.TextMeshPro cardDescriptionText;

    [field: SerializeField] public float Width { get; private set; }
    [field: SerializeField] public float Height { get; private set; }

    public Card Card { get; private set; }

    public void Initialize(Card card) {
        Card = card;
        cardNameText.text = card.CardName;
        cardDescriptionText.text = card.Description;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }
}