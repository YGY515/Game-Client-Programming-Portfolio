## 코드 안티데시스 리빌드
> 출시한 게임의 보스전을 Unity 2D로 재구성해보는 프로젝트<br>
> 2025. 5 ~ 2025. 7 / 이후 프로젝트 최적화와 코드 수정 진행 중

<br>

### Controller
게임 내 주요 오브젝트를 조작하는 코드입니다.
- BossController: 보스의 상태를 관리합니다.
- CameraController: 카메라가 플레이어를 중심으로 위치하고, 치명타 시 줌 연출이 포함되어 있습니다.
- EnemyController: 보스가 소환하는 하위 몬스터를 조작하는 코드입니다.
- NpcController: NPC가 플레이어와 상호작용 하는 것을 관리합니다.
- PlayerCotroller: 입력키에 따른 플레이어의 이동과 무기 조작 등 전반적인 조작을 담당합니다.
<br>

### Manager
게임 내 주요 기능을 관리하는 코드입니다.
- BossPhaseManager: 보스의 페이즈를 관리하고, 필요 시 페이즈를 1단계 올리는 AdvancePhase 함수가 있습니다.
- DamageTextManager: 플레이어가 보스를 공격할 시 출력되는 데미지 숫자를 관리합니다.
- DialogueManager: NPC와 플레이어의 대사가 한 글자식 따각따각 출력되기 위해 사용한 코드입니다.
- EnemyPoolManager: 보스가 페이즈마다 소환하는 하위 몬스터를 풀에 관리하는 코드입니다.
- TimerManager: 각 페이즈마다 유효한 시간을 관리하고, 이를 게임 화면 가운데 상단에 출력합니다.
<br>

### UI
게임 내 플레이어와 보스의 체력을 담당하고, 체력바로 화면에 표시하기 위해 사용된 코드입니다.
- PlayerHealth, BossHealth: 플레이어와 보스의 체력을 담당하고 피격 시 스프라이트가 짧게 붉어지고 원래대로 돌아오도록 합니다.
- PlayerHealthUI, BossHealthUI: 이벤트 구독을 통해 플레이어와 보스의 체력바를 화면에 표시합니다.
- PlayerHealthWarning: 플레이어의 체력이 일정 이하로 떨어질 경우 화면에 경고창을 표시합니다.
<br>

### Weapon
플레이어가 사용하는 무기 코드로 전략 패턴을 사용했습니다.
- Baton: 플레이어가 사용하는 근거리 무기입니다.
- Gun: 플레이어가 사용하는 원거리 무기입니다.
- IWeapon: 인터페이스로 무기 공격력과 공격 함수를 선언하고, 각 무기에서 상속받도록 하였습니다.
- WeaponManager: 플레이어가 공격키를 누를 시 사용되는 UseWeapon과, tab키를 누를 시 무기가 교체되는 ChangeWeapon 함수가 있습니다.
