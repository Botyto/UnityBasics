A collection of basic tools/classes/resources that will jumpstart your project.

# Gameplay
Contents related to gameplay development.

## Cameras
Collection of generic camera behaviors.

WindowCamera2D
: The camera follows it's targets while keeping them inside a small fixed portion of the viewport.

## Leaderboard
Tools for quickly implementing simple online leaderboards for your game.

DreamloLeaderboard
: Data class for your leaderboard settings.

DreamloLeaderboardApi
: API class for executing actions on and querying your leadboards.

## Loot table
Generic implementation for loot tables.

LootResult
: Pair of resource and it's amount.

ILootEntry
: Interface for loot table entries which can generate LootResults.

EmptyLootEntry
: Generates nothing.

LootEntry
: Generation results are picked from a list of ILootEntries.

LeaftLootEntry
: Generates a spcific resourse and amount.

## Properties
A collection of useful general purpose component properties related to gameplay.

Modifiable
: A modifiable float property. Objects can add/removes modifiers to/from it, such as "add 5", or "multiply by 3.5"

TaggedText
: A string that can contain tags (e.g. "HP: <Health>") that can be automatically replaced with values from the attached components.

TaggedTextFuncs
: A collection of functions that can be natively accessed through tags in TaggedText.

## General
A collection of components which are useful for gameplay.

BasicMonoBehavior
: Base class for all "basic" components.

AutoDecay
: Destroy the GameObject after a certain amount of time, or instantly.

InitialForce2D
: Applies a 2D force to the object on Start().

KillZone
: A component which destroys all objects that collide with it.

YDeath
: A component which destroys the object when it reachers a certain Y position value (useful for platformers).

Collector
: A component which collects (and counts) objects of a specific type (e.g. coins).

Selectable
: A marker component for objects that should be selectable.

SpawnPoint
: Object which automatically spawns a prefab (with a delay, optionally).

RotateCollectable2D
: Component to animation classic collectable bobbing.

# Attributes
A collection of useful attributes that help create more useful and user-friendly inspectors.

ClassName
: Assigned to string properties.
: Allows and helps to assign only values that correspond to a class name.
: A base class must be specified (it may or may not be included).

ColorPalette
: Allows easy color assignment from a predefined pallete.

DisableIf
: Inspector is disabled when the condition is true.

EnableIf
: Inspector is enabled only when the condition is true.

HideIf
: Hides the inspector when the condition is true.

ShowIf
: Shows the inspector only when the condition is true.

Required
: Shows an error message box near the property if no value is assigned (used for reference properties only).

InfoBox
: Shows a message box near the property to inform the editor of something.

ReadOnly
: Makes the inspector read-only.

FindInThis
: Automatically assigns a reference to a component from the object itself.

FindInParent
: Automatically assigns a reference to a component from the parent object.

FindInChild
: Automatically assigns a reference to a component from any child object.

# UI
Collection of starter components for developing UI.

UIDevConsole
: Developer console which can execute arbitrty C# code (in the editor).

UIDialog
: Open UI dialogs can be kept track of using a global list.

UINode
: UI nodes are objects which are important for the UI logic.
: Other objects may be used only for decorative purposes.
: Nodes can easily look for each other in the hierarchy.

# Visuals
A collection of components useful for creating better visuals.

SecondOrderDynamics
: A component inspired by this video: https://www.youtube.com/watch?v=KPoeNZZ6H4s
