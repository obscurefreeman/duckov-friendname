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
        void Awake()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: Awake called.");
        }

        void OnEnable()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: OnEnable called.");
        }

        void OnDisable()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: OnDisable called.");
        }

        void OnDestroy()
        {
            Debug.Log("OFDuckovFriendname ModBehaviour: OnDestroy called.");
        }
    }

    [HarmonyPatch(typeof(HealthBar))]
    public static class CharacterNameDisplay
    {
        [HarmonyPrefix]
        [HarmonyPatch("RefreshCharacterIcon")]
        public static bool RefreshCharacterIconPrefix(HealthBar __instance)
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
            characterPreset.showName = !characterMainControl.IsMainCharacter;
            return true;
        }
    }
}