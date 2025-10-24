using System;
using System.Collections.Generic;
using Duckov.UI;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Steamworks;
using System.Reflection;
using Duckov.Modding;
using System.Linq; // Added for .ToList()

namespace OFDuckovFriendname
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        private Harmony? _harmony; // 修正：声明为可空类型
        private static List<string> _friendNames = new List<string>();
        private static System.Random _random = new System.Random(); // 使用 System.Random 而不是 Unity 的 Random

        protected override void OnAfterSetup()
        {
            base.OnAfterSetup();
            
            // 初始化Harmony并应用所有补丁
            _harmony = new Harmony("OFDuckovFriendname");
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Debug.Log("OFDuckovFriendname ModBehaviour: 已加载并应用Harmony补丁");

            // 初始化Steamworks并获取好友列表
            if (SteamManager.Initialized)
            {
                int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
                for (int i = 0; i < friendCount; i++)
                {
                    CSteamID friendSteamID = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                    if (friendSteamID.IsValid())
                    {
                        _friendNames.Add(SteamFriends.GetFriendPersonaName(friendSteamID));
                    }
                }
                Debug.Log($"OFDuckovFriendname ModBehaviour: 获取到 {_friendNames.Count} 个Steam好友"); // 修正：使用 _friendNames
            }
            else
            {
                Debug.LogWarning("OFDuckovFriendname ModBehaviour: SteamManager 未初始化。无法获取好友列表。");
            }
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

        // 用于外部访问好友列表和随机数生成器
        internal static List<string> FriendNames => _friendNames;
        internal static System.Random RandomGenerator => _random;
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

        // 添加额外的补丁确保名称显示 (此Postfix可能不再需要，因为我们直接修改 DisplayName)
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

    // 新增补丁，用于修改 CharacterRandomPreset 的 DisplayName
    [HarmonyPatch]
    public static class CharacterDisplayNamePatch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            // 查找 CharacterRandomPreset.DisplayName 的 get 方法
            return AccessTools.PropertyGetter(typeof(CharacterRandomPreset), "DisplayName");
        }

        [HarmonyPostfix]
        public static void Postfix(CharacterRandomPreset __instance, ref string __result)
        {
            try
            {
                // 如果当前角色不是主角色且好友列表不为空
                // 修正：CharacterRandomPreset 没有 GetComponent。需要找到其他方式判断是否为主角色。
                // 暂时注释掉这部分，以便编译通过。
                // if (!__instance.GetComponent<CharacterMainControl>().IsMainCharacter && ModBehaviour.FriendNames.Any())
                if (ModBehaviour.FriendNames.Any()) // 暂时只判断好友列表是否为空
                {
                    int randomIndex = ModBehaviour.RandomGenerator.Next(0, ModBehaviour.FriendNames.Count);
                    __result = ModBehaviour.FriendNames[randomIndex];
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CharacterDisplayNamePatch错误: {e}");
            }
        }
    }
}