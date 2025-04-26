# Brawl_Animals
## Unity Script
Workspaces
    └── Seo_juwon/
         └── Scripts/
              ├── Character.cs  // 캐릭터 기본 스탯 및 데미지 처리
              ├── CharacterSpawner.cs  // 캐릭터 스폰 및 초기 설정
              └── Skill/
                  ├── Skill.cs  // Skill 인터페이스 정의
                  └── WaterSpray/
                      ├── WaterSpray.cs  // 물 분사 스킬 구현
                      └── WaterSprayController.cs  //
         └── Prefabs/
              ├── Player1.prefab         // 플레이어1 프리팹
              ├── Player2.prefab         // 플레이어2 프리팹
              └── Water.prefab           // 물 파티클 프리팹

### Character.cs
* Hp 관리, 사망 시스템
* 스킬 사용 기능 포함(UseSkill())
### CharacterSpawner.cs
* 게임 시작 시 자동으로 Player Character 스폰
