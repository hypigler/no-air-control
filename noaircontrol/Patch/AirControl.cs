using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using no_air_control;

// We are patching the ApplyFreeFall method.
[HarmonyPatch(typeof(PModulePlayerInAir), "ApplyFreeFall")] 
public class AirControl
{
    // Required Harmony argument injections to access instance data and method parameters.
    [HarmonyPrefix]
    public static bool Prefix(PModulePlayerInAir __instance, float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        EntityPlayer player = entity as EntityPlayer;
        // 1. Handle Climbing: If climbing, allow the original method to run the base logic.
        if (player == null || controls.IsClimbing)
        {
            // By returning true, we execute the original method's 'if (controls.IsClimbing)' block 
            // and then allow it to exit cleanly via base.ApplyFreeFall().
            return true;
        }
        // 2. Modified Freefall Logic (Always use high strength/no drag)
        
        // Replicate the original 'float strength' calculation
        // __instance.AirMovingStrength correctly accesses the property from the base PModuleInAir class.
        float strength = __instance.AirMovingStrength * Math.Min(1, ((EntityPlayer)entity).walkSpeed) * dt * 60 * NoAirControlSystem.Config.AirControlStrength;

        // 3. Apply the movement vector using the calculated strength (which is always the high strength)
        pos.Motion.Add(
            controls.WalkVector.X * strength, 
            controls.WalkVector.Y * strength, 
            controls.WalkVector.Z * strength
        );
        
        // Return false to skip execution of the original ApplyFreeFall method.
        return false; 
    }
}