using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using LibCpp2IL.Logging;
using SOD.Common;
using SOD.Common.BepInEx.Configuration;
using SOD.Common.Extensions;

namespace Sod1_0QuickPatch;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency(SOD.Common.Plugin.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class QuickPatch : BasePlugin
{
    
    public const string PLUGIN_GUID = "Severedsolo.SOD.QuickPatch";
    public const string PLUGIN_NAME = "QuickPatch";
    public const string PLUGIN_VERSION = "1.0.0";
    private ConfigEntry<bool> disableCoverups { get; set; }
    private ConfigEntry<bool> forceEnablePasswords { get; set; }
    
    public override void Load()
    {
        Lib.SaveGame.OnAfterNewGame += RunPatches;
        Lib.SaveGame.OnAfterLoad += RunPatches;
        disableCoverups = Config.Bind("Settings", "Sod1_0QuickPatch.DisableCoverups", true, "Disable Coverups");
        forceEnablePasswords = Config.Bind("Settings", "Sod1_0QuickPatch.forcePasswordCompletion", true, "Force Password Related Business to be unlocked");
    }

    private void RunPatches(object? sender, EventArgs e)
    {
        GameplayControls.Instance.enableCoverUps = !disableCoverups.Value;
        if (!forceEnablePasswords.Value) return;
        List<Company> allCompanies = CityData.Instance.companyDirectory.ToList();
        for (int i = 0; i < allCompanies.Count; i++)
        {
            Company c = allCompanies[i];
            if (c?.address?.addressPreset == null) continue;
            if (!c.address.addressPreset.needsPassword) continue;
            GameplayController.Instance.SetPlayerKnowsPassword(c.address);
            Log.LogInfo("Unlocked "+c.name);
        }
    }
}