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

        // Call void method to avoid inability to cast to action
        Engine.Current.RunPostInit(AddEditor);
    }

    private static void AddEditor()
    {
        // Add new button to editors submenu
        DevCreateNewForm.AddAction("Editor", "Resurfacer", (UI) => SpawnResurfacer(UI));
    }

    private static void SpawnResurfacer(Slot ResurfacerUI)
    {
        // Slot setup
        ResurfacerUI.PersistentSelf = false;
        ResurfacerUI.LocalScale *= 0.0005f;

        // Initial UI generation
        UIBuilder UI = RadiantUI_Panel.SetupPanel(ResurfacerUI, "Resurfacer", new float2(360f, 360f), true, true);
        RadiantUI_Constants.SetupEditorStyle(UI);
        VerticalLayout verticalLayout = UI.VerticalLayout(4f);
        UI.Style.MinHeight = 24f;
        UI.Style.PreferredHeight = 24f;

        // Prevent accidents and abuse
        UI.Canvas.AcceptPhysicalTouch.Value = false;
        UI.Canvas.MarkDeveloper();

        // Setup target slot field
        ReferenceField<Slot> targetSlot = ResurfacerUI.AttachComponent<ReferenceField<Slot>>();
        UI.Text("Target Hierarchy:", true, Alignment.BottomCenter);
        UI.Next("Root");
        UI.Current.AttachComponent<RefEditor>().Setup(targetSlot.Reference);

        // Setup preferred format
        ValueField<TextureCompression> targetFormat = ResurfacerUI.AttachComponent<ValueField<TextureCompression>>();
        UI.EnumMemberEditor(targetFormat.Value);

        UI.Spacer(24f);

        // Buttons that do things maybe
        var setFormatButton = UI.Button("Set Format");
        var forceFormatButton = UI.Button("Force Format");

        // Subscribe buttons to do things
        setFormatButton.LocalPressed += SetFormat;
        forceFormatButton.LocalPressed += ForceFormat;

    }

    private static void SetFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        TextureCompression targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        Action<IAssetRef> primaryAction = null!;
        if (targetSlot == null) return;

        foreach (MeshRenderer mesh in targetSlot.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (IAssetProvider<Material> material in mesh.Materials)
            {
                if (material != null)
                {
                    Worker worker = (Component)material;
                    Action<IAssetRef> action;
                    if ((action = primaryAction) == null)
                    {
                        action = (primaryAction = delegate (IAssetRef textureRef)
                        {
                            StaticTexture2D texture = textureRef.Target as StaticTexture2D;
                            if (texture != null)
                            {
                                if (texture.Uncompressed.Value == false && texture.PreferredFormat.Value == null)
                                {
                                    texture.PreferredFormat.Value = targetFormat;
                                }
                            }
                        });
                    }
                    worker.ForeachSyncMember<IAssetRef>(action);
                }
            }
        }
    }

    private static void ForceFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        TextureCompression targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        Action<IAssetRef> primaryAction = null!;
        if (targetSlot == null) return;

        foreach (MeshRenderer mesh in targetSlot.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (IAssetProvider<Material> material in mesh.Materials)
            {
                if (material != null)
                {
                    Worker worker = (Component)material;
                    Action<IAssetRef> action;
                    if ((action = primaryAction) == null)
                    {
                        action = (primaryAction = delegate (IAssetRef textureRef)
                        {
                            StaticTexture2D texture = textureRef.Target as StaticTexture2D;
                            if (texture != null)
                            {
                                if (texture.Uncompressed.Value == false && texture.PreferredFormat.Value == null)
                                {
                                    texture.PreferredFormat.Value = targetFormat;
                                    texture.ForceExactVariant.Value = true;
                                }
                            }
                        });
                    }
                    worker.ForeachSyncMember<IAssetRef>(action);
                }
            }
        }
    }
}
