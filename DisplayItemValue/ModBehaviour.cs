using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SodaCraft.Localizations;

namespace OFDisplayItemRarity
{

    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        TextMeshProUGUI? _text = null;
        TextMeshProUGUI Text
        {
            get
            {
                if (_text == null)
                {
                    _text = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI);
                }
                return _text;
            }
        }
        void Awake()
        {
            Debug.Log("OFDisplayItemRarity Loaded!!!");
        }
        void OnDestroy()
        {
            if (_text != null)
                Destroy(_text);
        }
        void OnEnable()
        {
            ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
        }
        void OnDisable()
        {
            ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
        }

        private void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item item)
        {
            if (item == null)
            {
                Text.gameObject.SetActive(false);
                return;
            }

            Text.gameObject.SetActive(true);
            Text.transform.SetParent(uiInstance.LayoutParent);
            Text.transform.SetSiblingIndex(1);
            Text.transform.localScale = Vector3.one;

            // 提升 quality 和 qualityColor 的作用域
            int quality; // 移除默认值，将在 if/else 块中赋值
            Color qualityColor; // 移除默认值，将在 if/else 块中赋值
            string rarityText; // 将rarityText的声明提升到这里

            // 获取物品品质和对应的颜色
            if (ItemLevelHelper.IsItemLevelModInstalled())
            {
                object itemValueLevelObj = ItemLevelHelper.GetItemValueLevel(item);
                if (itemValueLevelObj != null)
                {
                    string itemValueLevelString = itemValueLevelObj.ToString();

                    switch (itemValueLevelString)
                    {
                        case "White":
                            quality = 0;
                            break;
                        case "Green":
                            quality = 1;
                            break;
                        case "Blue":
                            quality = 2;
                            break;
                        case "Purple":
                            quality = 3;
                            break;
                        case "Orange":
                            quality = 4;
                            break;
                        case "LightRed":
                            quality = 5;
                            break;
                        case "Red":
                            quality = 6;
                            break;
                        default:
                            quality = 0; // 默认值，或者您可以根据需要设置其他值
                            break;
                    }
                }
                else
                {
                    quality = 0; // 如果无法获取品质，给一个默认值
                }
                // 这里 GetItemValueLevelColor 可能会返回 null，需要处理
                Color? nullableQualityColor = null;
                if (itemValueLevelObj != null)
                {
                    nullableQualityColor = ItemLevelHelper.GetItemValueLevelColor(itemValueLevelObj);
                }
                qualityColor = nullableQualityColor ?? Color.white; // 如果为 null，使用默认白色
                rarityText = GetRarityText(quality);
            }
            else
            {
                quality = item.Quality;
                qualityColor = GetQualityColor(quality);
                rarityText = GetRarityText(quality); // 移除 string 关键字
            }

            string priceText;
            switch (LocalizationManager.CurrentLanguage)
            {
                case SystemLanguage.Russian:
                    priceText = $"Цена: ${item.GetTotalRawValue() / 2}";
                    break;
                case SystemLanguage.English:
                    priceText = $"Price: ${item.GetTotalRawValue() / 2}";
                    break;
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                default:
                    priceText = $"价格：${item.GetTotalRawValue() / 2}";
                    break;
            }

            Text.text = $"{rarityText} | {priceText}";
            Text.fontSize = 20f;
            Text.color = qualityColor;
        }

        private Color GetQualityColor(int quality)
        {
            return GetDefaultQualityColor(quality);
        }

        private Color GetDefaultQualityColor(int quality)
        {
            switch (quality)
            {
                case 0: return new Color(0.5f, 0.5f, 0.5f, 1f);           // 垃圾
                case 1: return Color.white;                          // 普通
                case 2: return new Color(0f, 1f, 0f, 1f);           // 绿色 - 优良
                case 3: return new Color(0f, 0.5f, 1f, 1f);         // 蓝色 - 精良
                case 4: return new Color(0.5f, 0f, 1f, 1f);         // 紫色 - 史诗
                case 5: return new Color(1f, 0.5f, 0f, 1f);         // 橙色 - 传说
                default: return new Color(1f, 0f, 0f, 1f);          // 红色 - 神话+
            }
        }

        private string GetRarityText(int quality)
        {
            switch (LocalizationManager.CurrentLanguage)
            {
                case SystemLanguage.Russian:
                    switch (quality)
                    {
                        case 0: return "Мусор";
                        case 1: return "Обычный";
                        case 2: return "Отличный";
                        case 3: return "Редкий";
                        case 4: return "Эпический";
                        case 5: return "Легендарный";
                        case 6: return "Мифический";
                        default: return "Мифический+";
                    }
                case SystemLanguage.English:
                    switch (quality)
                    {
                        case 0: return "Trash";
                        case 1: return "Common";
                        case 2: return "Fine";
                        case 3: return "Rare";
                        case 4: return "Epic";
                        case 5: return "Legendary";
                        case 6: return "Mythic";
                        default: return "Mythic+";
                    }
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                default:
                    switch (quality)
                    {
                        case 0: return "垃圾";
                        case 1: return "普通";
                        case 2: return "精良";
                        case 3: return "稀有";
                        case 4: return "史诗";
                        case 5: return "传说";
                        case 6: return "神话";
                        default: return "神话+";
                    }
            }
        }
    }

    public static class ItemLevelHelper
    {
        public static bool IsItemLevelModInstalled()
        {
            try
            {
                // 尝试访问ItemLevelAndSearchSoundMod的类型来检查是否安装
                System.Type modType = System.Type.GetType("ItemLevelAndSearchSoundMod.Util, ItemLevelAndSearchSoundMod");
                return modType != null;
            }
            catch
            {
                return false;
            }
        }

        // 获取稀有度对应的颜色
        public static Color? GetItemValueLevelColor(object itemValueLevel) // 允许返回 null
        {
            try
            {
                // 调用ItemLevelAndSearchSoundMod的Util.GetItemValueLevelColor方法
                System.Type utilType = System.Type.GetType("ItemLevelAndSearchSoundMod.Util, ItemLevelAndSearchSoundMod");
                var method = utilType.GetMethod("GetItemValueLevelColor",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                if (method != null)
                {
                    Color? originalColor = (Color?)method.Invoke(null, new object[] { itemValueLevel });

                    if (originalColor.HasValue)
                    {
                        Color c = originalColor.Value;

                        // 设置不透明度拉满
                        c.a = 1f;

                        // 提升明度，使用HSV色彩空间更方便调整
                        float h, s, v;
                        // 确保使用 UnityEngine.Color.RGBToHSV
                        UnityEngine.Color.RGBToHSV(c, out h, out s, out v);

                        // 增加亮度，但不要超过1.0。可以根据需要调整 0.2f 的增量
                        v = UnityEngine.Mathf.Min(1.0f, v + 0.2f);

                        // 转换回RGB颜色
                        return UnityEngine.Color.HSVToRGB(h, s, v);
                    }
                    return originalColor; // 如果原始颜色为null，则返回null
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error getting item value level color: {ex.Message}");
            }
            return null; // 返回 null
        }

        // 获取物品的值等级
        public static object? GetItemValueLevel(Item item) // 允许返回 null
        {
            try
            {
                System.Type utilType = System.Type.GetType("ItemLevelAndSearchSoundMod.Util, ItemLevelAndSearchSoundMod");
                var method = utilType.GetMethod("GetItemValueLevel",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                if (method != null)
                {
                    return method.Invoke(null, new object[] { item });
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error getting item value level: {ex.Message}");
            }
            return null;
        }
    }
}