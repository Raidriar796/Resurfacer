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
    public override string Version => "0.2.1";
    public override string Link => "https://github.com/Raidriar796/Resurfacer";
    public static ModConfiguration Config;

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

        UI.Spacer(24f);

        // Setup buttons for re-encoding
        var reEncodeButton = UI.Button("Re-Encode");

        // Subscribe buttons to methods
        setFormatButton.LocalPressed += SetFormat;
        forceFormatButton.LocalPressed += ForceFormat;
        unsetForceButton.LocalPressed += UnsetForceFormat;
        unsetPreferredButton.LocalPressed += UnsetPreferredFormat;
        reEncodeButton.LocalPressed += ReEncode;
    }

    // Intentionally not local so I can avoid issues with passing arguments to methods
    private static TextureCompression targetFormat = new();

    // Methods for handling info to pass to the main batch action method
    private static void SetFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        BatchAction(targetSlot, setFormatAction);
    }

    private static void ForceFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        BatchAction(targetSlot, forceFormatAction);
    }

    private static void UnsetForceFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        BatchAction(targetSlot, unsetForceAction);
    }

    private static void UnsetPreferredFormat(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        BatchAction(targetSlot, unsetPreferredAction);
    }

    private static void ReEncode(IButton sourceButton, ButtonEventData eventData)
    {
        Slot targetSlot = sourceButton.Slot.GetComponentInParents<ReferenceField<Slot>>().Reference;
        targetFormat = sourceButton.Slot.GetComponentInParents<ValueField<TextureCompression>>().Value;
        BatchAction(targetSlot, reEncodeAction);
    }

    // The actions that are ran per texture in each job
    // IDE0019 is supressed since the intended ways of writing this result in it simply not working
    // If a working solution is found, I will remove the supression
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0019", Justification = "Intended solutions do not work.")]
    private static readonly Action<IAssetRef> setFormatAction = delegate (IAssetRef textureRef)
    {
        StaticTexture2D texture = textureRef.Target as StaticTexture2D;
        if (texture != null && texture.Uncompressed.Value == false && texture.PreferredFormat.Value == null)
        {
            texture.PreferredFormat.Value = targetFormat;
        } 
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0019", Justification = "Intended solutions do not work.")]
    private static readonly Action<IAssetRef> forceFormatAction = delegate (IAssetRef textureRef)
    {
        StaticTexture2D texture = textureRef.Target as StaticTexture2D;
        if (texture != null && texture.Uncompressed.Value == false && texture.PreferredFormat.Value == null)
        {
            texture.PreferredFormat.Value = targetFormat;
            texture.ForceExactVariant.Value = true;
        }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0019", Justification = "Intended solutions do not work.")]
    private static readonly Action<IAssetRef> unsetForceAction = delegate (IAssetRef textureRef)
    {
        StaticTexture2D texture = textureRef.Target as StaticTexture2D;
        if (texture != null && texture.Uncompressed.Value == false)
        {
            texture.ForceExactVariant.Value = false;
        }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0019", Justification = "Intended solutions do not work.")]
    private static readonly Action<IAssetRef> unsetPreferredAction = delegate (IAssetRef textureRef)
    {
        StaticTexture2D texture = textureRef.Target as StaticTexture2D;
        if (texture != null && texture.Uncompressed.Value == false)
        {
            texture.PreferredFormat.Value = null;
        }
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "IDE0019", Justification = "Intended solutions do not work.")]
    private static readonly Action<IAssetRef> reEncodeAction = delegate (IAssetRef textureRef)
    {
        StaticTexture2D texture = textureRef.Target as StaticTexture2D;
        if (texture != null)
        {
            var longestSide = MathX.Max(texture.Asset.BitmapMetadata.Height, texture.Asset.BitmapMetadata.Width);
            texture.Rescale(longestSide, texture.MipMapFilter);
        }
    };

    // Find every material in every mesh, then run an action for each texture on said materials
    private static void BatchAction(Slot targetSlot, Action<IAssetRef> action)
    {
        if (targetSlot == null) return;

        // Used to skip duplicate instances of materials
        HashSet<RefID> materialHashes = [];

        foreach (MeshRenderer mesh in targetSlot.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (IAssetProvider<Material> material in mesh.Materials)
            {
                if (material != null && materialHashes.Add(material.ReferenceID))
                {
                    Worker worker = (Component)material;
                    worker.ForeachSyncMember(action);
                }
            }
        }
    }
}
