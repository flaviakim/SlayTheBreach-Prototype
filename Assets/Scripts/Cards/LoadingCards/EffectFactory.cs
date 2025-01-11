using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory {
    private readonly Dictionary<string, Func<Dictionary<string, object>, ICardEffect>> _effectCreators;

    public EffectFactory() {
        _effectCreators = new Dictionary<string, Func<Dictionary<string, object>, ICardEffect>> {
            {
                "PlayerSelectOtherTargetsEffect", parameters => new PlayerSelectOtherTargetsCardCardEffect(
                    Convert.ToInt32(parameters["TargetCount"]),
                    Convert.ToInt32(parameters["RangeFromCardTarget"]),
                    Enum.Parse<FactionRelationship>(parameters["FactionRelationship"].ToString()))
            }, {
                "AutoSelectAllOtherTargetsEffect", parameters => new AutoSelectAllOtherTargetsCardEffect(
                    Convert.ToInt32(parameters["RangeFromCardTarget"]),
                    Enum.Parse<FactionRelationship>(parameters["FactionRelationship"].ToString()))
            },

            { "DamageEffect", parameters => new DamageCardEffect(Convert.ToInt32(parameters["DamageAmount"])) },

            { "MoveCardEffect", parameters => new MoveCardEffect(Convert.ToInt32(parameters["MoveDistance"])) },

            // { "PushCardEffect", parameters => new PushCardEffect(
            //     (PushIntoCreatureVersion) Enum.Parse(typeof(PushIntoCreatureVersion), parameters["PushIntoCreatureVersion"].ToString()),
            //     Convert.ToInt32(parameters["PushDistance"]),
            //     Convert.ToInt32(parameters["PushRange"]),
            //     Convert.ToInt32(parameters["PushDamage"])
            // ) },
            // with default values, if parameters are absent instead:
            { "PushCardEffect", parameters => new PushCardEffect(
                (PushIntoCreatureVersion) Enum.Parse(typeof(PushIntoCreatureVersion), parameters.GetValueOrDefault("PushIntoCreatureVersion", "PushIntoCreatureDamage").ToString()),
                Convert.ToInt32(parameters.GetValueOrDefault("PushDistance", 1)),
                Convert.ToInt32(parameters.GetValueOrDefault("PushRange", 1)),
                Convert.ToInt32(parameters.GetValueOrDefault("PushDamage", 0))
            ) },


            // Future: Possible future effects:
            // { "DrawCardEffect", parameters => new DrawCardEffect(Convert.ToInt32(parameters["DrawAmount"])) },
            // { "DiscardCardEffect", parameters => new DiscardCardEffect(Convert.ToInt32(parameters["DiscardAmount"])) },
            // { "HealEffect", parameters => new HealCardEffect(Convert.ToInt32(parameters["HealAmount"])) },
            // { "SummonEffect", parameters => new SummonCardEffect(parameters["CreatureID"].ToString()) },
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