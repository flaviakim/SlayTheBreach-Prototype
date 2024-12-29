using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureEditorUICreator : MonoBehaviour {

    private UIDocument _uiDocument;
    private readonly CreaturePrototypeCollection _jsonCreaturePrototypeCollection = new();

    private void Awake() {
        if (!TryGetComponent(out _uiDocument)) {
            Debug.LogError("UIDocument component not found.");
            _uiDocument = gameObject.AddComponent<UIDocument>();
        }
    }

    private void Start() {
        GenerateUIElements();
    }

    private void GenerateUIElements([CanBeNull] Creature.CreatureData creatureData = null) {
        var creatureDataType = typeof(Creature.CreatureData);
        var fields = creatureDataType.GetFields();
        var root = _uiDocument.rootVisualElement;

        var verticalContainer = new VisualElement();
        root.Add(verticalContainer);
        var header = new Label("Creature Editor");
        verticalContainer.Add(header);
        var scrollView = new ScrollView();
        verticalContainer.Add(scrollView);
        var scrollContainer = new VisualElement();
        scrollView.Add(scrollContainer);

        foreach (var field in fields) {
            var fieldContainer = new VisualElement();
            fieldContainer.style.flexDirection = FlexDirection.Row;
            scrollContainer.Add(fieldContainer);

            var label = new Label(field.Name);
            label.style.width = 150;
            fieldContainer.Add(label);

            if (field.FieldType == typeof(string)) {
                var textField = new TextField {
                    name = field.Name,
                    value = creatureData == null ? "" : field.GetValue(creatureData) as string,
                    style = { flexGrow = 1 }
                };
                fieldContainer.Add(textField);
            } else if (field.FieldType == typeof(int)) {
                var integerField = new IntegerField {
                    name = field.Name,
                    value = creatureData == null ? 1 : (int)field.GetValue(creatureData),
                    style = { flexGrow = 1 }
                };
                fieldContainer.Add(integerField);
            } else if (field.FieldType == typeof(float)) {
                var floatField = new FloatField {
                    name = field.Name,
                    value = creatureData == null ? 1f : (float)field.GetValue(creatureData),
                    style = { flexGrow = 1 }
                };
                fieldContainer.Add(floatField);
            } else if (field.FieldType == typeof(Faction)) {
                var factionField = new EnumField {
                    name = field.Name,
                    value = creatureData == null ? Faction.Enemy : (Faction)field.GetValue(creatureData),
                    style = { flexGrow = 1 }
                };
                factionField.Init(Faction.Player);
                fieldContainer.Add(factionField);
            }
        }

        var saveButton = new Button(SaveCreatureData) {
            text = "Save"
        };
        verticalContainer.Add(saveButton);
    }

    private void SaveCreatureData() {
        var creatureData = new Creature.CreatureData();
        var fields = creatureData.GetType().GetFields();
        foreach (var field in fields) {
            if (field.FieldType == typeof(string)) {
                var textField = _uiDocument.rootVisualElement.Q<TextField>(field.Name);
                field.SetValue(creatureData, textField.value);
            } else if (field.FieldType == typeof(int)) {
                var integerField = _uiDocument.rootVisualElement.Q<IntegerField>(field.Name);
                field.SetValue(creatureData, integerField.value);
            } else if (field.FieldType == typeof(float)) {
                var floatField = _uiDocument.rootVisualElement.Q<FloatField>(field.Name);
                field.SetValue(creatureData, floatField.value);
            } else if (field.FieldType == typeof(Faction)) {
                var factionField = _uiDocument.rootVisualElement.Q<EnumField>(field.Name);
                field.SetValue(creatureData, factionField.value);
            }
        }

        // Save creature data to file
        var saved = _jsonCreaturePrototypeCollection.TrySavePrototypeData(creatureData, overwrite: false);
        // TODO confirmation dialog
    }
}


