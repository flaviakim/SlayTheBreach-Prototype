using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLoop {
    public class BattleLoop : MonoBehaviour {
        [SerializeField] private string battleSceneName = "BattleScene";
        private readonly BattleFactory _battleFactory = new();
        private Battle _battle;

        private string _sceneNameWhenBattleEnds;

        private void Awake() {
            // Make sure, only one instance of this object exists
            var objects = FindObjectsByType<BattleLoop>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (objects.Length > 1) {
                Debug.LogWarning("Multiple BattleLoop instances found. Destroying all but one.");
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Update() {
            _battle?.Update();
        }

        private void OnGUI() {
            _battle?.OnGUI();
        }

        public List<string> GetAllBattlesIDs() {
            var battles = _battleFactory.GetPrototypeNames();
            return battles;
        }

        public void StartBattleInNewScene(string battleId, string sceneNameWhenBattleEnds = null) {
            if (string.IsNullOrEmpty(battleSceneName)) {
                Debug.LogError("Battle scene name is not set");
                return;
            }

            _sceneNameWhenBattleEnds = sceneNameWhenBattleEnds;

            SceneManager.LoadScene(battleSceneName);
            SceneManager.sceneLoaded += OnSceneManagerSceneLoaded;
            return;

            void OnSceneManagerSceneLoaded(Scene scene, LoadSceneMode mode) {
                Debug.Assert(scene.name == battleSceneName, $"Expected scene name {battleSceneName}, got {scene.name}");
                _battle = _battleFactory.CreateBattleInstance(battleId);
                if (_battle == null) {
                    Debug.LogError($"Failed to start battle with id {battleId}");
                    return;
                }

                _battle.BattleEndedEvent += OnBattleEnded;

                SceneManager.sceneLoaded -= OnSceneManagerSceneLoaded;
            }
        }

        private void OnBattleEnded(object sender, EventArgs e) {
            Debug.Log("Battle ended");
            _battle.BattleEndedEvent -= OnBattleEnded;
            _battle = null;

            if (!string.IsNullOrEmpty(_sceneNameWhenBattleEnds)) SceneManager.LoadScene(_sceneNameWhenBattleEnds);
        }
    }
}