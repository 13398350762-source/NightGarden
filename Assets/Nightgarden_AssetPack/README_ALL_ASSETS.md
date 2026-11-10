# 夜园守卫：Unity 整合素材包

整合日期：2026-11-10。Unity 2022.3.62f3c1。

直接导入 Nightgarden_AllAssets.unitypackage，资源位于 Assets/Nightgarden_AssetPack。贴图切片、UI 九宫格边距、动画和预制体引用已保存在包内，不必再次运行生成器。

- GeneratedV32/Characters：5 名空手主角、26 种敌人（含普通、精英和 Boss），31 个角色预制体、65 个动画片段。
- GeneratedV32/Particles：16 个粒子特效预制体。
- GeneratedV32/Projectiles：6 种敌人球形弹幕预制体；敌人分类见 Animations/enemy_behaviors.json。
- Combat：16 把武器、对应弹体、发射与命中特效；对应关系见 weapon_manifest.json。
- Accessories 与 Accessories_Expansion20：共 26 件饰品，已切片。
- UI_DemoKit：15 张页面布局参考、30 个独立组件、属性图标、种子货币和受伤特效。Screens 是布局参考图，正式界面请使用 Components、Art 的 Sprite 与运行时文字组合。
- Textures：场景、障碍、拾取物及额外静态图集。
- Audio：4 段音乐、21 个音效、4 段短提示乐，共 29 个音频文件。音乐循环在 AudioSource 上开启。
- Unity：现有运行时辅助脚本、导入器和粒子 Shader。

打开 GeneratedV32/AnimationPreview.unity 可查看角色动画预览。所有数值和界面文本均为模板，登录注册等页面不包含后端服务。此包是素材与辅助组件合集，不是完整可玩的游戏工程。

原 v2 版本与旧版持武器主角图集未纳入。UI 使用的头像、武器图标等可能与游戏图集同源，但保留了各自切片和用途。
