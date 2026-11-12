# 美术资源目录

- `Heroes`：五名主角的纹理、游戏预制体/动画，以及选人界面用的16帧完整图和头像图集。
- `Enemies`：普通敌人、精英、Boss的预制体、动画、纹理和敌方弹幕。
- `Accessories/Atlases`：基础饰品与扩展饰品图集。
- `Combat/Atlases`：四张战斗图集，均已配置 Sprite 切片。
- `UI`：按钮、边框、面板组件、属性图标、背景和受击等UI图片。
- `Environment`：场地、障碍、拾取物等纹理。
- `VFX`：通用粒子预制体、材质、粒子纹理和Shader。
- 音频独立位于 `Assets/Audio`；美术辅助运行时脚本位于 `Assets/Scripts/ArtSupport`。

## 四张战斗图集

| 文件 | 内容 |
| --- | --- |
| `weapons.png` | 16种武器 |
| `projectiles.png` | 12种弹体，包括种子、豌豆、水泡、箭矢、蜂蜜、孢子、南瓜等 |
| `weapon_effects.png` | 8种已有发射特效，以及新增的棒/镰/锤/鞭挥击、刀/叉刺击、持续火焰、提灯雾气，共16种 |
| `projectile_effects.png` | 16种命中/爆炸特效 |

新增特效为静态 Sprite，可用缩放、旋转、透明度或粒子系统驱动，不是已经接入游戏的特效逻辑。

`Combat/weapon_catalog.json` 对应 Excel 的16个武器ID、弹体别名和特效ID；也列出四张图集的GUID、Sprite名称、fileID和矩形坐标。
例如 Excel 的 `gun/gun2/cannon/wand/staff` 分别对应美术切片 `chili_gun/pea_gun/water_cannon/honey_wand/spore_staff`；`bullet0` 对应 `projectile_00`，`bullet1` 对应 `projectile_01`。
喷火是持续范围效果，使用 `flame_stream`，不生成普通飞行子弹。

## UI与角色

攻击范围属性图标已加入 `UI/Textures/stats.png`，切片名为 `attack_range`，与力量等属性图标统一使用；独立源图 `UI/Icons/attack_range.png` 仍保留。
力量（统一百分比增伤）图标已追加到 `stats.png`，切片名为 `damage`；原有9个属性切片的GUID、fileID和像素保留。
四级暗红色边框位于 `UI/Components`：`weapon_frame_4.png`（武器）、`accessory_frame_legendary.png`（饰品）、`shop_card_accessory_legendary.png`（商店卡片）。沿用同类图片尺寸与九宫格边距，等级到边框的选择逻辑尚未接入。
`UI/Backgrounds/starting_weapon_reference.png` 仍被现有界面引用，因此保留，未当作未使用的演示图删除。
角色选人动画位于 `Heroes/UI`，每张图为4×4共16帧；完整人物和小头像分别使用 Animation16、Portrait16。

## 数据约定

`Assets/Resources/Excel/Datas.xlsx` 包含 Weapon、Role 和 Rules。Weapon已合并公共信息与等级数据，共57行；每行对应一种武器的一个等级，可直接生成WeaponData资产，不再需要关联WeaponLevelData页。
字段按当前RoleData、WeaponData命名：weaponID与整数level共同标识一行；武器基础伤害为整数atk（1–40），四级梯度已拉开；internalTime为秒，击退repel配置0–8，范围range配置4–35。damageType使用STR/AST枚举名称。
Range是小整数范围值，实际距离为 `Range × 范围系数 × 角色Range`；Repel是小整数击退强度，实际击退距离为 `Repel × 击退系数`。两个系数由代码分别设置，不再按厘米除以100。Rules中的爆炸、弹射和孢子传播范围也使用范围系数。
角色STR/AST为整数物理/魔法基础加伤，damage为全角色初始1的统一增伤倍率。最终伤害为 `(角色STR或AST + 武器atk) × 角色damage`；+10%、再+20%时damage为1.3。移除了源码中不存在的近战/远程倍率列，特殊角色被动后续单独实现。range、atkSpeed以1为中性值；drain是概率，recovery是每秒回血整数，初始均保留0。详细规则见Rules。
当前数值是初始平衡配置，尚未经过战斗实测。

## 加载与维护

本次整理保留已迁移资源的 .meta 和GUID，维持现有场景、预制体与动画引用；不改变 Resources 中现有UI预制体的加载路径。
UI原有的武器图集与战斗武器图集像素、切片及导入设置完全相同，已合并为 `Combat/Atlases/weapons.png`，并更新原UI引用；Sprite的fileID保持一致。
目录分类与AB包分组是两件事。当前未新建或重打AB包，新增战斗素材还需要在接入时指定加载和打包方式。
后续不要把同一素材同时复制进Resources和AB包；预制体可以直接引用这里的Sprite，再按实际加载需求分包。

演示网页、未引用的截图、旧导入器/清单和未引用的完全重复文件已移出Assets。原素材和表格备份、迁移清单位于项目根目录 `.codex-artifacts/resource-cleanup`。
