# 夜园守卫 · NightGarden

夜园守卫是一款用 Unity 和 C# 开发的 2D 俯视角单人波次生存游戏。你可以选择一位守卫和一把初始武器，在夜晚的花园里躲开怪群、收集种子金币，并在波次之间升级属性、购买装备。

## 玩法

- **自动战斗**。移动角色拉开距离或绕开怪群，装备的武器会自动攻击范围内的敌人。不同武器有刺击、挥击、穿透、爆炸、减速或孢子传染等效果。
- **波次生存**。刷怪节奏随波数和本波进度加快。第一波持续 30 秒，后续每波增加 5 秒，最长 60 秒。每 10 波会出现 Boss。
- **装备和升级**。击杀敌人获得经验，拾取掉落的金币可在波间商店购买武器和饰品。最多可装备 6 把武器，两把同种同级的武器可以合成，最高 4 级。商店商品可以锁定，刷新时会保留。
- **角色和记录**。目前有萝卜骑士、南瓜卫士、蘑菇巫师、辣椒游侠、豌豆射手五位角色。完成第 5 波解锁辣椒游侠，完成第 10 波解锁豌豆射手。排行榜和角色解锁进度保存在本机。

## 操作

| 操作 | 按键 |
| --- | --- |
| 移动 | `W`、`A`、`S`、`D` 或方向键 |
| 暂停 | `Esc`，也可点击战斗界面上的暂停按钮 |
| 选择角色、购物和升级 | 鼠标左键 |

## 在 Unity 中运行

1. 使用 Unity Hub 添加本仓库的根目录，建议使用项目当前版本 **Unity 2022.3.62f3c1**。
2. 等待 Unity 导入资源和安装项目依赖。
3. 打开 [LoginScene](Assets/Scenes/LoginScene.unity)，点击 **Play**。注册或登录本地账号后，进入主菜单，选择守卫和初始武器开始游戏。

## 构建 Windows 版本

1. 在 Unity 中打开 **File → Build Settings**，选择桌面平台，目标系统设为 **Windows**，架构选 **x86_64**。
2. 确认场景列表中的顺序为 `LoginScene`、`BeginScene`、`GameScene`，并且三个场景都已勾选。
3. 点击 **Build**，将结果输出到仓库根目录下的 `Builds/`。分发时打包整个构建目录，保留可执行文件、同名的 `_Data` 目录和随构建生成的文件。

游戏用 [ResManager](Assets/Scripts/Components/ResManager.cs) 从 `Assets/StreamingAssets` 读取 AssetBundle，目录中的 `PC` 是主清单包。构建玩家程序会复制这些包。如果改动了已标记进 AB 的预制体或图集，需要先重建相关包并同步清单，再构建玩家程序。

## 项目结构

| 路径 | 内容 |
| --- | --- |
| `Assets/Scenes` | 登录、主菜单和战斗场景 |
| `Assets/Scripts/Game` | 波次、角色、商店、武器、敌人和弹体逻辑 |
| `Assets/Scripts/UI` | 界面与面板生命周期 |
| `Assets/Scripts/Components` | 资源加载、对象池、音频、事件和数据存取 |
| `Assets/Resources/ScriptableObjects` | 角色、武器、敌人、饰品及升级属性配置 |
| `Assets/Resources/GameObject` | 玩家、敌人、武器等运行时预制体的源资源 |
| `Assets/Art`、`Assets/Audio` | 美术、界面、音乐和音效资产 |
| `Assets/StreamingAssets` | 运行时加载的 AssetBundle 和包清单 |

角色和武器数值以当前的 `ScriptableObject` 配置为准。需要批量生成配置时，可以用 [ScriptableEditor](Assets/Editor/ScriptableEditor.cs) 提供的 `Tools → Generate…` 菜单。生成器读取对应 JSON，更新现有配置前应先核对输入数据。
