using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.UIX;
using Elements.Assets;
using Elements.Core;

namespace Resurfacer;

public class Resurfacer : ResoniteMod
{
    public override string Name => "Resurfacer";
    public override string Author => "Raidriar796";
    public override string Version => "1.0.0";
    public override string Link => "";
    public static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.raidriar796.Resurfacer");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }
}
