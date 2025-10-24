using System;
using System.Collections.Generic;
using Duckov.UI;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Steamworks;
using System.Reflection;
using Duckov.Modding;

namespace OFDuckovFriendname
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony _harmony;

        protected override void OnAfterSetup()
        {
            base.OnAfterSetup();
            
            // 初始化Harmony并应用所有补丁
            _harmony = new Harmony("OFDuckovFriendname");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Debug.Log("OFDuckovFriendname ModBehaviour: 已加载并应用Harmony补丁");
        }

        protected override void OnBeforeDeactivate()
        {
            base.OnBeforeDeactivate();
            
            // 卸载所有补丁
            if (_harmony != null)
            {
                _harmony.UnpatchAll("OFDuckovFriendname");
                _harmony = null;
            }
            
            Debug.Log("OFDuckovFriendname ModBehaviour: 已卸载");
        }

        void OnEnable()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: OnEnable called.");
        }

        void OnDisable()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: OnDisable called.");
        }
    }

    [HarmonyPatch(typeof(HealthBar))]
    public static class CharacterNameDisplay
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshCharacterIcon")]
        public static bool RefreshCharacterIconPrefix(HealthBar __instance)
        {
            try
            {
                if (!__instance.target)
                {
                    return true;
                }
                
                CharacterMainControl characterMainControl = __instance.target.TryGetCharacter();
                if (!characterMainControl)
                {
                    return true;
                }
                
                CharacterRandomPreset characterPreset = characterMainControl.characterPreset;
                if (!characterPreset)
                {
                    return true;
                }
                
                // 强制显示所有角色的名称
                characterPreset.showName = true;
                
                Debug.Log($"设置角色名称显示: {characterPreset.showName}, 是否主角色: {characterMainControl.IsMainCharacter}");
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"RefreshCharacterIconPrefix错误: {e}");
                return true;
            }
        }

        // 添加额外的补丁确保名称显示
        [HarmonyPostfix]
        [HarmonyPatch("RefreshCharacterIcon")]
        public static void RefreshCharacterIconPostfix(HealthBar __instance)
        {
            try
            {
                if (!__instance.target) return;
                
                CharacterMainControl characterMainControl = __instance.target.TryGetCharacter();
                if (!characterMainControl) return;
                
                CharacterRandomPreset characterPreset = characterMainControl.characterPreset;
                if (!characterPreset) return;
                
                // 再次确保名称显示
                characterPreset.showName = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"RefreshCharacterIconPostfix错误: {e}");
            }
        }
    }
}