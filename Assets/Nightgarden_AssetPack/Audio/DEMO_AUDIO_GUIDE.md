# 夜园 Demo 音频

原有 4 首音乐、16 个短音效、4 个提示乐保持不变；只补 5 个短音效。
全部为原创程序合成 WAV（44.1 kHz / 16-bit PCM），新增音效为单声道，延续已有卡通合成风格。

## 武器与事件对应

| 用途 | 播放文件（相对于 Audio） | 触发时机 |
| --- | --- | --- |
| 枪械、豌豆射手 | SFX/08_shot.wav | 实际发射时 |
| 刀、镰刀、鞭等轻型近战 | SFX/09_slash.wav | 挥动开始 |
| 通用子弹/轻近战命中 | SFX/10_hit.wav | 命中目标 |
| 锤、重棒等重型近战 | SFX/21_heavy_strike.wav（新增） | 命中时，替代通用命中声 |
| 弓、弩 | SFX/22_bow_release.wav（新增） | 发射时 |
| 箭矢命中 | SFX/23_arrow_impact.wav（新增） | 命中时，替代通用命中声 |
| 子弹撞墙、硬甲跳弹 | SFX/24_bullet_ricochet.wav（新增） | 确实发生硬物撞击/跳弹时 |
| 大弹体或敌方子弹掠过 | SFX/25_projectile_whizz.wav（新增） | 靠近玩家时播放一次，可选 |
| 法杖、能量弹、晶体武器 | SFX/14_magic.wav | 施放时；命中共用 10_hit |
| 炸弹、爆炸弹 | SFX/13_explosion.wav | 爆炸时 |
| 怪物受伤 | SFX/11_hurt.wav | 受到伤害；普通、精英、Boss 共用 |
| 怪物死亡 | SFX/12_death.wav | 死亡时 |

枪声就是发射音效，不必让每颗子弹飞行时再持续发声。弓弩无需枪口火焰，但仍有弦响。
同一命中事件优先播放命中声或受伤声之一，避免每次伤害叠加多层。死亡时用死亡声替代受伤声。
普通怪受伤可以略抬高 pitch，Boss 略降低；Demo 阶段无需每种怪单独录制。

## 背景音乐与提示乐（全部复用）

| 场景 | 文件 |
| --- | --- |
| 主菜单 | Music/music_01_moonlit_menu.wav |
| 常规战斗 | Music/music_02_garden_patrol.wav |
| Boss 战 | Music/music_03_thorn_siege.wav |
| 商店/升级选择 | Music/music_04_seed_shop.wav |
| Boss 登场、波次、胜利、失败 | Stingers/17_boss.wav 至 20_lose.wav |

拾取、金币、回血、升级、按钮、闪避、宝箱等继续使用原有 SFX/01–07、15–16。
最小 Demo 先接入常规战斗音乐和实际使用的武器音效，其他音乐按场景接入即可。

## Unity 接入建议

- 音乐独立 AudioSource，按清单启用 Loop；短音效禁用 Loop。
- 短音效可用 AudioSource.PlayOneShot；建议起始音量约 0.25–0.4，音乐约 0.15–0.25，按实际混音调整。
- 高频射击/受伤声按同类设置约 50–80 ms 的最短间隔，并限制同时发声数，避免怪群声音堆积。
- 这次交付为音频素材及对应说明，未修改游戏播放代码，未做游戏内混音验收。
- validation.json 记录文件格式、时长、非静音、无满幅削波及原文件未改动检查。
- Preview/demo_audio_additions.wav 按 21–25 的顺序串接试听，片段间留白。
