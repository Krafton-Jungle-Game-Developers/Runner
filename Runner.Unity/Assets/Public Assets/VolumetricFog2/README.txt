**************************************
*       VOLUMETRIC FOG & MIST 2      *
*         Created by Kronnect        *   
*            README FILE             *
**************************************

Requirements
------------
Volumetric Fog & Mist 2 currently works only with Universal Rendering Pipeline (v7.1.8 or later)
Make sure you have Universal RP package imported in the project before using Volumetric Fog & Mist 2.


Demo Scenes
-----------
There's a demo scene which lets you quickly check if Volumetric Fog & Mist 2 is working correctly in your project.
Note: make sure you have Universal RP 7.5 or later installed from Package Manager and also a URP pipeline asset assigned to Graphics Settings.


Documentation/API reference
---------------------------
The PDF is located in the Documentation folder. It contains instructions on how to use this asset as well as a useful Frequent Asked Question section.


Support
-------
Please read the documentation PDF and browse/play with the demo scenes and sample source code included before contacting us for support :-)

* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Volumetric Fog & Mist will be eventually available on the Asset Store.


Version history
---------------

Current version
- Added "Max Distance" and "Max Distance FallOff" options to profile
- Border option in profile is now disabled by default, can be enabled in Volumetric Fog Manager's Shader Options section

V9.5.2
- [Fix] Fixed an issue with Volumetric Lights when disabling the render feature

V9.5
- Added "Custom Detail Noise Wind Direction" option
- Added in-editor size handles to subvolume gizmo
- [Fix] Fixed some properties not transitioning correctly when using subvolumes

V9.4
- Added "Ambient Light" multiplier under Directional Light section
- Terrain Fit: occlusion cam now uses an exclusive renderer to avoid issues with other render features
- API: added "RestoreToAlpha" parameter to SetFogOfWar methods

V9.3
- Added support for directional light cookie (requires Unity 2021.3 or later)
- [Fix] Fixed some fog of war traces when two or more user clear fog calls act on the same area
- [Fix] Fixed rendering issue with transparent & alpha-cutout prepass in VR Single Pass Instanced

V9.2
- Added Camera Layer Mask option to the render feature
- Option to allow render feature to run on reflection probes
- Improved volumetric fog inspector
- Ensures only one fog manager exists when loading multiple scenes

V9.1
- Improvements to fog composition when using downscaling & blur
- Fog distance now takes into account 3d distance instead of 2d distance
- [Fix] Fixed issue when SSAO render feature is used in Unity 2021
- [Fix] Fixed depth gradient wrapping colors issue on Android

V9.0
- Added support for Unity 2022
- Depth / Height Gradient options now take into account alpha values for transparency
- Added "Use Day Night Cycle" option to inspector

V8.3
- Added option to allow rotation of fog volumes in Volumetric Fog Manager (Shader Options section)
- Minimum Unity version required is now 2020.3.16

V8.2
- Fade option: changed fade distance meaning. Fog volume blendings now starts when reference controller is within this fade distance to any volume edge.
- [Fix] Fixed transparency issue which prevents full opacity under certain circumstances (like using very large volumes)

V8.1.1
- Fog volumes now completely disable if they become completely transparent
- [Fix] Fixed a material issue when duplicating a fog volume

V8.1
- Added new shader option to Volumetric Fog Manager: support for fog void rotation (example: https://youtu.be/rtS1ayn28X0)
- Added new shader option to Volumetric Fog Manager: world-space aligned noise

V8.0
- A new section called "Shader Options" has been added to Volumetric Fog Manager which allows easy management of static options
- Added blur HDR option
- Added support for blue-noise (check documentation to learn how to enable it)

V7.2 16/Mar/2022
- Unique file name will now be assured when creating a new fog of war texture asset
- [Fix] Fixed an issue that prevented refresh the fog of war texture when assigning it from scripting

V7.1 1/Feb/2022
- Added downscaling option to Volumetric Fog Manager to boost performance
- Change: height gradient option now goes from bottom to top of fog volume instead of mirroring around the center
- Prevents fog render features to run on terrain fit cam

v7.0 26/Dec/2021
- Added new blur options to Volumetric Fog Manager
- [Fix] Fixed light diffusion intensity when there's no Sun in the scene

v6.3 12/Dec/2021
- Added "Depth Gradient Color" and "Height Gradient Color" options to add stylized distance-based gradient color
- Added support for Unity 2021.2
- Reduced shader variants
- [Fix] Fixed distance falloff not applied when user saves scene until scene restart

v6.2 28/Nov/2021
- Added shader option "FAST_POINT_LIGHT_OCCLUSION" in PointLights.cginc
- Improved reliability of fog of war delayed transitions

v6.1
- Added "Moon" slot to Volumetric Fog Manager

v6.0
- Added support for Unity native lights including spot lights with shadows
- Fast point lights management optimizations
- [Fix] Fixed wind direction

v5.4
- Fog of War: added "Is Local" option: makes the fog of war center local to the fog volume (moves with it)
- [Fix] Changes to profiles used in subvolumes didn't affect the fog instance at runtime

v5.3.3
- [Fix] Fixed issue with fade out option

v5.3.1
- [Fix] Fixed issue with SetFogOfWarAlpha using the default fog of war texture

v5.3
- Added "Fade Out" option
- Depth pre-pass now uses an internal shader to avoid dependency on standard urp/unlit
- [Fix] Fixed a fog void issue during scene unload

v5.2
- Optimization: point lights are now excluded if they're behing camera and beyond light range
- Optimization: faster fog void registration
- Optimization: added MAX_ITERATIONS global setting to CommonsURP.hlsl

v5.1
- Fog Voids are now fully 3D
- API: added "settings" accessor so fog properties can be changed without affecting the profile (similar to material vs sharedMaterial)

v5.0
- New Terrain Fit option: https://youtu.be/yBT4no45g2Q
- Fog void changes: removed radius, now size is controlled with transform scale.
- New "Roundness" option to fog void, allows rectangles, rounded rectangles and circular shapes

v4.1
- Added "Alpha Clipping" transparency support for improving results with special objects like fur or hair.
- Sub-Volumes is now a separate option from Fade section.

v4.0
- Added "Fade" option. Allows smooth transitioning when moving from outside into the fog volume.
- Added "Show Boundary" option. Shows an overlay on the fog volume in Game View.
- Added "Sub-Volume" support. Allows customizing fog properties in different areas within the same volume.

v3.4
- Added "Alpha CutOff" option when "Include Transparent" option is enabled

v3.3
- Improved "Border" appearance
- Ability to use only 3D noise when Noise Final Multiplier is set to 0 (this only affects to the base/2D noise)

v3.2
- Added "Raymarch Min Step" parameter
- Added "Detail Offset" parameter
- [Fix] Fixed point lights issue with orthographic camera

v3.1
- Added orthographic camera support

v3.0.1
- [Fix] Fixed "Flip Depth Texture" option issue which prevented it from being applied

v3.0
- Added Detail Noise option with custom strength and scale
- Added Boundary section (new boundary type: sphere)
- Added Vertical Offset option

v2.2.1 29-APR-2020
- [Fix] Fixed fog not rendering at distance due to camera far clip issue

v2.2 April / 2020
- [Fix] Fixed VR issues

v2.1 April / 2020
- Shader optimizations
- [Fix] Workaround for shadows issue on WebGL 2.0

v1.0 Febrary / 2020
First release