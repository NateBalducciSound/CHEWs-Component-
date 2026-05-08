# CHEWS (ChucK Environmental Weather System)

CHEWS is a procedural audio weather generation component for Unity, powered by Chunity (ChucK in Unity). Instead of relying on static audio loops, CHEWS synthesizes dynamic rain and wind ambience in real-time, allowing the environment's soundscape to react fluidly to the player's actions and location.

## Features

* Procedural Weather Synthesis: Generates infinitely variable wind and rain sounds in real-time using ChucK scripts.
* Reactive Player Dynamics: Modulates audio parameters on the fly based on the player's world position and movement velocity.
* Grid-Based Weather Mapping: Define spatial weather zones using a coordinate grid system. Map specific audio parameters to physical directions (e.g., moving positive on the X-axis increases wind intensity; moving positive on the Y-axis increases rain density).
* Automatic Indoor Occlusion: Includes trigger-based scripts that automatically apply acoustic filtering (muffling/lowpass) when the player enters or exits a building.

## Included Unity Scripts

The package comes with a few handy Unity C# scripts to handle the communication between the game engine and the ChucK compiler:

* `ChewsManager`: The core script that initializes the Chunity component and holds the main ChucK audio script.
* `ChewsPlayerTracker`: Attaches to your player controller. It calculates real-time velocity and positional data and feeds it directly into ChucK global variables.
* `ChewsGrid`: Handles the spatial mapping. Allows you to define the X and Y bounds of your weather grid and scales the ChucK parameters accordingly.
* `ChewsIndoorFilter`: Attach this to a trigger collider representing the inside of a building. It automatically lerps the procedural audio's filter frequencies when the player enters or leaves the trigger zone.

## Prerequisites

To use this component, your Unity project must have:
* Unity 2021.3 or higher (recommended)
* Chunity (ChucK for Unity) installed and configured in your project.

## Installation and Setup

1. Ensure Chunity is installed in your Unity project.
2. Import the CHEWS package or clone this repository into your Unity `Assets` folder.
3. Drag the `CHEWS_System` prefab into your scene, or manually attach the `ChewsManager` and a `ChucK` component to an empty GameObject.
4. Attach the `ChewsPlayerTracker` script to your main Player GameObject and assign it in the manager.
5. Set up your weather grid by tweaking the X/Y coordinate bounds in the `ChewsGrid` component.
6. To set up indoor areas, add a Box Collider (set to IsTrigger) to your building interior and attach the `ChewsIndoorFilter` script.

## Customizing the Audio

Because the audio is entirely procedural, you can open the included `.ck` files to tweak the fundamental synthesis architecture. The Unity scripts communicate with ChucK via global variables, so you can easily add your own parameters to the ChucK script and update them via C# to create entirely new reactive behaviors.

## Acknowledgments

* Built with Chunity (ChucK in Unity).
