# TUD Polfilter AR (tud-polfilter-ar)

Heartucate GmbH — in collaboration with Technische Universität Dresden (TUD)

A Unity-based augmented reality application and framework used to demonstrate and teach polarization concepts using polarising filters and interactive AR visualisations. The project integrates AR Foundation for camera/AR features, a Bluetooth microcontroller interface for hardware experiments, and several in‑editor utilities and examples.

This README gives a practical map of where to find important scripts and components in the repository so you can quickly locate, test and extend functionality.

Key features
- AR visualisations of polarisation and polariser behaviour
- Bluetooth microcontroller (HM10 and ESP32) support and example code
- Utility plugins: Ingame Debug Console and a cross-platform Bluetooth interface
- Example scenes and microcontroller examples

Quick start
1. Requirements
   - Unity (recommended 2022.3.62f3).
   - AR packages: AR Foundation and the matching platform provider (ARCore for Android, ARKit for iOS).
   - Mobile device with AR support for full functionality (only iOS is fully tested).

2. Open the project
   - Open this folder in Unity Hub and load the project.

3. Configure XR / AR
   - Ensure AR Foundation + platform packages are installed via Window → Package Manager.
   - Configure XR Plug-in Management in Project Settings for your platform.

4. Permissions and platform notes
   - Android: Ensure CAMERA and Bluetooth permissions are added to Player Settings / manifest and requested at runtime.
   - iOS: Add camera usage description and Bluetooth usage strings to Info.plist when building to device.

Where to find important scripts and what they do
- Bluetooth & microcontroller
  - Assets/TUD/Scripts/MicrocontrollerManager.cs
    - Unity-side manager handling connection, sending/receiving text payloads, and high-level commands (Reset, StartScan). Look for public static helpers: SendString, StartScan, SendResetCommand.
  - Assets/Shatalmic (examples)
    - Example microcontroller code and Unity examples that demonstrate scanning/connecting. See Assets/Shatalmic/Example/StartingExample/StartingExample.cs for a simple BLE server example and Assets/Shatalmic/Example/MultipleLevels for scanner usage.
  - Assets/Plugins/BluetoothHardwareInterface.cs
    - Platform abstraction layer for Bluetooth (iOS / Android / Editor workarounds). Contains permission prompts and native plugin calls.

- AR scene control and polarisation logic
  - Assets/TUD/Scripts/PolarisationSceneManager.cs
    - Central scene manager for polarisation experiments: controls visibility, state transitions and interaction hooks.
  - Assets/TUD/Scripts/RealtimeDataProcessor.cs
    - Receives and processes realtime data from microcontroller (if present); publishes parsed values to other systems.
  - Assets/TUD/Scripts/PolarisationData.cs
    - Data container / model used by scene manager and UI.

- UI and interaction utilities
  - Assets/TUD/Scripts/DegreeAdjuster.cs, DegreeAdjusterManager.cs — UI controls for rotating polariser visuals and degree displays.
  - Assets/TUD/Scripts/VisibilityButton.cs, VisibilityChanger.cs — Toggle visibility of visualization layers.
  - Assets/TUD/Scripts/ImageLibraryEnsurer.cs — Ensures AR reference images are available and converted for AR builds.
  - Assets/TUD/Scripts/MicrocontrollerSetupManager.cs — UI workflow for setting up/connecting to microcontroller.

- Debugging and editor helpers
  - Assets/Plugins/IngameDebugConsole — in-game console for runtime debugging.

Example scenes and starting points
- Assets/Shatalmic/Example contains BLE example scenes and scripts.
- Mobile AR template assets under Assets/MobileARTemplateAssets include a simple AR template menu and example scenes to start from.

Scene descriptions
- 00_Menu
  - Purpose: Main entry/menu scene for the app. All menu logic is located on the "MENU" GameObject.
  - What to inspect: Open scene 00_Menu in the Editor and expand the MENU GameObject in the Hierarchy. Typical components and scripts attached to the MENU root include:
    - MainMenuOpener: handles navigation and initialisation
    - MicrocontrollerSetupManager: BLE setup workflow and connect UI
    - GameStateSelectionManager: mode/level selection UI
    - Any UI Canvas and Buttons used to switch scenes
  - Note: The MENU GameObject centralises menu behaviours so you can quickly find the start-up flow by selecting this GameObject and inspecting all attached components in the Inspector.

- 01_Main
  - Purpose: Primary AR experiment scene. Contains the polarisation simulation objects, UI and runtime logic for the experiment.
  - Top-level structure and what it means (inferred from project hierarchy and scripts):
    - LOGIC
      - CameraManager: Contains camera for simulation mode.
        - SceneManager: sets start level.
      - PolarisationDataManager: holds PolarisationData and exposes it to UI and simulation logic.
      - PopUpManager: controls pop-up text and user guidance.
      - Visibility Changer: central place to toggle layer visibility across experiments.
      - LightUI: Light-related UI controls.
      - EventSystem: Unity EventSystem for UI interactions.

    - COMPONENTS
      - DegreeAdjusterManager (DegreeAdjusterManager.cs)
      - 00_Breadbord_Tisch_V1 (scene prop / table)
      - 06_Laser_V1 (laser object)
      - Lens (optical lens object)
      - Polarisationsfilter, Polarisationsfilter (1), Polarisationsfilter (2) (three polariser objects used in experiments)
      - Schirm (screen to visualise beam)
      - LaserBeam (beam renderer object)
      - These components are typically referenced from PolarisationSceneManager, DegreeAdjusterManager and VisibilityChanger via serialized fields — inspect those scripts to see exact references.

    - RENDERING
      - Global Volume (post-processing)
      - Simulation_BG (background for simulation)
      - Skybox

    - UI
      - Canvas (main UI canvas)
      - SceneButtons (buttons for experiment controls)

  - Where to look in code to understand behaviour and references:
    - PolarisationSceneManager.cs: open this script to see serialized GameObject/Component references; follow those fields in the Inspector to find the exact GameObjects in the scene.
    - DegreeAdjusterManager.cs / DegreeAdjuster.cs: handles polariser rotation and degree UI; search for fields named for the polariser GameObjects.
    - VisibilityChanger.cs / VisibilityButton.cs: see how layers (filters, beam, screen) are toggled.
    - RealtimeDataProcessor.cs: maps incoming microcontroller data to visual changes (intensity, angles, etc.).
    - LevelChanger.cs: controls experiment level state and triggers popups / degree resets when levels change.


Contributing, licensing and contact
- This repository contains code developed by Heartucate GmbH for TUD. Use and modification are permitted under agreed terms; please retain file credits when possible.
- For questions, licensing, or collaboration contact: info@heartucate.eu