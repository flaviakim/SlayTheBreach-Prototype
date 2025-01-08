using GameLoop;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleSelectionUI : MonoBehaviour {

    private UIDocument _uiDocument;
    private BattleLoop _battleLoop;
    [SerializeField] private string battleSelectionSceneName = "BattleSelectionScene";

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
        _battleLoop = FindFirstObjectByType<BattleLoop>() ?? new GameObject("NewBattleLoop").AddComponent<BattleLoop>();

        ShowMenu();
    }

    public void ShowMenu() {
        CreateMenu();
    }

    public void HideMenu() {
        _uiDocument.rootVisualElement.Clear();
    }

    /// <summary>
    /// Creates a menu with a selector for the player to choose a battle to start.
    /// </summary>
    private void CreateMenu() {
        var battleIDs = _battleLoop.GetAllBattlesIDs();

        var root = _uiDocument.rootVisualElement;
        root.style.flexDirection = FlexDirection.Column;
        root.style.flexGrow = 1;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;

        var label = new Label("Choose a battle to start:");
        root.Add(label);

        var selector = new PopupField<string>("Battle", battleIDs, 0);
        root.Add(selector);

        var startButton = new Button(() => {
            HideMenu();
            _battleLoop.StartBattleInNewScene(selector.value, battleSelectionSceneName);
        });
        startButton.text = "Start";
        root.Add(startButton);
    }

}