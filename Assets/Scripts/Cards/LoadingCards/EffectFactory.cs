using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory {
    private readonly Dictionary<string, Func<Dictionary<string, object>, ICardEffect>> _effectCreators;

    public EffectFactory() {
        _effectCreators = new Dictionary<string, Func<Dictionary<string, object>, ICardEffect>> {
            {
                "PlayerSelectOtherTargetsEffect", parameters => new PlayerSelectOtherTargetsEffect(
                    Convert.ToInt32(parameters["TargetCount"]),
                    Convert.ToInt32(parameters["RangeFromCardTarget"]),
                    Enum.Parse<FactionRelationship>(parameters["FactionRelationship"].ToString()))
            }, {
                "AutoSelectAllOtherTargetsEffect", parameters => new AutoSelectAllOtherTargetsEffect(
                    Convert.ToInt32(parameters["RangeFromCardTarget"]),
                    Enum.Parse<FactionRelationship>(parameters["FactionRelationship"].ToString()))
            },

            { "DamageEffect", parameters => new DamageEffect(Convert.ToInt32(parameters["DamageAmount"])) },

            { "MoveCardEffect", parameters => new MoveEffect(Convert.ToInt32(parameters["MoveDistance"])) },

            // { "LuaScriptEffect", parameters => new LuaScriptEffect(parameters["Script"].ToString()) }
        };
    }

    public ICardEffect CreateEffect(string type, Dictionary<string, object> parameters) {

        if (_effectCreators.TryGetValue(type, out var creator)) {
            try {
                return creator(parameters);
            }
            catch (Exception e) {
                Debug.LogError($"Error creating effect of type '{type} with parameters {parameters}': {e.Message}");
                // throw new Exception($"Error creating effect of type '{type} with parameters {parameters}': {e.Message}");
            }
        }
        else {
            Debug.LogError($"Effect type '{type}' is not recognized.");
        }

        return null;
    }
}