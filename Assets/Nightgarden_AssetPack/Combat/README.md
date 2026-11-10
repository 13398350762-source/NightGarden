# 武器视觉对应表

正式归档之前生成的 16 把武器及 12 个弹体、8 个发射特效、16 个命中特效，没有扩充武器数量。

放入 Unity Assets 后运行 Tools → Nightgarden → Import Weapon Visuals，或按 weapon_manifest.json 坐标切图。武器、弹体、枪口与命中效果按 ID 对应；弓弩 muzzle 为 null。音效 ID 对应主素材包 Audio/SFX 中现有文件。

近战使用挥动与命中特效；远程发射时显示 muzzle，并生成 projectile，命中时显示 impact。枪口、挥动轨迹、碰撞和伤害由游戏代码控制。本次补的是正式归档与配对数据，并未实现武器 AI。
