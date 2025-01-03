using System;
using UnityEngine;

namespace GameLoop {
    public class BattleLoop : MonoBehaviour {
        [SerializeField] private string battleId = "TestBattle";

        private readonly BattleFactory _battleFactory = new BattleFactory();
        private Battle _battle;

        private void Awake() {
            StartBattle(battleId);
        }

        public void StartBattle(string battleId) {
            _battle = _battleFactory.CreateBattleInstance(battleId);
            if (_battle == null) {
                Debug.LogError($"Failed to start battle with id {battleId}");
                return;
            }

            _battle.OnBattleEnded += OnBattleEnded;
        }

        private void OnBattleEnded(object sender, EventArgs e) {
            Debug.Log("Battle ended");
            _battle.OnBattleEnded -= OnBattleEnded;
            _battle = null;
        }

        private void Update() {
            _battle?.Update();
        }

        private void OnGUI() {
            _battle?.OnGUI();
        }
    }
}