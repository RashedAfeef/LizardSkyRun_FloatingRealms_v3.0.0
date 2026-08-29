# Changelog

## 3.0.0 — Lizard Sky Run: The Floating Realms

- Preserved the supplied animated lizard FBX, its original spotted-skin texture and the full run, jump, lane-change, slide, stumble, fall and celebration motion layer.
- Replaced the Amman runtime environment with a floating moonstone causeway, luminous lane runes, drifting islands, orbiting rocks and five lightweight fantasy landmark variants.
- Integrated the supplied 8192×4096 blue-sky equirectangular image as the active panoramic Skybox, with a cool procedural fallback.
- Re-themed every gameplay obstacle as a crystal fence, rune totem, low aether gate, sleeping sky serpent or floating rune boulder while preserving collision sizes and gameplay behavior.
- Added a cyan, violet and rose emissive material palette, fantasy HUD identity, aether board visuals and stardust particles.
- Removed the unreferenced 5,057,024-byte legacy FBX after verifying it was an exact truncated prefix of the complete active 26,062,492-byte character file; the complete animated lizard remains intact.

## 2.9.0 — Downtown Roman Theatre District

- Restored the supplied textured and animated Meshy FBX as the active runtime 3D character, with the articulated procedural Jordanian hero retained as fallback.
- Rebuilt the Roman Theatre as an 11-tier semicircular cavea composed of curved seating segments, three radial aisles, orchestra floor, stage deck, retaining walls and a multi-level columned stage facade.
- Added a joined Hashemite Plaza forecourt with patterned paving, twin fountains, palms, gardens and benches.
- Added a compact Odeon theatre beside the plaza and a dense terraced Jabal Al-Joufah limestone-building backdrop with windows, rooflines and water tanks.
- Reserved the landmark side of its road segment for the theatre district so normal shop blocks no longer obscure the main view.
- Added dedicated Roman stone, plaza paving and garden materials, and updated project validation for the active FBX, texture, clips and Humanoid Avatar.

## 2.8.0 — Original Cartoon Plumber Hero

- Replaced the active gecko-derived Meshy model with a new original short, stocky platform-game plumber built as a fully articulated procedural 3D character.
- Added a large expressive head, round nose, layered moustache, hair, ears, blinking blue eyes, red cap with an original geometric badge, ivory gloves and oversized boots.
- Added layered red shirt and royal-blue overalls with straps, pocket, stitches and gold buttons.
- Added a dedicated saturated cel-shaded material palette with three-step lighting, colored shadows and subtle rim highlights.
- Connected the new character to run, idle, jump, fall, lane lean and three-stage one-knee slide motions, and added procedural stumble, game-over fall and celebration poses.
- Kept the supplied Meshy FBX in Resources as an optional legacy asset while removing it from the default runtime path.
- Avoided protected logos, copied meshes and trademark lettering; the character is an original platform-game design.

## 2.7.0 — Hero Close-Up, Cartoon Color and Pro Slide

- Brought the gameplay camera substantially closer, from a 4.65–5.25 m follow distance, and reduced gameplay FOV to 49–56 degrees while preserving road visibility.
- Added a bundled toon shader with three-step cel lighting, richer saturation and contrast, cool shadow color and a soft cyan rim highlight.
- Reduced surface gloss for a cleaner professional runner-game character style while preserving the supplied UV texture details.
- Rebuilt slide posing into three authored phases: quick compression, readable one-knee glide and planted-foot recovery back into the run.
- Fixed excessive slide depth by removing the duplicated downward body displacement and reducing the external pivot drop and rotation.

## 2.6.2 — Complete FBX Restoration

- Added the complete 26,062,492-byte supplied Meshy FBX as `JordanianHero_Meshy_Animated_Full.fbx` after detecting that the previous project copy was truncated to its first 5,057,024 bytes, and redirected the importer and runtime loader to the complete asset.
- Verified the FBX 7.4 node boundaries, Humanoid bone names and the `Run_03`, `Running` and `Walking` animation stacks before packaging.

## 2.6.0 — Close Runner Camera, Skybox and Advanced Slide

- Moved the gameplay camera closer to the runner, lowered its height and reduced its FOV while retaining speed-based pullback and lane-change lag.
- Added a bundled Amman golden-hour Skybox shader with sky gradient, horizon haze, sun disc and subtle animated clouds, plus a built-in procedural fallback.
- Expanded procedural Humanoid motion with run acceleration, sprint posture, footfall response, lane-change balance, multi-stage jump/fall, board-riding stance and richer idle motion.
- Rebuilt sliding with eased entry/hold/exit, an asymmetrical low pose, hand balance, one tucked leg and one extended leg.
- Added overhead-clearance checking so the runner does not stand up inside a low obstacle when a slide finishes.

## 2.5.0 — Complete Runtime Motion Set

- Added Humanoid muscle-driven poses directly on the supplied Meshy character skeleton.
- Added professional idle breathing, run-speed blending, jump ascent, airborne descent, landing compression and low slide/crouch motion.
- Added lane-change body lean, collision stumble, game-over fall and mission-complete celebration.
- Connected all motions automatically to the existing runner state, vertical velocity, impacts and mission events.
- Kept the supplied Running, Run_03 and Walking clips as the natural animation base, with procedural motion layers filling the missing actions.

## 2.4.0 — Reliable Color and Extensible Motions

- Replaced the fragile `.fbm` texture lookup with a stable `JordanianHero_BaseColor.png` resource.
- Added a forced runtime Lit material compatible with Built-in, URP and HDRP shaders.
- Added explicit errors and procedural fallback when the texture or a compatible shader is unavailable.
- Added automatic discovery of separate Idle, Jump, Slide and Stumble FBX motion files from the `Motions` resource folder.
- Added a fifth Playables state for impact/stumble and improved clip-name matching and fallbacks.
- Added Arabic instructions for exporting and adding more Meshy or Mixamo motions.

## 2.3.1 — Animation Module Fix

- Enabled Unity's built-in `com.unity.modules.animation` package so the runtime assembly can resolve `Animator`, `AnimationClip`, `AnimationMixerPlayable` and `AnimationClipPlayable`.

## 2.3.0 — Meshy Character Integration

- Integrated the supplied Meshy FBX character and its 4K base-color texture into the Amman project.
- Imported the supplied `Running`, `Run_03` and `Walking` animation stacks as a Humanoid Avatar.
- Added runtime Playables blending for ready, run, airborne and slide states without root motion.
- Added automatic forward-direction detection, ground alignment and consistent 2.16 m character scaling.
- Added optimized model and texture import settings plus project validation for the model, texture, clips and Avatar.
- Retained the procedural Jordanian hero as a safe runtime fallback.
- Added the required Meshy attribution notice for the CC BY 4.0 generation setting.

## 2.2.0 — Jordanian Hero

- Replaced the procedural low-detail runner with a substantially more detailed stylized Jordanian male hero.
- Added a fully articulated hierarchy for pelvis, spine, chest, neck, jaw, arms, hands, legs and feet.
- Added detailed eyes, blinking eyelids, brows, nose, ears, lips, teeth, hair, beard and moustache.
- Added layered jacket, shirt, belt, denim seams, detailed sneakers, fingers and thumbs.
- Added a red-and-white keffiyeh with neck wrap, knot, patterned articulated tails and tassels.
- Added cross-body leather bag, smart watch, Jordan-color bracelets, pendant and metallic details.
- Added secondary motion for keffiyeh, hair and bag, plus breathing, jaw exertion and watch pulse.
- Added a front hero showcase camera and a redesigned translucent start screen.
- Enabled GPU instancing and reduced shadow cost for small character details.

## 2.1.0 — Amman Al-Balad

- Added a complete Amman downtown visual variant with warm limestone materials and golden-hour lighting.
- Added procedural hillside homes, shops, awnings, balconies, rooftop tanks, satellite dishes and street signs.
- Added Roman Theatre, Citadel columns, Hashemite Plaza and painted-stair landmark scenes.
- Replaced the rail environment with an asphalt downtown street, sidewalks and black/yellow curbs.
- Added a downtown city bus, yellow taxi, produce cart, roadwork barrier and low market awning.
- Reworked the runner palette, HUD title, product identity and local save key.
- Added an original procedural rhythm and melody loop for the Amman theme.

## 2.0.0 — Neon Sands

- Replaced the capsule placeholder with a complete procedural low-poly human runner.
- Added procedural run, jump, slide, airborne, lean and impact animation.
- Added Pulse Board protection, coin magnet and temporary 2x score boost.
- Added progressive score multiplier, rotating run missions and expanded persistent statistics.
- Added Metro Pod obstacles, fair challenge patterns, coin arcs and zigzag trails.
- Added countdown, upgraded HUD, mission progress, power-up timers and richer results screen.
- Added procedural beat loop, new sound effects, optional mobile vibration, camera roll and impact shake.
- Added procedural sky, improved lighting, rails, curbs, gates, palms, lamps and illuminated buildings.
- Expanded Edit Mode coverage for multipliers and mission rotation.

## 1.0.0

- أول نسخة قابلة للعب.
- ثلاثة مسارات، قفز، انزلاق، عقبات، عملات ودرع.
- توليد طريق لا نهائي مع Object Pooling.
- تحكم لوحة مفاتيح ولمس وفأرة.
- نقاط وأعلى نتيجة محفوظة محلياً.
- واجهة متجاوبة ومؤثرات صوتية مولدة.
- إعدادات تصدير واختبارات Edit Mode.
