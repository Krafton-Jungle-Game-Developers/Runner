**************************************
*             RADIANT GI             *
*        Created by Kronnect         *   
*            README FILE             *
**************************************


Notice about Universal Rendering Pipeline
-----------------------------------------
This package is designed for URP. It requires Unity 2021.3 or later
Video instructions: https://youtu.be/sjcUA_nx5s4

To install the plugin correctly:

1) Make sure you have Universal Rendering Pipeline asset installed (from Package Manager).
2) Go to Project Settings / Graphics.
3) Double click the Universal Rendering Pipeline asset.
4) Double click the Universal Renderer asset.
5) Click "+" to add the Radiant Renderer Feature to the list of the Renderer Features.
6) If you're using forward rendering path, select the checkbox "Forward Rendering" in the Radiant Render Feature.

Note: URP assets can be assigned to Settings / Graphics and also Settings / Quality. Check both sections!

Make sure the Radiant Renderer Feature is listed in the Renderer Features of the  Forward Renderer in the pipeline asset.



Quick help: what's this and how to use this asset?
--------------------------------------------------

Radiant GI brings realtime screen space global illumination to URP.
It's a fast way to provide more natural look to the scenes.
Global Illumination means that each pixel acts as a tiny light so it bounces on the pixel surface and illuminate other surfaces.

Follow the instructions above to configure the effect in your project.


Help & Support Forum
--------------------

Check the Documentation folder for detailed instructions.

Have any question or issue?

* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect

If you like Radiant GI, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!



Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Radiant GI will be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Current version
- Virtual emitters: added Scene View bounds modifier tool

Version 6.5
- Virtual emitters: added range and material options
- Default maximum virtual emitters increased to 32
- Performance optimizations

Version 6.4.2
- Improved speed response of reflection probe changes
- Fixes

Version 6.4
- Improved near field obscurance option

Version 6.3
- Added support for orthographic camera

Version 6.2
- Added "Near Camera Attenuation" option under Artistic Controls
- Added material index field to virtual emitter
- Filters NaN pixels

Version 6.1
- Some shader optimizations

Version 6.0
- Added "Near Field Obscurance" option
- [Fix] Fixed virtual emitters not visible in compare mode

Version 5.2
- Improved accuracy of distance attenuation term
- Integration with URP native SSAO: parameter AO Influence added under Artistic Control section

Version 5.1
- Added support for both deferred and forward path materials. Enable 'Both' in the Radiant GI render feature if needed.
- Added stencil check option under Artistic Controls section
- Improved GI reconstruction speed on new screen pixels

Version 5.0
- Added support for Unity 2022
- Added "Probes Intensity" option to reflection probe fallback
- Added "Add Material Emission" option to virtual emitters
- Added "Show In Scene View" option

Version 4.0
- Added virtual emitters
- Added reflective shadow map fallback option and intensity property
- Added support for reflection probe blending distance
- Added optimization for single probe
- Updated documentation

Version 3.1
- Added "Limit To Volume Bounds" option
- [Fix] Fixes transparent objects not showing up correctly when Compare Mode is enabled

Version 3.0
- Added "Fallback Mode" with reflection probes option
- [Fix] Fixed color bleeding when using very high indirect intensity values

Version 2.4
- Added new Artistic Controls section with new options: Brightness Threshold, Maximum Brightness and Saturation.
- Radiant GI now ignores overlay cameras
- [Fix] Fixed issue when enabling "Accurate G-Buffer normals"

Version 2.3
- Added "Raytracer Accuracy" option
- Some performance improvements

Version 2.2
- Added "Chroma Threshold". A new option that stabilizes noise based on chroma variance

Version 2.1
- Additional levels for downsampling are now available for better mobile support
- Minor changes to smoothing levels
- [Fix] Fixed depth rejection bug when camera moves

Version 2.0
- Added Normals Influence option to enhance/preserve normal map based details
- Added Luma Influence option to enhance results in forward rendering mode by adding variety

Version 1.0
- Initial launch