# PRD: Taekwondo Tech v2

**Author:** Norrie Taylor
**Date:** 2026-02-27
**Status:** Approved
**Version:** 1.1

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Problem Statement](#problem-statement)
3. [Goals & Success Metrics](#goals--success-metrics)
4. [User Stories](#user-stories)
5. [Functional Requirements](#functional-requirements)
6. [Non-Functional Requirements](#non-functional-requirements)
7. [Technical Considerations](#technical-considerations)
8. [Implementation Roadmap](#implementation-roadmap)
9. [Out of Scope](#out-of-scope)
10. [Open Questions & Risks](#open-questions--risks)
11. [Validation Checkpoints](#validation-checkpoints)
12. [Appendix: Task Breakdown Hints](#appendix-task-breakdown-hints)
13. [Revision History](#revision-history)

---

## Executive Summary

Taekwondo Tech v2 is a ground-up reimagining of a side-scrolling platformer where a young taekwondo martial artist collects robot parts to build the ultimate robot companion. Built in Unity (C#) targeting kids ages 6-12, v2 replaces the vanilla JS/Phaser prototype with a production-quality game featuring proper audio, hand-crafted sprite art, a narrative-driven progression system, and cross-platform deployment (WebGL, iOS, Android).

The v1 prototype validated core gameplay across 15+ PRs — combat, collection, 10 dragon costumes with unique elemental powers, a power-up queue system, banana bonus/survival modes, and a robot craft system. v2 takes ALL of these forward, reimagined with polished mechanics, a modular Unity codebase, and a kid-friendly experience.

---

## Problem Statement

### Current Situation (v1)
The v1 prototype is a Phaser.js browser game with:
- Monolithic code files (GameScene.js at 67K+ lines, CraftScene.js at 45K+ lines)
- No build system, no TypeScript, no module bundling — loaded via CDN script tags
- No audio whatsoever — completely silent gameplay
- Geometric/procedural graphics — no sprite sheets or proper animations
- 30 FPS target with no optimization pipeline
- 10 dragon costumes bolted on incrementally with inconsistent unlock logic
- iOS/iPad fullscreen hacks and workarounds throughout the codebase
- No narrative or story — levels feel unconnected

### User Impact
- **Who is affected:** Kids ages 6-12 playing the game
- **How they're affected:** Silent gameplay reduces engagement, inconsistent mechanics confuse young players, mobile experience is buggy, lack of story provides no motivation to continue
- **Severity:** High — the prototype has reached its ceiling and cannot be extended further without a rewrite

### Why Rebuild Now?
- v1 successfully validated the core concept: taekwondo combat + robot building + costume customization + banana modes
- The codebase is unmaintainable at its current scale (100K+ lines across 2 files)
- Unity provides a clear path to iOS/Android app stores with a single codebase
- The feature set is well-understood from 15+ iterations of the prototype

---

## Goals & Success Metrics

### Goal 1: Deliver a Polished, Kid-Friendly Platformer
- **Metric:** Game completion rate (% of players who finish the main story)
- **Baseline:** Unknown (v1 had no analytics)
- **Target:** 40% of players complete at least 3 levels
- **Timeframe:** 3 months post-launch
- **Measurement:** Unity Analytics events

### Goal 2: Cross-Platform Release
- **Metric:** Successful deployment to all target platforms
- **Target:** Ship on WebGL, iOS App Store, and Google Play Store
- **Timeframe:** Launch day
- **Measurement:** Approved and live on all platforms

### Goal 3: Engaging Audio-Visual Experience
- **Metric:** Average session duration
- **Baseline:** N/A
- **Target:** 10+ minutes average session for kids 6-12
- **Timeframe:** 1 month post-launch
- **Measurement:** Unity Analytics session tracking

### Goal 4: Maintainable, Modular Codebase
- **Metric:** No single script file exceeds 500 lines
- **Target:** Clean architecture with separation of concerns from day one
- **Measurement:** Code review and linting rules

---

## User Stories

### Story 1: Core Platforming

**As a** young player,
**I want to** run, jump, and fight through colorful levels,
**So that I can** have fun and feel like a martial arts hero.

**Acceptance Criteria:**
- [ ] Player can move left/right with smooth acceleration
- [ ] Player can jump and double-jump with satisfying physics
- [ ] Player can perform punch and kick attacks with visual and audio feedback
- [ ] Player can stomp on enemies Mario-style with bounce effect
- [ ] Controls work equally well on keyboard, gamepad, and touchscreen
- [ ] All actions have corresponding sound effects

**Dependencies:** None (foundational)

---

### Story 2: Robot Part Collection & Building

**As a** young player,
**I want to** collect robot parts scattered through levels and assemble them,
**So that I can** build my own unique robot companion.

**Acceptance Criteria:**
- [ ] Robot parts spawn in levels with visual shimmer/glow
- [ ] Collecting a part plays a satisfying pickup animation and sound
- [ ] Parts have types: Head, Body, Arms, Legs, Power Core
- [ ] Parts have visual rarity (Common, Rare, Epic) indicated by color
- [ ] Craft mode lets player drag-and-drop parts onto a robot blueprint
- [ ] Assembled robot is visible as a companion during gameplay
- [ ] Progress saves between sessions
- [ ] Collecting all part types (1 of each) unlocks Legendary Mode

**Dependencies:** Story 1 (needs core platforming)

---

### Story 3: Dragon Costume System (All 10 + Legendary)

**As a** young player,
**I want to** unlock and wear different dragon costumes with unique elemental powers,
**So that I can** customize my character and use cool special attacks.

**Acceptance Criteria:**
- [ ] All 10 costumes available with unique visuals, animated wings, and elemental projectiles
- [ ] Legendary Mode unlockable as an 11th "fusion" form
- [ ] Each costume has a distinct special move with visual and audio effects
- [ ] Costume selection screen shows unlock conditions, previews, and "NEW!" badges
- [ ] Costume choice persists across sessions
- [ ] Unlock conditions are clear and trackable

**Dependencies:** Story 1, Story 5 (needs levels to unlock)

---

### Story 4: Power-Up Queue System

**As a** young player,
**I want to** collect and strategically activate power-ups,
**So that I can** feel powerful and overcome tough challenges.

**Acceptance Criteria:**
- [ ] Power-ups spawn in levels at designed locations
- [ ] Player can hold up to 2 queued power-ups (FIFO)
- [ ] Power-ups activate manually (E/Q on keyboard, button on mobile)
- [ ] When queue is full, new pickup replaces oldest item
- [ ] Active power-up shown with timer/indicator on HUD
- [ ] Visual and audio effects clearly communicate power-up state
- [ ] Power-up types: Speed Boost, Shield, Elemental Attack, Invincibility

**Dependencies:** Story 1

---

### Story 5: Story-Driven Level Progression

**As a** young player,
**I want to** follow a story as I progress through themed worlds,
**So that I** have a reason to keep playing and see what happens next.

**Acceptance Criteria:**
- [ ] Game has an overarching narrative told through brief cutscenes
- [ ] 5 themed worlds with unique enemies and a boss each
- [ ] Star rating (1-3 stars) on level completion based on performance
- [ ] World map shows progression and star counts
- [ ] Completing a world unlocks the next with a narrative transition

**Dependencies:** Story 1, Story 2

---

### Story 6: Banana Game Modes

**As a** young player,
**I want to** play fun banana-themed mini-games,
**So that I can** take a break from the main campaign and earn bonus rewards.

**Acceptance Criteria:**
- [ ] Banana Survival mode accessible from main menu (endless dodge/deflect challenge)
- [ ] Banana Bonus mode triggers randomly between levels (30-second timed challenge)
- [ ] Monkey Titan enemies throw bananas at the player
- [ ] Player can deflect bananas with kick/punch attacks
- [ ] 5 food types with unique properties: Banana, Pancake, Cherry, Waffle, Apple
- [ ] Food Power-Up collectibles temporarily change thrown food type
- [ ] Deflected bananas can hit enemies for bonus damage and points
- [ ] Banana Survival: 5-slip limit, wave system, scoring based on time + deflections + waves
- [ ] Banana Bonus: 3-slip limit, 30-second timer, bonus coins + chance for rare robot part
- [ ] Banana Dragon costume integrates with banana modes (banana trail, banana breath)

**Dependencies:** Story 1, Story 3

---

### Story 7: Audio & Music

**As a** young player,
**I want to** hear music and sound effects while playing,
**So that** the game feels alive and exciting.

**Acceptance Criteria:**
- [ ] Each world has a unique background music track
- [ ] Menu, craft mode, and banana modes have their own music
- [ ] Sound effects for: jump, land, punch, kick, stomp, collect, hurt, defeat enemy, power-up activate, level complete, banana slip, banana deflect, costume special moves
- [ ] Boss encounters have distinct music
- [ ] Volume controls in settings (music and SFX separate)
- [ ] Audio does not play when device is on silent/mute

**Dependencies:** None (can develop in parallel)

---

### Story 8: Mobile & Touch Controls

**As a** young player on a tablet or phone,
**I want to** play with intuitive touch controls,
**So that I can** enjoy the game without a keyboard.

**Acceptance Criteria:**
- [ ] Virtual joystick for movement (left side of screen)
- [ ] Action buttons for jump, attack, power-up activate, and special move (right side)
- [ ] Button size appropriate for children's fingers (minimum 44pt touch targets)
- [ ] Controls adapt to screen size and orientation
- [ ] Single-finger controls only — no multi-touch gestures required
- [ ] Haptic feedback on supported devices

**Dependencies:** Story 1

---

## Functional Requirements

### Must Have (P0) — Required for Launch

#### REQ-001: Player Controller
**Description:** Responsive 2D player character with movement, jumping, and combat.

**Specifications:**
- Horizontal movement with acceleration/deceleration curves
- Gravity-based jump with variable height (hold to jump higher)
- Double jump with distinct animation
- Punch attack (short range, 0.3s cooldown)
- Kick attack (medium range, stronger)
- Head stomp on enemies with upward bounce
- Invincibility frames after taking damage (1 second, visual flash)
- Health system: 3 hits before defeat (shown as hearts on HUD)

**Task Breakdown:**
- Player movement & physics: Medium (8h)
- Jump system with double jump: Small (4h)
- Combat system (punch, kick, stomp): Medium (8h)
- Health & damage system: Small (4h)
- Animation state machine: Medium (6h)
- Input abstraction (keyboard, gamepad, touch): Medium (6h)

**Dependencies:** None

---

#### REQ-002: Level System
**Description:** 5 themed worlds with levels, platforms, hazards, enemies, and collectibles.

**Specifications:**
- Levels built using Unity Tilemap
- Side-scrolling camera that follows player with parallax backgrounds (3 layers minimum)
- Platform types: static, moving, crumbling
- 5 World themes from v1:
  - **World 1: Ice World** — Slippery platforms, ice hazards, blue/white/silver palette
  - **World 2: Fire World** — Lava hazards, volcanic environment, red/orange/black palette
  - **World 3: Ultimate Power Bomb** — Mixed environments, elite enemies, purple/gold/electric blue
  - **World 4: Lightning World** — Electric platforms, chain lightning hazards, gold/purple palette
  - **World 5: Shadow World** — Dark theme, visibility mechanics, dark purple/black palette
- Enemy spawn points with configurable enemy types per world
- Collectible placement: coins, robot parts, power-ups
- Level end trigger with completion screen
- 3-star rating: based on parts collected (60%/80%/100%), enemies defeated, and damage taken

**Task Breakdown:**
- Tilemap setup and base level structure: Medium (6h)
- Camera system with parallax: Small (4h)
- Platform types (static, moving, crumbling): Medium (6h)
- Hazard system per world theme: Medium (8h)
- Level completion and star rating: Medium (6h)
- Level loading and transitions: Small (4h)
- World 1 levels: Large (12h)
- World 2 levels: Large (12h)
- World 3 levels: Large (12h)
- World 4 levels: Large (12h)
- World 5 levels: Large (12h)

**Dependencies:** REQ-001

---

#### REQ-003: Enemy System
**Description:** AI-controlled enemies with distinct behaviors per world theme.

**Specifications:**
- State machine AI: Idle, Patrol, Chase, Attack, Stunned, Defeated
- Enemy types per world (at least 2 regular + 1 boss per world):
  - **Ice World:** Ice Titans
  - **Fire World:** Fire Titans
  - **Ultimate Power Bomb:** Elite Titans + Boss
  - **Lightning World:** Lightning Titans
  - **Shadow World:** Shadow Titans
  - **Banana Modes:** Monkey Titans (banana-throwing specialist)
- Patrol enemies walk between waypoints
- Chase triggers when player enters detection radius
- Attack patterns vary by enemy type
- Stun state when stomped
- Boss enemies have multi-phase attack patterns
- Red indicator above enemy heads (visual threat marker from v1)
- **Monkey Titan** specific: throws bananas at player, 2500ms throw delay, 3 bananas per burst, 5000ms burst cooldown, 50 HP, stomps deal 75 damage

**Task Breakdown:**
- Base enemy with state machine: Medium (8h)
- Patrol and chase behaviors: Small (4h)
- Enemy attack patterns: Medium (6h)
- Monkey Titan (banana-throwing enemy): Medium (8h)
- Boss enemy framework: Large (10h)
- Per-world enemy variants (5 worlds): Large (10h per world)

**Dependencies:** REQ-001, REQ-002

---

#### REQ-004: Robot Building (Craft Mode)
**Description:** Between-level screen where players assemble collected robot parts.

**Specifications:**
- Craft mode accessible between levels and from world map
- Robot blueprint with 5 slots: Head, Body, Arms, Legs, Power Core
- Drag-and-drop parts onto blueprint slots
- Parts show rarity with visual distinction (Common, Rare, Epic — border color/glow)
- Preview of assembled robot with idle animation
- Robot companion appears in gameplay once partially built
- Clear visual indication of empty vs filled slots
- "Build" button with celebration animation when robot is complete
- Collecting all 5 part types unlocks Legendary Mode costume

**Task Breakdown:**
- Craft UI layout and blueprint: Medium (6h)
- Drag-and-drop system: Medium (6h)
- Part inventory display: Small (4h)
- Robot preview and assembly animation: Medium (6h)
- Robot companion in-game appearance: Medium (8h)

**Dependencies:** REQ-002

---

#### REQ-005: Dragon Costume System (All 10 + Legendary Mode)
**Description:** 10 unlockable dragon costumes, each with a unique elemental projectile and special ability, plus an 11th Legendary fusion mode.

**Costume Definitions (from v1 codebase):**

| # | Costume | Unlock Condition | Projectile | Damage | Speed | Special Effect |
|---|---------|-----------------|------------|--------|-------|----------------|
| 1 | **Default Gi** | Always available | None | — | — | Base costume, no wings |
| 2 | **Fire Dragon** | Complete Level 1 | Fireball | 20 | 500 | Burn trail on hit |
| 3 | **Ice Dragon** | Collect 5 robot parts | Ice shard | 15 | 550 | Freeze/slow enemies |
| 4 | **Lightning Dragon** | Complete Level 2 | Lightning bolt | 22 | 700 | Chain to nearby enemies |
| 5 | **Shadow Dragon** | Complete Level 4 | Smoke bomb | 18 | 350 | Projectile expands as it travels |
| 6 | **Earth Dragon** | Complete Level 5 | Earthquake rock | 25 | 400 | Screen shake on hit |
| 7 | **Banana Dragon** | Complete Level 1 | Banana | 18 | 450 | Enemies slip and fall; banana trail when running; banana breath |
| 8 | **Present Dragon** | Complete Level 3 | Present bomb | 60 (explode) | 400 | Bomb explodes → summons dragon ally (8s, 25dmg fireballs) |
| 9 | **Stone Dragon** | Complete Level 1 | Stone shard | 22 | 450 | T+S special: laser beam → transforms into exploding boulder (15+50 dmg) |
| 10 | **Dino Grimlock** | Complete Level 2 | Fire+Lightning breath | 28 | 600 | Press 2 to transform between robot/dino forms; L for duck laser special |

**Legendary Mode (11th — Ultimate Fusion):**
- **Unlock:** Collect all robot part types (head, body, arms, legs, powerCore)
- **Visuals:** 2.5x size, gold/rainbow colors, legendary wings, body parts mapped to each elemental dragon color
- **Power:** 5x fireball damage multiplier, cycles through all 6 elemental colors, 3-shot burst before cooldown

**Specifications:**
- Each costume changes the player's entire sprite set (body, wings, belt, eyes)
- Wings are animated with state-responsive animations (flapping on jump, flutter on run, breathing on idle)
- Wing styles per costume: flame, crystal, electric, shadow, stone, banana, festive, stone, mechanical, legendary
- Costume selection overlay in CraftScene with compact preview (fits all costumes on screen)
- "NEW!" badge on recently unlocked costumes
- Unlock progress shown on locked costumes
- Special move activated with dedicated button, has cooldown timer per costume
- Grimlock's transform mode switches between robot form (normal speed/jump) and dino form (slower, lower jump, 1.5x damage, 1.3x size)

**Task Breakdown:**
- Costume data model (ScriptableObjects): Small (4h)
- Costume selection UI (10 items + Legendary): Medium (8h)
- Sprite swapping and wing animation system: Medium (8h)
- Projectile system (base class + 8 projectile types): Large (16h)
- Fire Dragon special (burn trail): Small (3h)
- Ice Dragon special (freeze/slow): Small (3h)
- Lightning Dragon special (chain): Medium (5h)
- Shadow Dragon special (expanding smoke): Small (3h)
- Earth Dragon special (screen shake): Small (2h)
- Banana Dragon special (slip, banana trail, banana breath): Medium (6h)
- Present Dragon special (present bomb → explosion → dragon ally AI): Large (10h)
- Stone Dragon special (T+S laser-to-boulder combo): Medium (8h)
- Dino Grimlock special (transform + duck laser): Large (10h)
- Legendary Mode (size, multi-color, 5x damage, burst): Medium (6h)
- Unlock condition tracking and persistence: Small (4h)

**Dependencies:** REQ-001, REQ-002

---

#### REQ-006: Banana Game Modes
**Description:** Two banana-themed mini-game modes: Banana Survival (endless) and Banana Bonus (timed between-level challenge).

**Banana Survival Mode:**
- Accessible from main menu
- Jungle-themed arena (forest green background, wood platforms)
- 5 elevated platforms at varying heights + ground level
- Starts with 2 Monkey Titans, new monkey spawns every 8s (decreasing to min 4s)
- Banana spawn rate: 3000ms base, decreases by 50ms per banana, minimum 500ms
- 5-slip limit — game over on 5th slip
- Scoring: `(survivalTime × 10) + (bananasDeflected × 15) + (wavesSurvived × 100)`
- Stomping a Monkey Titan recovers 1 slip
- Game over screen shows: time, waves, deflections, slips, final score

**Banana Bonus Mode:**
- Triggers randomly between levels (40% after L1, 50% after L2, 60% after L3, 75% after L4+)
- Can be disabled in settings
- Indigo/purple themed arena with golden stars
- 30-second timed challenge with 3-2-1-GO countdown
- 2-4 Monkey Titans based on completed level: `min(2 + floor(level/2), 4)`
- 3-slip limit — fail on 4th slip
- Faster spawn rate: 2000ms base, 100ms ramp
- Scoring: `50 + (deflections × 3) - (slips × 10)` bonus coins
- Perfect run bonus (10+ deflections, 0 slips): earn a rare robot part
- Red flashing timer + camera shake when ≤5 seconds remain
- Results lead to CraftScene with next level

**Banana/Food Entity System:**
- 5 food types with unique physics:
  - **Banana:** 800ms slip, 300 velocity, 15 deflect points (default)
  - **Pancake:** 1000ms slip (stickiest), 200 velocity, 20 deflect points
  - **Cherry:** 600ms slip (quickest), 400 velocity (fastest), 25 deflect points
  - **Waffle:** 900ms slip, 250 velocity, 20 deflect points
  - **Apple:** 700ms slip, 350 velocity, 20 deflect points
- Bananas spawn from top with random trajectory or thrown by Monkey Titans at player
- Deflect mechanics: kick (400 force) or punch (300 force) reverses banana direction
- Deflected bananas can hit enemies for 20 damage + 50 bonus points
- Slip effect: squash animation, yellow stars burst, "SLIP!" text, temporary jump disable
- Splat effect on destroy: yellow ellipse + 8 particle spread
- Food Power-Up collectible: changes food type for 15 seconds, spawns every 8-13s

**Task Breakdown:**
- Banana entity with 5 food types: Medium (8h)
- Banana physics (spawn, deflect, slip, splat): Medium (8h)
- BananaManager (spawning, trajectory, food power-ups): Medium (6h)
- Monkey Titan enemy (throwing AI, burst cooldown): Medium (8h)
- Banana Survival scene (arena, waves, scoring, UI): Large (12h)
- Banana Bonus scene (timer, countdown, rewards, level integration): Large (10h)
- Banana Bonus trigger logic in level progression: Small (3h)

**Dependencies:** REQ-001, REQ-003

---

#### REQ-007: Power-Up Queue System
**Description:** Collectible power-ups with queue-based manual activation.

**Specifications:**
- Power-up types:
  - **Speed Boost:** 2x movement speed for 8 seconds
  - **Shield:** Absorb 1 hit without taking damage, lasts 15 seconds or until hit
  - **Elemental Attack:** Costume-themed ranged attack for 10 seconds
  - **Invincibility:** Full invincibility for 5 seconds (sparkling visual)
- Queue holds 2 power-ups (FIFO — oldest activates first)
- Manual activation via dedicated button (E/Q on keyboard, button on mobile)
- When queue is full, new pickup replaces oldest queued item
- Active power-up displays countdown on HUD
- Visual effects clearly show active power-up state

**Task Breakdown:**
- Power-up base class and types: Medium (6h)
- Queue system with UI: Medium (6h)
- Activation logic and timers: Small (4h)
- Visual effects per power-up: Medium (8h)

**Dependencies:** REQ-001, REQ-008

---

#### REQ-008: HUD & UI
**Description:** In-game heads-up display and menu system.

**Specifications:**
- In-game HUD: health hearts, score, coin count, power-up queue (2 slots), active power-up timer, banana mode stats (slips, timer, deflections)
- Pause menu: Resume, Settings, Quit to Menu
- Main menu: Start Game, Continue, Banana Survival, Settings, Credits
- World map: themed nodes showing level stars and lock state
- Level complete screen: star rating, parts found, score, "Next Level" button
- Game over screen: "Try Again" and "Quit" options
- Banana mode results screens with stats and rewards
- All UI minimum 44pt touch targets
- UI scales correctly across resolutions (Canvas Scaler)
- Fullscreen toggle (F key on desktop)

**Task Breakdown:**
- HUD layout and components: Medium (6h)
- Main menu with banana survival access: Small (4h)
- World map screen: Medium (8h)
- Pause and settings menus: Small (4h)
- Level complete and game over screens: Small (4h)
- Banana mode results screens: Small (4h)
- UI scaling and responsiveness: Small (3h)

**Dependencies:** REQ-001

---

#### REQ-009: Save System
**Description:** Persistent save system that works across all target platforms.

**Specifications:**
- Auto-save after level completion and craft mode changes
- Save data: level progress, star ratings, robot parts inventory, costume unlocks, active costume, power-up queue, settings (volume, banana bonus enabled), high scores for banana modes
- Platform-appropriate storage (PlayerPrefs for mobile, IndexedDB for web)
- Single save slot (simplicity for kids)
- "Continue" option on main menu when save exists
- "New Game" with confirmation prompt if save exists

**Task Breakdown:**
- Save data model (serializable): Small (3h)
- Save/load logic with platform abstraction: Medium (6h)
- Auto-save triggers: Small (2h)
- Menu integration (Continue / New Game): Small (3h)

**Dependencies:** REQ-004, REQ-005

---

#### REQ-010: Audio System
**Description:** Complete audio with music, sound effects, and player controls.

**Specifications:**
- Background music per world (looping, crossfade between scenes)
- Menu theme music, craft mode music, banana mode music (jungle theme)
- Boss encounter music (overrides world music)
- Victory and defeat jingles
- Sound effects (minimum 20): jump, land, punch, kick, stomp, enemy hit, enemy defeat, collect coin, collect part, power-up pickup, power-up activate, player hurt, player defeat, level complete, UI button press, banana slip, banana deflect, costume special move fire, transform (Grimlock), present bomb explode
- Settings: separate music and SFX volume sliders
- Respect device silent mode

**Task Breakdown:**
- Audio manager (music + SFX): Medium (6h)
- Music integration per scene: Small (4h)
- SFX integration for all actions: Medium (8h)
- Volume controls UI: Small (3h)
- Audio asset sourcing/creation: Large (12h)

**Dependencies:** None (parallel track)

---

### Should Have (P1) — Important, Not Blocking Launch

#### REQ-011: Cutscene System
**Description:** Panel-based illustrated cutscenes for story delivery between worlds.

**Specifications:**
- Panel-based cutscenes (illustrated stills with text)
- Tap/click to advance panels
- Skip button for replays
- Cutscenes trigger: game intro, between worlds, final ending
- Minimum 6 cutscenes (intro + 5 world transitions)

**Task Breakdown:**
- Cutscene player (panel display, text, advance): Medium (6h)
- Cutscene data format and loading: Small (4h)
- Art assets for cutscenes: Large (16h)
- Integration with level flow: Small (2h)

**Dependencies:** REQ-002

---

#### REQ-012: Gamepad Support
**Description:** Native controller support for desktop and console-like experience.

**Specifications:**
- Support standard gamepad layout (Xbox/PlayStation)
- D-pad or left stick for movement
- A/Cross for jump, X/Square for punch, Y/Triangle for kick
- Bumpers for power-up activation
- Triggers for costume special move
- Start for pause
- Auto-detect gamepad connection
- UI navigation with gamepad

**Task Breakdown:**
- Input system gamepad mapping: Medium (6h)
- UI navigation with gamepad: Small (4h)
- Testing across controller types: Small (3h)

**Dependencies:** REQ-001

---

#### REQ-013: Achievements / Progress Tracking
**Description:** Visible achievement system that rewards exploration and mastery.

**Specifications:**
- Achievement categories: Combat, Collection, Costume, Banana, Mastery
- Examples: "Defeat 100 enemies," "Unlock all costumes," "Survive 60 seconds in Banana Survival," "Complete a level without taking damage," "Deflect 50 bananas"
- Achievement notification popup during gameplay
- Achievement gallery accessible from main menu
- At least 20 achievements at launch
- Achievements save to profile

**Task Breakdown:**
- Achievement system and data model: Medium (6h)
- Achievement UI (popup, gallery): Medium (6h)
- Define and implement 20 achievements: Medium (8h)
- Save integration: Small (2h)

**Dependencies:** REQ-009

---

### Nice to Have (P2) — Post-Launch

#### REQ-014: Time Trial Mode
**Description:** Speed-run mode for completed levels with leaderboard.

**Dependencies:** REQ-002

---

#### REQ-015: Online Leaderboards
**Description:** Per-level and banana mode leaderboards via Game Center / Google Play Games.

**Dependencies:** REQ-006, REQ-014

---

## Non-Functional Requirements

### Performance
- **Frame Rate:** 60 FPS on target devices (mobile: iPhone SE 2nd gen+, midrange Android)
- **Load Time:** Initial load < 5 seconds on 4G, level transitions < 2 seconds
- **WebGL Build Size:** < 30MB initial download, assets streamed as needed
- **Memory:** < 300MB RAM on mobile devices
- **Battery:** Object pooling for projectiles, particles, and bananas; < 10 draw calls per frame for UI

### Security
- Save data integrity (checksum to prevent trivial tampering)
- No server-side components at launch (offline-first)
- No collection of personal data (COPPA compliance for kids under 13)

### Compatibility
- **WebGL:** Chrome, Safari, Firefox, Edge (last 2 versions)
- **iOS:** iOS 15+ (iPhone SE 2nd gen and newer, iPad)
- **Android:** Android 10+ (API 29+)
- **Screen Sizes:** 320px to 2560px width
- **Orientation:** Landscape-preferred with landscape lock on mobile

### Accessibility
- Large, clear UI elements (minimum 44pt touch targets)
- High contrast text and UI elements
- Colorblind-friendly design (don't rely on color alone for information)
- Single-finger controls only — no multi-touch gestures required
- Toggle to disable screen shake and flash effects (important for Earth Dragon screen shake and banana mode camera shake)

---

## Technical Considerations

### System Architecture

**Engine:** Unity 2022 LTS (or latest LTS at development start)
**Language:** C#
**Rendering:** Universal Render Pipeline (URP) 2D
**Physics:** Unity 2D Physics (Rigidbody2D, BoxCollider2D)

**Project Structure:**
```
taekwondo-robot-builder-v2/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                   # Game manager, save, audio, scene loading
│   │   │   ├── GameManager.cs
│   │   │   ├── SaveSystem.cs
│   │   │   ├── AudioManager.cs
│   │   │   └── SceneLoader.cs
│   │   ├── Player/                 # Player controller, combat, costumes
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerCombat.cs
│   │   │   ├── PlayerHealth.cs
│   │   │   └── CostumeManager.cs
│   │   ├── Enemies/                # Enemy AI, bosses, state machine
│   │   │   ├── EnemyBase.cs
│   │   │   ├── EnemyStateMachine.cs
│   │   │   ├── States/             # PatrolState, ChaseState, AttackState, etc.
│   │   │   ├── MonkeyTitan.cs
│   │   │   └── BossController.cs
│   │   ├── Levels/                 # Level management, completion, stars
│   │   │   ├── LevelManager.cs
│   │   │   ├── LevelCompletionHandler.cs
│   │   │   └── StarRatingCalculator.cs
│   │   ├── Collectibles/           # Coins, robot parts, power-ups
│   │   │   ├── Collectible.cs
│   │   │   ├── RobotPart.cs
│   │   │   └── PowerUp.cs
│   │   ├── CraftMode/              # Robot assembly UI and logic
│   │   │   ├── CraftManager.cs
│   │   │   ├── PartSlot.cs
│   │   │   └── RobotPreview.cs
│   │   ├── Costumes/               # Costume system and special moves
│   │   │   ├── CostumeData.cs      # ScriptableObject
│   │   │   ├── CostumeSwitcher.cs
│   │   │   ├── SpecialMoves/       # One script per special move
│   │   │   │   ├── FireballSpecial.cs
│   │   │   │   ├── FreezeWaveSpecial.cs
│   │   │   │   ├── ChainLightningSpecial.cs
│   │   │   │   ├── SmokeExpandSpecial.cs
│   │   │   │   ├── EarthquakeSpecial.cs
│   │   │   │   ├── BananaSlipSpecial.cs
│   │   │   │   ├── PresentBombSpecial.cs
│   │   │   │   ├── StoneLaserSpecial.cs
│   │   │   │   ├── GrimlockTransformSpecial.cs
│   │   │   │   └── LegendaryBurstSpecial.cs
│   │   │   └── DragonAlly.cs       # AI companion for Present Dragon
│   │   ├── Projectiles/            # Projectile base + elemental variants
│   │   │   ├── ProjectileBase.cs
│   │   │   └── ProjectileFactory.cs
│   │   ├── BananaMode/             # Banana game modes
│   │   │   ├── BananaEntity.cs     # Food physics, slip, deflect
│   │   │   ├── BananaManager.cs    # Spawning, food power-ups
│   │   │   ├── FoodType.cs         # ScriptableObject for 5 food types
│   │   │   ├── BananaSurvival.cs   # Survival mode logic
│   │   │   └── BananaBonus.cs      # Bonus mode logic
│   │   ├── UI/                     # All UI controllers
│   │   │   ├── HUDController.cs
│   │   │   ├── MainMenuController.cs
│   │   │   ├── WorldMapController.cs
│   │   │   ├── PauseMenuController.cs
│   │   │   ├── CostumeSelectUI.cs
│   │   │   ├── BananaResultsUI.cs
│   │   │   └── CutscenePlayer.cs
│   │   ├── Input/                  # Input abstraction
│   │   │   └── InputManager.cs
│   │   └── PowerUps/              # Power-up queue and effects
│   │       ├── PowerUpQueue.cs
│   │       ├── SpeedBoost.cs
│   │       ├── Shield.cs
│   │       ├── ElementalAttack.cs
│   │       └── Invincibility.cs
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Collectibles/
│   │   ├── Projectiles/
│   │   ├── Bananas/
│   │   ├── Platforms/
│   │   └── Effects/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── WorldMap.unity
│   │   ├── CraftMode.unity
│   │   ├── BananaSurvival.unity
│   │   ├── BananaBonus.unity
│   │   ├── World1/
│   │   ├── World2/
│   │   ├── World3/
│   │   ├── World4/
│   │   └── World5/
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Tilesets/
│   │   ├── UI/
│   │   ├── Backgrounds/
│   │   └── Cutscenes/
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   └── ScriptableObjects/
│       ├── EnemyData/
│       ├── CostumeData/          # 10 costumes + Legendary
│       ├── FoodTypeData/         # 5 food types
│       ├── LevelData/
│       └── PowerUpData/
├── Packages/
├── ProjectSettings/
└── README.md
```

**Key Design Patterns:**
- **ScriptableObjects** for data-driven design (enemy stats, costume definitions, food types, level configs)
- **State Machine** for player animation and enemy AI
- **Singleton** for managers (GameManager, AudioManager, SaveSystem) via ServiceLocator
- **Observer/Event** pattern for loose coupling (UnityEvents, C# events)
- **Object Pooling** for projectiles, particles, bananas, and collectibles
- **Strategy Pattern** for costume special moves (each costume's special move is a separate MonoBehaviour implementing ISpecialMove)
- **Factory Pattern** for projectile creation (ProjectileFactory creates the right projectile based on costume type)

### Input System
- Unity's new Input System package
- Action maps: Gameplay, UI, Cutscene, BananaMode
- Automatic device switching (keyboard ↔ gamepad ↔ touch)
- Custom bindings for Grimlock transform (2 key) and Stone Dragon blast (T+S combo)

### Build Pipeline
- Unity Cloud Build or GitHub Actions for CI/CD
- Automated builds for WebGL, iOS, Android
- Addressable Assets for WebGL streaming (reduce initial load)

---

## Implementation Roadmap

### Phase 1: Foundation (Week 1-3)
**Goal:** Core player movement, single test level, basic enemies

**Tasks:**
1. Unity project setup with folder structure and URP 2D (3h)
2. Input system setup (keyboard + touch + gamepad) (6h)
3. Player controller (movement, jump, double jump) (8h)
4. Player combat (punch, kick, stomp, damage) (8h)
5. Player health and animation state machine (8h)
6. Basic tilemap level with platforms and camera (8h)
7. Base enemy with state machine AI (patrol, chase, attack) (8h)
8. Collectible system (coins, robot parts) (4h)
9. Basic HUD (health, score) (4h)

**Validation Checkpoint:** Player can run, jump, fight enemies, and collect items in a test level with audio feedback

---

### Phase 2: Core Systems (Week 4-6)
**Goal:** Craft mode, save system, power-ups, costume framework, World 1

**Tasks:**
10. Level completion and star rating system (6h)
11. Scene transitions and loading screens (4h)
12. Craft mode UI and drag-and-drop assembly (12h)
13. Robot part inventory and data model (6h)
14. Save system with platform abstraction (6h)
15. Power-up queue system with activation (10h)
16. Costume data model and sprite swapping (8h)
17. Projectile system (base class + factory) (8h)
18. First 3 costumes with specials (Default, Fire, Ice) (12h)
19. Main menu and pause menu (6h)
20. World 1: Ice World level design (3 levels + boss) (16h)

**Validation Checkpoint:** Playable World 1 with craft mode, saves, 3 costumes, and power-ups

---

### Phase 3: Content Expansion (Week 7-10)
**Goal:** All 5 worlds, all 10 costumes, banana modes

**Tasks:**
21. Lightning Dragon costume + chain special (5h)
22. Shadow Dragon costume + expanding smoke (5h)
23. Earth Dragon costume + earthquake (4h)
24. Banana Dragon costume + slip/trail/breath (6h)
25. Present Dragon costume + bomb + dragon ally AI (10h)
26. Stone Dragon costume + laser-to-boulder combo (8h)
27. Dino Grimlock costume + transform + duck laser (10h)
28. Legendary Mode (fusion, 5x damage, multi-color) (6h)
29. Costume selection UI (10 costumes, previews, unlock tracking) (8h)
30. World 2: Fire World (levels + enemies + boss) (16h)
31. World 3: Ultimate Power Bomb (levels + enemies + boss) (16h)
32. World 4: Lightning World (levels + enemies + boss) (16h)
33. World 5: Shadow World (levels + enemies + boss) (16h)
34. Banana entity with 5 food types and physics (8h)
35. BananaManager (spawning, deflection, food power-ups) (6h)
36. Monkey Titan enemy (banana throwing AI) (8h)
37. Banana Survival mode (arena, waves, scoring, UI) (12h)
38. Banana Bonus mode (timer, rewards, level integration) (10h)

**Validation Checkpoint:** Full game playable with all worlds, all costumes, and banana modes

---

### Phase 4: Audio, Story & Polish (Week 11-13)
**Goal:** Audio, cutscenes, visual polish, world map

**Tasks:**
39. Audio manager implementation (6h)
40. Source/create music (menu + 5 worlds + boss + banana) (16h)
41. Source/create 20+ sound effects (10h)
42. Integrate all SFX (8h)
43. Cutscene system (6h)
44. Cutscene art and narrative content (16h)
45. World map screen with progression (8h)
46. Parallax backgrounds per world (8h)
47. Particle effects and visual polish (10h)
48. Settings screen (volume, controls, accessibility) (4h)

**Validation Checkpoint:** Full game with audio, story, and polished visuals

---

### Phase 5: Platform & Release (Week 14-16)
**Goal:** Cross-platform deployment, testing, optimization

**Tasks:**
49. WebGL build optimization (Addressable Assets, compression) (8h)
50. iOS build setup and App Store configuration (8h)
51. Android build setup and Play Store configuration (8h)
52. Touch controls tuning and mobile UI scaling (6h)
53. Performance profiling and optimization (60 FPS target) (8h)
54. Achievement system (8h)
55. Playtesting with kids and feedback iteration (16h)
56. Bug fixing and polish (20h)
57. Final builds and store submissions (8h)

**Validation Checkpoint:** All platforms tested, store submissions approved

---

### Task Dependencies Visualization

```
Phase 1 (Foundation):
  Setup → Input → Player Controller → Combat → Enemies
  Setup → Tilemap → Camera
  Player → Collectibles → HUD

Phase 2 (Core Systems):
  Phase 1 → Level Completion → Transitions → World 1
  Phase 1 → Craft Mode → Save System
  Phase 1 → Power-Up Queue → HUD update
  Phase 1 → Costume Framework → Projectile System → First 3 Costumes

Phase 3 (Content):
  Costume Framework → Remaining 7 Costumes + Legendary
  Phase 2 → Worlds 2-5
  Phase 1 Enemies → Monkey Titan → Banana Entity → Banana Modes
  Costume System → Banana Dragon integration

Phase 4 (Audio & Story):
  Audio (parallel from Phase 1) → Full Integration
  Phase 3 → Cutscenes → World Map

Phase 5 (Platform):
  Phase 4 → Platform Builds → Testing → Store Submissions

Critical Path:
  Setup → Player → Combat → Enemies → World 1 → Worlds 2-5 →
  Audio Integration → Platform Builds → Store Submission
```

### Effort Estimation

| Phase | Estimated Hours |
|-------|----------------|
| Phase 1: Foundation | ~57h |
| Phase 2: Core Systems | ~94h |
| Phase 3: Content Expansion | ~196h |
| Phase 4: Audio, Story & Polish | ~92h |
| Phase 5: Platform & Release | ~90h |
| **Total** | **~529h** |
| Risk buffer (+20%) | ~106h |
| **Final estimate** | **~635h** |

---

## Out of Scope

Explicitly NOT included in v2 launch:

1. **Multiplayer / Co-op** — Adds significant server infrastructure complexity
2. **Voice Acting** — Budget and scope; text-based cutscenes are sufficient
3. **Level Editor** — Too complex for launch; levels are hand-designed in Unity
4. **In-App Purchases** — v2 is a premium/free game with no IAP
5. **Cloud Saves** — Local saves only at launch (cloud sync is post-launch)
6. **Additional Costumes Beyond 10+Legendary** — Ship 10 polished costumes, expand post-launch
7. **Procedural Level Generation** — All levels are hand-crafted
8. **Online Leaderboards** — Post-launch feature (REQ-015)

---

## Open Questions & Risks

### Open Questions

#### Q1: Art pipeline — custom sprites or asset store?
- **Options:** (A) Commission custom sprite art, (B) Use Unity Asset Store packs and customize, (C) Create original pixel art
- **Impact:** High — determines visual identity and timeline for art-dependent tasks
- **Deadline:** Before Phase 1 begins

#### Q2: Audio — original compositions or licensed?
- **Options:** (A) Commission original music, (B) License royalty-free tracks, (C) Use free/CC music
- **Impact:** Medium — affects budget and timeline but not architecture
- **Deadline:** Before Phase 4

#### Q3: Monetization model?
- **Options:** (A) Free with ads, (B) Premium (paid upfront), (C) Free, no monetization
- **Impact:** High — affects UI design, COPPA compliance requirements, and store listing
- **Deadline:** Before Phase 5

#### Q4: How many levels per world?
- **Options:** (A) 3 levels + boss per world (20 total), (B) 5 levels + boss per world (30 total), (C) Variable per world
- **Impact:** High — directly affects Phase 3 timeline
- **Deadline:** Before Phase 2

#### Q5: Should Banana Bonus trigger rate be configurable or fixed?
- **Options:** (A) Keep v1 rates (40/50/60/75%), (B) Always trigger, (C) Player choice in settings
- **Impact:** Low — single conditional check
- **Deadline:** During Phase 3

### Risks & Mitigation

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| WebGL build size too large | Medium | High | Addressable Assets, texture compression, test early |
| Unity WebGL performance on low-end devices | Medium | High | Profile early, object pooling, limit particles |
| App store rejection (kids category strict rules) | Medium | High | Review Apple/Google kids category guidelines pre-development |
| Art asset pipeline bottleneck | High | Medium | Start with placeholder sprites, art swappable without code changes |
| Scope creep from 10 costume special moves | Medium | Medium | Implement costumes incrementally, lowest complexity first |
| Touch controls unusable for kids | Medium | High | Playtest with actual kids at Phase 2 checkpoint |
| Grimlock transform introduces bugs | Medium | Medium | Isolate transform state in dedicated component, extensive testing |
| Banana mode balance issues | Low | Medium | Tunable ScriptableObject parameters, playtest heavily |

---

## Validation Checkpoints

### Checkpoint 1: End of Phase 1 (Week 3)
- [ ] Player movement feels responsive and fun
- [ ] Combat has satisfying feedback (even with placeholder art)
- [ ] At least 1 enemy type works with full AI
- [ ] Test level is playable start to finish
- [ ] Input works on keyboard AND touch

**If Failed:** Do not proceed. Core feel must be right before building content.

---

### Checkpoint 2: End of Phase 2 (Week 6)
- [ ] World 1 is fully playable (3 levels + boss)
- [ ] Craft mode works — parts collected → assembled → robot visible
- [ ] Save/load works across sessions
- [ ] Power-ups feel strategic and fun
- [ ] 3 costumes work with distinct specials
- [ ] Playtest with 2-3 kids — observe without guiding

**If Failed:** Iterate on gameplay feel and craft mode UX before adding more worlds.

---

### Checkpoint 3: End of Phase 3 (Week 10)
- [ ] All 5 worlds playable with unique themes and bosses
- [ ] All 10 costumes + Legendary Mode unlockable and functional
- [ ] Banana Survival and Banana Bonus modes working
- [ ] All 5 food types behave distinctly
- [ ] Grimlock transform works without breaking other systems
- [ ] Present Dragon ally AI behaves correctly

**If Failed:** Cut P1/P2 features and focus on polishing what exists.

---

### Checkpoint 4: End of Phase 4 (Week 13)
- [ ] Audio enhances the experience (not annoying on repeat)
- [ ] Cutscenes tell a coherent story
- [ ] Full game can be played start to finish with audio and story
- [ ] World map shows clear progression

**If Failed:** Ship without cutscenes if needed; audio is required.

---

### Checkpoint 5: End of Phase 5 (Week 16)
- [ ] WebGL builds and runs at 60 FPS on midrange hardware
- [ ] iOS build approved for TestFlight
- [ ] Android build approved for internal testing
- [ ] No critical bugs in 48-hour soak test
- [ ] All store listing assets prepared
- [ ] COPPA compliance verified

**If Failed:** Delay launch, fix blockers, re-submit.

---

## Appendix: Task Breakdown Hints

### Parallelizable Work
- Audio (tasks 39-42) can run in parallel with all other work from Phase 1 onward
- Art assets can be created independently and dropped in at any phase
- Worlds 2-5 (tasks 30-33) can be built in parallel once enemy framework exists
- Costumes 4-10 (tasks 21-28) can be built in parallel once costume framework and projectile system exist
- Platform builds (tasks 49-51) can run in parallel
- Banana modes (tasks 34-38) can be built in parallel with world content once Monkey Titan exists

### Critical Path Tasks
1. Project setup → Player controller → Combat system
2. Enemy AI → World 1 level design
3. Costume framework → Projectile system → All costumes
4. Audio integration → Platform builds → Store submission

**Critical path duration:** ~300h (~8 weeks with full-time dev)

---

**End of PRD**

*This PRD is based on comprehensive analysis of the v1 Taekwondo Tech codebase (15 PRs, 30+ commits), including all 10 dragon costumes with their exact elemental powers, Legendary Mode, both banana game modes with 5 food types, and the complete Monkey Titan enemy system. All v1 features are preserved and reimagined for Unity.*

---

## Revision History

| Version | Date | Author | Status | Summary |
|---------|------|--------|--------|---------|
| 1.0 | 2026-02-26 | Norrie Taylor | Draft | Initial PRD created covering all 8 user stories and full functional requirements |
| 1.1 | 2026-02-27 | Norrie Taylor | Approved | Reviewed all user stories for completeness; confirmed all 6 core stories have specific, testable acceptance criteria; promoted status from Draft to Approved |
