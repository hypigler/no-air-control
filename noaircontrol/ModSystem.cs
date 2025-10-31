using no_air_control.Config;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using System.Security.Cryptography.X509Certificates;
using System.Dynamic;
using Vintagestory.Server;
using System.Threading;
using System;

namespace no_air_control;
public class NoAirControlSystem : ModSystem
{
    public static ILogger Logger { get; private set; }
    public static string ModId { get; private set; }
    public static ICoreAPI Api { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static ModConfig ClientConfig { get; private set; }
    public static ModConfig ServerConfig { get; private set; }
    public static float ACS { get; private set; }

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

        api.Event.IsPlayerReady += OnPlayerReady;
    }

    public static bool OnPlayerReady(ref EnumHandling handling) {
        ACS = Api.World.Config.GetFloat("ACS");
        return true;
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);

        try
        {
            // api.LoadModConfig<T> will save/load to the server's ModConfig folder
            ServerConfig = api.LoadModConfig<ModConfig>("NoAirControl.json");
            if (ServerConfig == null)
            {
                ServerConfig = new ModConfig();
                api.StoreModConfig(ServerConfig, "NoAirControl.json");
            }
        }
        catch (System.Exception e)
        {
            api.Logger.Error($"[AirControlMod] Failed to load server config: {e.Message}");
            ServerConfig = new ModConfig();
        }

        // Sets to World Config for client access
        api.World.Config.SetFloat("ACS", ServerConfig.AirControlStrength);

        api.Logger.Notification($"[AirControlMod] Server-side Air Control Strength: {ServerConfig.AirControlStrength}");
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
