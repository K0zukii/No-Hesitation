# No Hesitation - Endless Arcade Racing

*No Hesitation* is an "Endless Runner" arcade racing game developed in Unity using C#. The player must survive as long as possible in procedurally generated traffic while collecting fuel, as the overall speed of the game constantly increases.

This project was developed as part of the Unity **Junior Programmer Pathway** certification, with a strong focus on design patterns, performance optimization (especially for WebGL), and system architecture.

🔗 **[Play the game on Itch.io](https://kozukii.itch.io/no-hesitation)** | 🔗 **[Play on Unity Play](https://play.unity.com/en/games/dbd23f01-f25c-4854-ac9a-4823454041ae/no-hesitation)**

![Gameplay](No-Hesitation-gif.gif)
---

## Screenshots
**Main Menu:** <br>
<img width="1919" height="1079" alt="Capture d'écran 2026-03-12 025639" src="https://github.com/user-attachments/assets/c0c43ca4-4f7a-411e-a00c-e5e3a5ca4fe2" />

**Gameplay:** <br>
<img width="1915" height="1079" alt="Capture d'écran 2026-03-12 025655" src="https://github.com/user-attachments/assets/64a7b782-c1c6-4a7e-a944-b5b526546771" />

**Game Over Screen:** <br>
<img width="1919" height="1079" alt="Capture d'écran 2026-03-12 035958" src="https://github.com/user-attachments/assets/b02d1c4e-3fb8-43ca-acab-01cd70709ed6" />

---

## Description & Controls

**Game Rules:** The gas pedal is stuck. Dodge traffic, collect fuel cans to avoid running out of gas, and survive as long as possible. As time passes, the speed multiplier increases.

* ⬅️ / ➡️ **Left / Right Arrows** (or A / D): Change lanes.
* ⏸️ **Escape (Esc)**: Pause the game.

---

## Technical Specs

* **Game Engine:** Unity (Version 6000.3 / 6+)
* **Language:** C# (.NET)
* **Render Pipeline:** URP (Universal Render Pipeline)
* **Design Patterns used:**
    * *Singleton* (for the `MusicManager`)
    * *Object Pooling* (for infinite environment and entity generation)
    * *Observer / Events* (for decoupled communication between Collectibles and the Fuel system)
    * *State Machine* (basic implementation via `GameManager` for states: Play, Pause, GameOver)

---

## Project Architecture

The code is modularly structured to separate global logic from object logic:

* `GameManager.cs`: Core of the game. Handles the global `speedMultiplier`, score, pause states (using `Time.timeScale` and `AudioListener.pause`), and game over logic.
* `MusicManager.cs`: Implements the Singleton pattern coupled with `DontDestroyOnLoad` to ensure seamless audio playback between the menu and the game, without unwanted resets.
* `FuelManager.cs`: Attached to a specific GameObject, it controls the player's current fuel, max fuel limits, the constant burn rate, and handles the UI updates.
* `Collectible.cs`: Uses C# Events to communicate directly with the `FuelManager`. When collected, it passes the exact amount of fuel restored by reading values from a **ScriptableObject**, making the system highly modular and scalable.
* `MoveBackwards.cs`: Script attached to environment elements and obstacles. Reads the speed from the `GameManager` to simulate player movement.

---

## Technical Postmortem

This project was a major optimization and architectural challenge. Here are the main technical issues encountered and the solutions implemented:

### 1. The Seamless Infinite Loop & Memory Optimization
Rather than moving the player through an infinite world (which causes floating-point precision issues), I opted for the "Treadmill effect": the player remains fixed on the Z-axis, while the environment, enemies, and fuel move towards them.

* **The Looping Struggle:** Initially, I tried to build and place the chunks manually. Following the Junior Programmer Pathway, I automated the repetition, but achieving a perfectly symmetrical and seamless infinite illusion was a massive headache. The alignment was constantly off, breaking the immersion.
* **The Solution:** I meticulously readjusted the asset positions to achieve perfect symmetry and encapsulated an entire scenery chunk inside an empty parent GameObject. By adding a single `BoxCollider` to this empty parent, the game cleanly detects exactly when the whole chunk is out of bounds and triggers the repetition logic flawlessly. 
* **Object Pooling:** Instantiating and destroying objects in a loop created memory allocation spikes (*Garbage Collection*), causing stuttering. I implemented an **Object Pooling** system where objects leaving the camera's view are deactivated and recycled, ensuring a consistent framerate and a fixed memory footprint.

### 2. Audio State Management & Persistence
Creating a coherent audio experience between scenes and game states proved to be surprisingly difficult. 

* **The Problem:** At first, my `MusicManager` kept getting destroyed every time the player transitioned from the Main Menu to the Game Scene. Furthermore, handling the music's state (pausing exactly where it left off during the pause menu, but fully restarting from `0:00` upon death or game restart) required precise logic.
* **The Solution:** I discovered and implemented the `DontDestroyOnLoad()` method to preserve the Singleton `MusicManager` across scenes. For state management, I centralized the audio logic inside the `GameManager`. I used `AudioListener.pause` to globally freeze all sounds during the pause menu, and explicitly called `Stop()` and `Play()` on the music source *only* when returning to the main menu or restarting a run.

### 3. WebGL Export Challenges
Compiling the game for web browsers was the most complex phase. The game initially ran at an unplayable 5 FPS in WebGL despite maintaining a steady 60 FPS on Windows.

* **Blocker A (The Moving Wall Syndrome):** The `MoveBackwards` script moved objects with `BoxCollider` components but no `Rigidbody`.
    * *Solution:* On PhysX, moving a static collider forces the engine to recalculate the entire spatial tree every frame, choking the CPU on the browser's single thread. I added a `Rigidbody` set to *Is Kinematic* at the root of the Prefabs (using the *Compound Colliders* principle for the environment). This explicitly told the engine the objects were script-driven, instantly solving the physics bottleneck.
* **Blocker B (Unsupported URP Shaders):** The browser console was spamming *Shader not supported on this GPU* errors. URP was trying to use heavy post-processing effects (HDR, MSAA) that the WebGL context could not handle without falling back to software rendering.
    * *Solution:* Assigned the `Mobile_RPAsset` profile specifically for the WebGL platform in the Quality Settings, disabling HDR and complex shadows, ensuring smooth rendering natively on the GPU.
