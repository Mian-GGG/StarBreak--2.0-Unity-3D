# StarBreak 2.0

A 3D sci-fi action shooter developed in Unity as my final-year BSc (Hons) Game Design and Development project, serving as the successor to StarBreak 1.0[cite: 5]. **StarBreak 2.0** combines physics-based 3D space flight combat with a first-person tactical ground level connected by a narrative briefing, scene transitions, and progression hooks[cite: 5, 7].

> **Repository Scope Note:** Because the complete Unity project exceeds 20 GB due to 3D models, environment assets, VFX, and audio packages, this repository contains **selected C# source scripts and gameplay screenshots** to showcase the programming and system architecture[cite: 7]. The complete, playable Windows build is distributed separately via **[GitHub Releases](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME/releases/tag/v1.0)**.

---

## Gameplay Demonstration

Watch the full playthrough of the implemented space and ground levels on YouTube:

[![StarBreak 2.0 Gameplay](https://img.youtube.com/vi/cn2Zl9tAvz4/0.jpg)](https://youtu.be/cn2Zl9tAvz4)

**Direct Link:** https://youtu.be/cn2Zl9tAvz4

---

## Screenshots

| Space Combat & Commander Encounter | Ground Combat & Base Infiltration |
| :---: | :---: |
| ![Space Combat](Screenshots/02-space-dogfight.png) | ![Ground Combat](Screenshots/05-ground-combat.png) |
| ![Commander Boss](Screenshots/03-commander-boss.png) | ![Keypad Puzzle](Screenshots/07-keypad-puzzle.png) |

---

## Project Overview

In **StarBreak 2.0**, the player takes on the role of Commander Riven Dale, deployed to the Solaris system after a human science team goes missing following an invasion by the Xyrath Collective[cite: 7].

During pre-production and level design, a three-level campaign was planned across three planets (*Solaris Ventra*, *Xaeus Primus*, and *Alpha Tauri*), where technology recovered on each planet's surface unlocks new ship capabilities in the next orbital encounter[cite: 2, 6]. Due to final-year time constraints, **Level 1 (both the Space Combat phase and the Ground Combat phase)** was fully implemented in-engine as a playable vertical slice[cite: 2, 5, 6].

### Playable Mission Flow (Level 1)
1. **Mission Briefing:** A paginated narrative briefing introduces the mission context before launching into orbit[cite: 5, 7].
2. **Space Combat (Solaris Ventra Orbit):**[cite: 6]
   - Fight waves of Xyrath fighter ships while evading fireball projectiles launched by the Commander Ship[cite: 7].
   - Once all escort waves are eliminated, the Commander Ship's energy shield drops, exposing destructible laser turrets/weak spots[cite: 7].
   - Survive delayed-tracking laser sweeps and intensified fireball volleys while destroying all weak spots/turrets to defeat the Commander Ship[cite: 7].
3. **Planetary Landing:** Approach the planet and trigger the landing interaction to play the transition cutscene into ground combat[cite: 7].
4. **Ground Combat & Base Infiltration (Solaris Ventra):**[cite: 2]
   - Engage Xyrath ground troops using a first-person rifle, tactical energy shield, and sprint mechanics[cite: 7].
   - After clearing the area, restore power to the locked base by shutting off electricity at the generator, repairing a damaged pipeline, and restoring power[cite: 7].
   - Bypass the warehouse security lock via a sequence-memory keypad mini-game and retrieve the loadout to complete the mission[cite: 7].

---

## Main Features

### Space Mode
- **Physics-Based Flight & Cockpit Feedback:** Rigidbody-driven pitch, yaw, roll, and thrust with boost energy management (`PlayerBoostSystem`), dynamic speed readout, and animated cockpit joystick/throttles (`CockpitAnimationController`)[cite: 5, 7].
- **Capacitor-Based Blasters:** Firing drains a recharging weapon capacitor (`Blaster`), firing weakened shots if depleted[cite: 7].
- **Autonomous Enemy Fighter AI:** Finite State Machine (`Patrol`, `Attack`, `Reposition`, `Retreat`) driven by PID steering controllers (`PIDController`) and multi-probe raycast/spherecast obstacle avoidance (`ObstacleDetector`)[cite: 5, 7].
- **Multi-Phase Commander Boss:** Wave spawner, invulnerability shield (`CommanderShield`), multi-point fireball barrages, and independent rotating turrets (`LaserTurret` / `CommanderWeakSpot`) that lock onto the player's position with a brief dodge delay[cite: 5, 7].
- **3D HUD & Target Tracking:** On-screen target boxes, health bars, and off-screen directional edge indicators (`TargetIndicator`, `UIManager`)[cite: 7].

### Ground Mode
- **First-Person Movement & Energy Shield:** Walk, sprint, jump, crouch, and activate a temporary energy shield (`PlayerMovementAlienShooter`) that shares an energy pool with sprinting[cite: 7].
- **Raycast Weapon System:** Modular weapon architecture (`IWeapon`, `WeaponHolder`) featuring hip/ADS spread buildup and recovery, recoil, procedural weapon sway/bob/breathing (`WeaponSway`), bullet trails, and reload animations (`RifleController`)[cite: 5, 7].
- **State-Machine Enemy AI (`EnemyAI`):**[cite: 5, 7]
  - States for `Roaming`, `Chasing`, `Rotating`, `Laser`, and `Dead`, plus group alert propagation (`CallForBackup`)[cite: 5, 7].
  - **Parabolic Fireball Throw:** Calculates ballistic launch velocity to arc fireballs toward the player; fireballs can also be shot mid-air or before release (`IExplodeable`) to trigger splash damage[cite: 1, 5, 7].
  - **Delayed-Tracking Laser:** Locks onto the player's position with a reaction window before sweeping toward the player[cite: 5, 7].
  - **Melee Slam:** Close-range shockwave attack with animation-event timing and CharacterController knockback[cite: 5, 7].
  - **Weak-Spot Hit Detection:** Separate hitboxes (`WeakSpotHit`, `NormalSpotHit`) with floating world-space damage numbers (`DamageNumberSpawner`)[cite: 7].
- **Interactive Puzzles & Progression:** Unified `IInteractable` system powering rotating levers (`DoorOpener`), sliding doors (`DoorSlider`), pipeline repair (`ElectricMender`), a Simon-style sequence-memory mini-game (`SequenceGame`), and mission objective tracking (`LevelManager`)[cite: 5, 7].

---

## Technical Architecture & Programming Concepts

- **Interface-Driven Decoupling:** Core gameplay systems communicate through C# interfaces (`IInteractable`, `IWeapon`, `IDamageable`, `ISpaceDamageable`, `IExplodeable`, `IMovementControls`, `IWeaponControls`) so player, weapon, damage, and interaction controllers do not depend on concrete object types[cite: 5, 7].
- **Finite State Machines (FSM):** Both space fighters (`EnemySpaceShipController`) and ground enemies (`EnemyAI`) use explicit state enums and transition logic to prevent conflicting simultaneous actions[cite: 5, 7].
- **Data-Driven Configuration:** Ship flight parameters, health, and blaster capacitor values are stored in `ScriptableObject` assets (`SpaceShipDataSO`)[cite: 5, 7].
- **PID Control & Spatial Math:** Space AI uses custom proportional-integral-derivative controllers (`PIDController`) for smooth angular steering, while ground AI uses kinematic projectile equations to compute parabolic fireball trajectories[cite: 5, 7].
- **Coroutines & UnityEvents:** Asynchronous combat phases, laser sweeps, reload animations, UI fades, and decoupled health/weak-spot UI updates are handled via coroutines and `UnityEvent` listeners[cite: 7].

---

## Controls

### Space Flight
| Action | Input |
| :--- | :--- |
| **Pitch / Yaw** | Mouse Movement[cite: 7] |
| **Thrust Forward / Reverse** | `W` / `S`[cite: 7] |
| **Roll Left / Right** | `Q` / `E`[cite: 7] |
| **Boost** | `Left Shift`[cite: 7] |
| **Fire Blasters** | `Left Mouse Button`[cite: 7] |
| **Initiate Planet Landing** | `T` (when in range after Commander is defeated)[cite: 7] |

### Ground Combat
| Action | Input |
| :--- | :--- |
| **Move** | `W` `A` `S` `D`[cite: 7] |
| **Look** | Mouse Movement[cite: 7] |
| **Sprint** | `Left Shift`[cite: 7] |
| **Jump** | `Space`[cite: 7] |
| **Crouch** | `Left Ctrl`[cite: 7] |
| **Fire / Aim Down Sights** | `Left Mouse Button` / `Right Mouse Button`[cite: 7] |
| **Reload** | `R`[cite: 7] |
| **Activate Energy Shield** | `Q` (requires ≥ 75% shield energy)[cite: 7] |
| **Switch Weapons** | `1` / `2` / `3`[cite: 7] |
| **Interact (Levers, Pipe, Keypad)** | `E`[cite: 7] |
| **Exit Keypad Mini-Game** | `Esc`[cite: 7] |

---

## Technologies Used

- **Engine:** Unity (3D, Universal Render Pipeline)[cite: 5, 7]
- **Language:** C#[cite: 5, 7]
- **Systems & Packages:** Unity Input System, Unity AI Navigation (`NavMeshAgent`), TextMeshPro, Unity UI, Particle Systems, LineRenderer[cite: 7]

---

##Download & Installation
Visit the Releases page.

Download StarBreak-2.0-Windows.zip.

Extract the .zip archive to a folder on your Windows PC.

Run the executable inside the extracted folder. (If Windows SmartScreen prompts on an unsigned build, select More info → Run anyway.)

##Acknowledgements & Credits
#Midnite Oil Software LLC (YouTube): Core space flight physics, ship control abstractions, and PID-based enemy fighter AI concepts were referenced from Midnite Oil Software LLC – How to make a 3D Space Shooter Game in Unity. I adapted and modified these systems—including enemy combat behavior, player controls and boost/capacitor mechanics, visuals, attack patterns, and the multi-phase Commander Ship encounter—to fit StarBreak 2.0.
