# ArcDice

Unity 6 2D 로컬 핫시트 보드배틀 게임 (최대 4인, 키보드)

> 100% Orange Juice / 마리오 파티에서 영감을 받은 주사위 기반 보드배틀.  
> 주사위를 굴려 보드를 돌며 깃발 3개를 먼저 모으면 승리.

---

## 기술 스택

- **Unity 6000.3.13f1** (Unity 6 LTS)
- URP 2D (`com.unity.render-pipelines.universal` 17.3.0)
- New Input System (`com.unity.inputsystem` 1.19.0)
- TextMeshPro, Aseprite 워크플로우

---

## 핵심 규칙

| 항목 | 내용 |
|---|---|
| 승리 조건 | 깃발 3개 선취 |
| 깃발 획득 | 보드 한 바퀴 완주 +1 / PvP 전투 승리 시 강탈 |
| 전투 HP | 6 (매 전투 리셋 X, 부활 시스템 있음) |
| 아이템 상한 | 3개 |
| 조작 | 전원 Space로 주사위 굴림 (핫시트) |

---

## 현재 구현 상황

### 보드 / 이동
- [x] `BoardManager` — `List<NodeBehaviour>` 인덱스 기반 루프 보드
- [x] `NodeBehaviour` + `Node` ScriptableObject (Normal / Boost / Trap / Warp / Battle)
- [x] 플레이어 노드 간 점프 이동 애니메이션 (sin 아크)
- [x] 시작 칸 통과 시 깃발 +1

### 턴 / 입력
- [x] `GameManager` 상태머신 (TurnStart → DiceRoll → Move → CombatCheck → EndTurn)
- [x] 핫시트 Space 입력
- [x] 주사위 굴림 애니메이션 (스프라이트 슬로우다운)

### 전투
- [x] 같은 노드 충돌 시 PvP 전투 발생
- [x] 배틀 인트로 패널 (방어자 포트레잇 2.5초)
- [x] 배틀 페이즈 — 선공 결정 (동률 재굴림) + 2라운드 공방
- [x] 피해 계산 (공격값 - 방어값, 초과분만 피해)
- [x] 승패 결과 표시 (Win! / Lose! + 캐릭터 스프라이트 전환)
- [x] HP 0 → 부활 시스템 (랜덤 임계값, 성공 시 풀 HP 복귀 + 즉시 이동)

### 캐릭터 / UI
- [x] `CharacterData` ScriptableObject (idle / rollDice / battleStart / damaged / lose / win / victory)
- [x] `PlayerBehaviour` — HP, 깃발, 아이템 보유 (`PlayerState`)
- [x] HUD — 턴 표시, 플레이어 HP / 이름 / 포트레잇
- [x] 주사위 결과 패널 (P1 / P2 개별)
- [x] 부활 패널 (주사위 애니메이션 포함)
- [x] 카메라 드래그 + 백그라운드 경계 클램프

---

## 남은 개발 사항

### 필수
- [ ] **캐릭터 2번 완성** — CharacterData SO 스프라이트 전체 할당
- [ ] **플레이어 3 / 4 UI** — HUD, 주사위 패널, 배틀 인트로 확장
- [ ] **노드 20칸 배열** — 씬에 배치 및 BoardManager Inspector 연결
- [ ] **노드 이벤트 처리** — Boost(추가 굴림) / Trap(1턴 쉼) / Warp(랜덤 이동) / Battle(AI 중립몹)
- [ ] **최종 승리 처리** — 깃발 3개 달성 시 게임 종료 + 승리 화면
- [ ] **타이틀 씬** — 게임 시작 진입점

### 추가
- [ ] **캐릭터 선택창** — 게임 시작 전 캐릭터 배정 UI
- [ ] AI 중립몹 전투 (Battle 노드)
- [ ] 아이템 4종 구현 + 전투 개시 전 사용 UI
- [ ] 깃발 강탈 로직 (PvP 승리 시)
- [ ] PvP 전투 후 HP HUD 갱신 연동
- [ ] 사운드 / 추가 애니메이션 폴리싱

---

## 씬 구조

- `BoardScene` — 현재 작업 중인 메인 씬 (보드 + HUD)
- `TitleScene` — 미구현
- `CharacterSelectScene` — 미구현

## 어셈블리

`Assets/Scripts/ArcBoard.asmdef` — `Unity.InputSystem`, `Unity.TextMeshPro` 참조
