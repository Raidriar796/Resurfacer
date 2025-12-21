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
    public override string Version => "0.1.0";
    public override string Link => "https://github.com/Raidriar796/Resurfacer";
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
        UIBuilder UI = RadiantUI_Panel.SetupPanel(ResurfacerUI, "Resurfacer", new float2(360f, 480f), true, true);
        RadiantUI_Constants.SetupEditorStyle(UI);
        VerticalLayout verticalLayout = UI.VerticalLayout(4f);
        UI.Style.MinHeight = 24f;
        UI.Style.PreferredHeight = 24f;

        // Prevent accidents and abuse
        UI.Canvas.AcceptPhysicalTouch.Value = false;
        UI.Canvas.MarkDeveloper();

        UI.Text("Target Hierarchy:", true, Alignment.BottomCenter);
        UI.Next("Root");

        // Setup target slot field
        ReferenceField<Slot> targetSlot = ResurfacerUI.AttachComponent<ReferenceField<Slot>>();
        UI.Current.AttachComponent<RefEditor>().Setup(targetSlot.Reference);

        UI.Spacer(24f);

        // Setup preferred format
        ValueField<TextureCompression> targetFormat = ResurfacerUI.AttachComponent<ValueField<TextureCompression>>();
        UI.EnumMemberEditor(targetFormat.Value);

        // Setup buttons for texture formats
        var setFormatButton = UI.Button("Set Preferred Format");
        var forceFormatButton = UI.Button("Force Preferred Format");
        var unsetForceButton = UI.Button("Unset Force Format");
        var unsetPreferredButton = UI.Button("Unset Preferred Format");

        // Subscribe buttons to methods
        setFormatButton.LocalPressed += SetFormat;
        forceFormatButton.LocalPressed += ForceFormat;
        unsetForceButton.LocalPressed += UnsetForceFormat;
        unsetPreferredButton.LocalPressed += UnsetPreferredFormat;

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

    private static void UnsetForceFormat(IButton sourceButton, ButtonEventData eventData)
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
                                if (texture.Uncompressed.Value == false)
                                {
                                    texture.ForceExactVariant.Value = false;
                                }
                            }
                        });
                    }
                    worker.ForeachSyncMember<IAssetRef>(action);
                }
            }
        }
    }

    private static void UnsetPreferredFormat(IButton sourceButton, ButtonEventData eventData)
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
                                if (texture.Uncompressed.Value == false)
                                {
                                    texture.PreferredFormat.Value = null;
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
