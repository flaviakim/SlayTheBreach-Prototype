// using MoonSharp.Interpreter;
//
// public class LuaScriptEffect : ICardEffect {
//     private readonly string _script;
//     private Script _lua;
//
//     public string EffectName { get; }
//     public string InstructionText { get; }
//
//     public LuaScriptEffect(string script, string effectName = "LuaScriptEffect", string instructionText = null) {
//         _script = script;
//         EffectName = effectName;
//         InstructionText = instructionText;
//
//         // Initialize the MoonSharp script engine
//         _lua = new Script();
//
//         // Register external objects for Lua to access
//         UserData.RegisterType<CardEffectHandler>();
//         UserData.RegisterType<MapTile>();
//     }
//
//     public void StartEffect(CardEffectHandler handler) {
//         // Load and execute the Lua script
//         _lua.DoString(_script);
//
//         // Call the "StartEffect" function if defined
//         DynValue startEffectFunction = _lua.Globals.Get("StartEffect");
//         if (startEffectFunction.Type == DataType.Function) {
//             _lua.Call(startEffectFunction);
//         }
//     }
//
//     public void OnSelectedTile(CardEffectHandler handler, MapTile selectedTile, out bool effectFinished) {
//         effectFinished = false;
//
//         // Call the "OnSelectedTile" function if defined
//         DynValue onSelectedTileFunction = _lua.Globals.Get("OnSelectedTile");
//         if (onSelectedTileFunction.Type == DataType.Function) {
//             DynValue result = _lua.Call(onSelectedTileFunction, handler, selectedTile);
//             if (result.Type == DataType.Boolean) {
//                 effectFinished = result.Boolean;
//             }
//         }
//     }
//
//     public void UpdateEffect(CardEffectHandler handler, out bool effectFinished) {
//         effectFinished = false;
//
//         // Call the "UpdateEffect" function if defined
//         DynValue updateEffectFunction = _lua.Globals.Get("UpdateEffect");
//         if (updateEffectFunction.Type == DataType.Function) {
//             DynValue result = _lua.Call(updateEffectFunction, handler);
//             if (result.Type == DataType.Boolean) {
//                 effectFinished = result.Boolean;
//             }
//         }
//     }
//
//     public void EndEffect(CardEffectHandler handler) {
//         // Call the "EndEffect" function if defined
//         DynValue endEffectFunction = _lua.Globals.Get("EndEffect");
//         if (endEffectFunction.Type == DataType.Function) {
//             _lua.Call(endEffectFunction, handler);
//         }
//
//         // Clean up the Lua interpreter
//         _lua = null;
//     }
// }
