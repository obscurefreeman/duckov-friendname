# Duckov Modding 示例 (Duckov Modding Example)

*This is an example project for modding Escape From Duckov. I didn't have enough time to translate this document, so it's currently Chinese only. I'll apprecieate it if anyone wants to help with the translation work!*

## 工作原理概述

《逃离鸭科夫》的Mod模块会扫描并读取 Duckov_Data/Mods 文件夹中的各个子文件夹，以及 Steam 创意工坊已订阅物品的各个文件夹。通过文件夹中包含的dll文件，info.ini和preview.png在游戏中展示、加载mod。

《逃离鸭科夫》会读取info.ini中的name参数，并以此作为namespace尝试加载名为ModBehaviour的类。例如，info.ini 中如果记载`name=MyMod`,则会加载`MyMod.dll`文件中的`MyMod.ModBehaviour`。

ModBehaviour 应继承自 Duckov.Modding.ModBehaviour。Duckov.Modding.ModBehaviour 是一个继承自 MonoBehaivour 的类。其中还包含了一些mod系统中需要使用的额外功能。在加载 mod 时，《逃离鸭科夫》会创建一个该 mod 的 GameObject 并通过调用 GameObject.AddComponent(Type) 的方式创建一个 ModBehaviour 的实例。Mod作者可以通过编写 ModBehaviour 的 Start\Update 等 Unity 事件实现功能，也可以通过注册《逃离鸭科夫》中的其他事件实现功能。

## Mod文件结构

将准备好的 Mod 文件夹放到《逃离鸭科夫》本体的 Duckov_Data/Mods 中，即可在游戏主界面的 Mods 界面加载该 Mod。
假设 Mod 的名字为"MyMod"。发布的文件结构应该如下：

- MyMod (文件夹)
    - MyMod.dll
    - info.ini
    - preview.png (正方形的预览图，建议使用256*256分辨率)

[Mod文件夹示例](DisplayItemValue/ReleaseExample/DisplayItemValue/)

### info.ini

info.ini 应包含以下参数:
- name (mod名称，主要用于加载dll文件)
- displayName (显示的名称)
- description（显示的描述）

info.ini 还可能包含以下参数:
- publishedFileId （记录本 Mod 在 steam 创意工坊的 id）

**注意：在上传Steam Workshop的时候，会复写info.ini。info.ini中原有的信息可能会因此丢失。所以不建议在info.ini中存储除以上项目之外的其他信息。**

## 配置 C# 工程

1. 在电脑上准备好《逃离鸭科夫》本体。
2. 创建一个 .Net Class Library 工程。
3. 配置工程参数。
    1. Target Framework
        - **TargetFramework 建议设置为 netstandard2.1。**
        - 注意删除TargetFramework不支持的功能，比如`<ImplicitUsings>`
    2. Reference Include
        - 将《逃离鸭科夫》的`\Duckov_Data\Managed\*.dll`添加到引用中。
        - 例：
        ```
        <ItemGroup>
            <Reference Include="E:\Program Files (x86)\Steam\steamapps\common\Escape from Duckov\Duckov_Win\Duckov_Data\Managed\*.dll" />
        </ItemGroup> 
        ```
3. 完成！现在在你Mod的Namespace中编写一个ModBehaivour的类。构建工程，即可得到你的mod的主要dll。

csproj文件示例：[DisplayItemValue.csproj](DisplayItemValue/DisplayItemValue.csproj)

## 其他

### Unity Package
使用 Unity 进行开发时，可以参考本仓库附带的 [manifest.json文件](UnityFiles/manifest.json) 来选择 package。

### 自定义游戏物品
- 可调用 `ItemStatsSystem.ItemAssetsCollection.AddDynamicEntry(Item prefab)` 添加自定义物品
- 可调用`ItemStatsSystem.ItemAssetsCollection.RemoveDynamicEntry(Item prefab)`将该mod物品移除
- 自定义物品的prefab上需要配置好TypeID。避免与游戏本体和其他MOD冲突。
- 进入游戏时如果未加载对应MOD，存档中的自定义物品会直接消失。

### 本地化
- 可调用 `SodaCraft.Localizations.LocalizationManager.SetOverrideText(string key, string value)` 来覆盖显示本地化文本。
- 可借助 `SodaCraft.Localizations.LocalizationManager.OnSetLanguage:System.Action<SystemLanguage>` 事件来处理语言切换时的逻辑


