using no_air_control.Config;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using System.Security.Cryptography.X509Certificates;
using System.Dynamic;

namespace no_air_control;
public class NoAirControlSystem : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig Config { get; private set; }

    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        Api = api;
        Logger = Mod.Logger;
        ModId = Mod.Info.ModID;
        HarmonyInstance = new Harmony(ModId);
        HarmonyInstance.PatchAll();
    }
    
    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        // Logger.Notification("Hello from template mod: " + api.Side);
        // Logger.StoryEvent("Loading"); // Sample story event (shown when loading a world)
        // Logger.Event("Templates loaded..."); // Sample event (shown when loading in dev mode or in logs)
    }
    
    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        // Logger.Notification("Hello from template mod client side: " + Lang.Get("noaircontrol:hello"));

        // Try to load clientconfig first.
        try
        {
            // api.LoadModConfig<T> will save/load to the server's ModConfig folder
            Config = api.LoadModConfig<ModConfig>("NoAirControl.json");
            if (Config == null)
            {
                Config = new ModConfig();
                api.StoreModConfig(Config, "NoAirControl.json");
            }
        }
        catch (System.Exception e)
        {
            api.Logger.Error($"[AirControlMod] Failed to load client config: {e.Message}");
            Config = new ModConfig();
        }
        
        // Then try to load from server
        airControlStrength = api.World.Config.GetFloat("AirControlStrength", 0);
        api.Logger.Notification($"[AirControlMod] Client-side Air Control Strength: {Config.AirControlStrength}");
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);

        try
        {
            // api.LoadModConfig<T> will save/load to the server's ModConfig folder
            Config = api.LoadModConfig<ModConfig>("NoAirControl.json");
            if (Config == null)
            {
                Config = new ModConfig();
                api.StoreModConfig(Config, "NoAirControl.json");
            }
        }
        catch (System.Exception e)
        {
            api.Logger.Error($"[AirControlMod] Failed to load server config: {e.Message}");
            Config = new ModConfig();
        }

        // Sets to World Config for client access
        api.World.Config.SetFloat("airControlStrength", Config.airControlStrength);

        api.Logger.Notification($"[AirControlMod] Server-side Air Control Strength: {Config.AirControlStrength}");
        }

    public override void Dispose()
    {
        HarmonyInstance?.UnpatchAll(ModId);
        HarmonyInstance = null;
        Logger = null;
        ModId = null;
        Api = null;
        base.Dispose();
    }
}
