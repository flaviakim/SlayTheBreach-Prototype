using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "Scriptable Objects/CardScriptableObject")]
public class CardScriptableObject : ScriptableObject {

    public string cardName;
    public string description;

    public AttackInfo attackInfo;
    public MoveInfo moveInfo;

}

[Serializable]
public struct AttackInfo {
    public int damage;
    public int distance;
}

[Serializable]
public struct MoveInfo {
    public int distance;
}

