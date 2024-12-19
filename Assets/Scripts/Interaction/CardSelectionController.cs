using System;
using UnityEngine;

public class CardSelectionController : MonoBehaviour {

    private int? _selectedCardIndex = null;

    private void Update() {
        // check number keys
        for (var i = 0; i < 10; i++) {
            if (Input.GetKeyUp(i.ToString())) {
                _selectedCardIndex = i - 1;
            }
        }

        if (Input.GetMouseButtonUp(0) && _selectedCardIndex.HasValue) {
            var mouseWorldPosition = CameraController.Instance.GetMouseWorldPosition();
            if (Battle.CurrentBattle.BattleMap.TryGetTile(mouseWorldPosition, out var clickedOnTile) && clickedOnTile.Occupant != null && clickedOnTile.Occupant.IsPlayerControlled) {
                Debug.Log($"Playing card {_selectedCardIndex} on {clickedOnTile.Occupant}");
                var success = Battle.CurrentBattle.PlayCard(_selectedCardIndex.Value, clickedOnTile.Occupant);
                if (success) {
                    _selectedCardIndex = null;
                } else {
                    Debug.LogWarning("Failed to play card");
                }
            }
        }
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(Screen.width - 210, 10, 200, 200));
        GUILayout.BeginVertical();
        GUILayout.Label("Select a card to play:");
        var handCards = Battle.CurrentBattle.CardsManager.PlayerDeck.HandCards;
        for (var i = 0; i < handCards.Count; i++) {
            var card = handCards[i];
            GUILayout.Label($"{i + 1}: {card.CardName}{(i == _selectedCardIndex ? " (<-)" : "")}");
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

    }
}