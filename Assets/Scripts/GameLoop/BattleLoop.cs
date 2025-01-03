using System;
using UnityEngine;

namespace GameLoop {
    public class BattleLoop : MonoBehaviour {
        [SerializeField] private string battleId = "TestBattle";

        private readonly BattleFactory _battleFactory = new BattleFactory();
        private Battle _battle;

        private void Start() {
            StartBattle(battleId);
        }

        public void StartBattle(string battleId) {
            _battle = _battleFactory.CreateBattleInstance(battleId);
        }

        private void Update() {
            _battle.Update();
        }

        private void OnGUI() {
            _battle.OnGUI();
        }
    }
}