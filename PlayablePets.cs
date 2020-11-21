using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System;
using UnityEngine;
using PowerTools;
using System.Collections.Generic;
using BepInEx.Configuration;

/* Among Us types here */
using PlayerControl = GLHCHLEDNBA;
using HudManager = KLEKBPLEDOA;
using LobbyBehaviour = IBNGIHCHBKN;
using DestroyableSingleton_HudManager_ = LBJBHFDNMCK<KLEKBPLEDOA>;

namespace PlayablePets
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Among Us.exe")]
    public class PlayablePets : BasePlugin
    {
        public const string PluginGuid = "tranfox.playablepets";
        public const string PluginName = "Playable Pets";
        public const string PluginVersion = "1.6.1";

        public static ManualLogSource _logger = null;
        public static ConfigFile _config = null;

        //Config variables
        public static ConfigEntry<bool> enabled;
        public static ConfigEntry<bool> onlyLocalPlayer;
        public static ConfigEntry<bool> onlyMiniCrewmate; 

        //Class variabled
        public static Vector3 petPositionOffset = new Vector3(0, -0.08f, 0); //Align the pet slighly better with the player's hitbox
        public static Vector3 namePositionOffset = new Vector3(0, 0.3f, 0);

        public static Dictionary<byte, float> animationStartTimes = new Dictionary<byte, float>(); //id, time

        public static readonly string SpecialName = "PlayablePet";

        public override void Load()
        {
            _logger = this.Log;
            _config = this.Config;

            enabled = Config.Bind<bool>("Enable", "enabled", true, "Enable/Disables the mod");
            onlyLocalPlayer = Config.Bind<bool>("Settings", "onlyLocalPlayer", false, "Only allow local player to be a playable pet");
            onlyMiniCrewmate = Config.Bind<bool>("Settings", "onlyMiniCrewmate", false, "Only apply to mini-crewmates");

            var harmony = new Harmony(PluginGuid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void UpdatePets()
        {
            if (PlayablePets.onlyLocalPlayer.Value)
            {
                if (PlayerControl.LocalPlayer != null)
                {
                    UpdatePet(PlayerControl.LocalPlayer);
                }
            }
            else
            {
                if (PlayerControl.AllPlayerControls != null)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        UpdatePet(player);
                    }
                }
            }
        }

        public static void UpdatePet(PlayerControl player)
        {
            if (player != null)
            {
                var pet = player.CurrentPet;
                if (pet != null)
                {
                    var playerID = player.PlayerId;
                    bool isDead = player.HMPLOOHMKEN.ENDDMNCPFHM;
                    bool localIsDead = PlayerControl.LocalPlayer.HMPLOOHMKEN.ENDDMNCPFHM;

                    if (!animationStartTimes.ContainsKey(playerID))
                    {
                        animationStartTimes.Add(playerID, Time.unscaledTime);
                        //_logger.LogInfo("Added player to animationStartTimes");
                    }
                    if (pet.sadClip == null) { return; } //No pet, continue on
                    if (!pet.name.Contains("Crewmate") && PlayablePets.onlyMiniCrewmate.Value) { return; } //If the pet isn't a mini-crewmate and it should only apply to mini's skip

                    pet.Collider.enabled = false; //Disable the pet collider so it doesn't get stuck on walls

                    pet.transform.position = player.transform.position + petPositionOffset; //Set pet position

                    pet.animator.name = pet.animator.name.Replace(PlayablePets.SpecialName, ""); //Allow animations to be changed

                    pet.rend.flipX = player.GDCMEAGNAEP.flipX; //Flip pet spriterenderer if needed

                    if (isDead) //If player is dead
                    {
                        if (PlayerControl.LocalPlayer == player)
                        {
                            pet.rend.color = new UnityEngine.Color(1, 1, 1, 0.5f);
                            pet.shadowRend.color = new UnityEngine.Color(1, 1, 1, 0.5f);
                        }
                        else
                        {
                            if (localIsDead) //If local player is dead
                            {
                                pet.rend.color = new UnityEngine.Color(1, 1, 1, 0.0f); //Set pet to not be visible
                                pet.shadowRend.color = new UnityEngine.Color(1, 1, 1, 0.0f); //Set pet to not be visible
                            }
                            else
                            {
                                pet.rend.color = new UnityEngine.Color(1, 1, 1, 0.5f); //Set pet to half visible
                                pet.shadowRend.color = new UnityEngine.Color(1, 1, 1, 0.5f); //Set pet to half visible
                            }
                        }
                    }
                    else
                    {
                        pet.rend.color = new UnityEngine.Color(1, 1, 1, 1);
                        pet.shadowRend.color = new UnityEngine.Color(1, 1, 1, 1);
                    }
                    

                    if (player.GDCMEAGNAEP.sprite.name == "idle" && pet.animator.Clip != pet.idleClip)
                    {
                        pet.animator.Play(pet.idleClip);
                        //_logger.LogMessage("Idle Alive");
                        animationStartTimes[playerID] = Time.unscaledTime;
                    }
                    else if (isDead && (player.Collider.attachedRigidbody.velocity == Vector2.zero) && pet.animator.Clip != pet.idleClip)
                    {
                        pet.animator.Play(pet.idleClip);
                        //_logger.LogMessage("Idle Ghost");
                        animationStartTimes[playerID] = Time.unscaledTime;
                    }
                    else if (player.GDCMEAGNAEP.sprite.name.Contains("walk") && pet.animator.Clip != pet.walkClip)
                    {
                        pet.animator.Play(pet.walkClip);
                        //_logger.LogMessage("Walk Alive");
                        animationStartTimes[playerID] = Time.unscaledTime;
                    }
                    else if (isDead && (player.Collider.attachedRigidbody.velocity != Vector2.zero) && pet.animator.Clip != pet.walkClip)
                    {
                        pet.animator.Play(pet.walkClip);
                        //_logger.LogMessage("Walk Ghost");
                        animationStartTimes[playerID] = Time.unscaledTime;
                    }
                    else
                    {
                        //_logger.LogMessage("None hit");
                    }

                    pet.animator.Time = (Time.unscaledTime - animationStartTimes[playerID]);

                    pet.animator.name += PlayablePets.SpecialName; //Stop animations from being changed when not outside this loop.
                }
            }
        }

        public static void UpdatePlayers()
        {
            if (PlayablePets.onlyLocalPlayer.Value)
            {
                UpdatePlayer(PlayerControl.LocalPlayer);
            }
            else
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    UpdatePlayer(player);
                }
            }
        }

        public static void UpdatePlayer(PlayerControl player)
        {
            if (player != null)
            {
                if (player.CurrentPet != null)
                {
                    if (player.CurrentPet.animator.name.Contains(PlayablePets.SpecialName))
                    {
                        if (player.nameText != null)
                        {
                            player.nameText.transform.position = player.CurrentPet.transform.position + PlayablePets.namePositionOffset;

                            setupPlayerColors(player, Color.clear);
                        }
                    }
                    else
                    {
                        //PlayablePets._logger.LogMessage(player.HatRenderer.FrontLayer.sprite.name);
                        bool hasHat = !(player.HatRenderer.FrontLayer.sprite.name == "hats0001");
                        player.nameText.transform.localPosition = new Vector3(0f, (!hasHat) ? 0.7f : 1.05f, -0.5f);
                        setupPlayerColors(player, Color.white);
                    }
                }
            }
        }

        private static void setupPlayerColors(PlayerControl player, Color c)
        {
            if (player.GDCMEAGNAEP != null)
            {
                player.GDCMEAGNAEP.color = c;
            }
            if (player.HatRenderer != null)
            {
                player.HatRenderer.BackLayer.color = c;
                player.HatRenderer.FrontLayer.color = c;
            }
            if (player.MyPhysics.Skin != null)
            {
                player.MyPhysics.Skin.layer.color = c;
            }
        }
    }

    public static class Patches
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class Patch_HudManager_Update
        {
            public static void Postfix()
            {
                try
                {
                    if (PlayablePets.enabled.Value)
                    {
                        PlayablePets.UpdatePets();
                        PlayablePets.UpdatePlayers();
                    }
                }
                catch (Exception e)
                {
                    PlayablePets._logger.LogError(e);
                }
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        public static class Patch_LobbyBehaviour_FixedUpdate
        {
            public static void Postfix()
            {
                try
                {
                    HudManager hudManager = DestroyableSingleton_HudManager_.MJPDBBJAMGG;

                    if (PlayablePets.enabled.Value)
                    {
                        if (hudManager != null) //This might be null
                        {
                            string creditPleaseDontRemoveThisKThx = $"{PlayablePets.PluginName} v{PlayablePets.PluginVersion} created by @Tran_Foxxo\r\nhttps://github.com/Tran-Foxxo/PlayablePets/\r\n\r\n";
                            if (!hudManager.GameSettings.Text.Contains(creditPleaseDontRemoveThisKThx))
                            {
                                hudManager.GameSettings.Text = creditPleaseDontRemoveThisKThx + hudManager.GameSettings.Text;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    PlayablePets._logger.LogError(e);
                }
            }
        }

        //All the spriteanim patches use this function
        public static bool SkipSpriteAnim(SpriteAnim instance)
        {
            if (PlayablePets.enabled.Value && instance != null) //Make sure this isn't null
            {
                if (instance.name.Contains(PlayablePets.SpecialName))
                {
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(SpriteAnim), "SetTime")]
        public static class Patch_PowerTools_SpriteAnim_SetTime
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Pause")]
        public static class Patch_PowerTools_SpriteAnim_Pause
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Play")]
        public static class Patch_PowerTools_SpriteAnim_Play
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Play")]
        public static class Patch2_PowerTools_SpriteAnim_Play
        {
            public static bool Prefix(SpriteAnim __instance, AnimationClip anim, float speed)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Reset")]
        public static class Patch_PowerTools_SpriteAnim_Reset
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Stop")]
        public static class Patch_PowerTools_SpriteAnim_Stop
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }

        [HarmonyPatch(typeof(SpriteAnim), "Resume")]
        public static class Patch_PowerTools_SpriteAnim_Resume
        {
            public static bool Prefix(SpriteAnim __instance)
            {
                //PlayablePets._logger.LogInfo(MethodBase.GetCurrentMethod().DeclaringType.Name + " " + __instance.name);
                return SkipSpriteAnim(__instance);
            }
        }
    }
}
