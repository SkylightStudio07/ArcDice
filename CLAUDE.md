# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**ArcBoard / DICEPAN** — Unity 6 2D 로컬 핫시트 보드배틀 게임 (키보드 2인). 전체 기획은 `@Assets/Docs/DICEPAN_GDD.md` 참조.

## Unity Version

Unity **6000.3.13f1** (Unity 6 LTS)

## Build & Test Commands

**에디터 열기:**
```
"C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe" -projectPath .
```

**EditMode 테스트 (headless):**
```
Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -testResults results.xml -logFile test.log -quit
```

**Windows 빌드:**
```
Unity.exe -batchmode -projectPath . -buildWindowsPlayer build\ArcBoard.exe -logFile build.log -quit
```

## Architecture

턴 루프 전체는 `GameManager` 싱글톤의 상태머신이 주도한다. 모든 게임 흐름이 이 단일 진입점을 통과한다.

### 핵심 시스템

| 클래스 | 역할 |
|---|---|
| `GameManager` | 싱글톤 상태머신 — TurnStart → DiceRoll → Move → NodeEffect → CombatCheck → VictoryCheck |
| `BoardManager` | `List<Transform>` 노드 배열 소유, 인덱스 기반 플레이어 이동. **Tilemap 미사용** |
| `NodeData` | ScriptableObject — 노드 유형(Normal/Boost/Trap/Warp/Battle) 및 효과 타입 enum |
| `CombatManager` | PvP + AI 중립몹 전투 처리 — 선공 결정, 공격/방어/회피 페이즈, HP 체크 |
| `ItemManager` | ScriptableObject 기반 아이템, 보유 상한 3개, 전투 개시 직전 1개 사용 |
| `PlayerState` | MonoBehaviour 아님 — HP·깃발 수·아이템 목록 보유 순수 데이터 클래스 |

### 씬 구조

- `BoardScene` — 메인 씬. 보드 + HUD 오버레이
- 전투는 패널 오버레이 또는 별도 씬으로 처리 (미확정). 씬 전환 시 보드 상태는 static hold

### Input

- New Input System (`com.unity.inputsystem` 1.19.0) 사용
- P1: `Space` / `Left Shift`, P2: `Enter` / `Right Shift`
- 핫시트 방식 — 매 턴 한 명만 입력 활성화, 동시 키 충돌 없음

### 렌더링

- URP 2D (`com.unity.render-pipelines.universal` 17.3.0)
- 보드는 2D 스프라이트를 45° 기울여 아이소메트릭 연출 — Unity Isometric Tilemap **미사용**

### 주요 패키지

- `com.unity.2d.aseprite` 3.0.1 — Aseprite 스프라이트 워크플로우
- `com.unity.2d.animation` 13.0.4
- `com.unity.test-framework` 1.6.0
