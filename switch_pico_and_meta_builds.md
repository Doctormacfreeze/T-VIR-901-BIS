# VR Platform Switching Guide (Meta Quest 3 ↔ Pico 4 Ultra)

This guide explains how to switch your Unity OpenXR settings when building for **Meta Quest 3** or **Pico 4 Ultra**.

---

## ▶️ Meta Quest 3 → Pico 4 Ultra

1. Open **Project Settings → XR Plug-in Management → OpenXR → Android**
2. **Enable**:
   - `PICO XR Feature Group`
3. **Disable**:
   - `Meta Quest Support`
4. Under **Interaction Profiles**:
   - Remove all profiles
   - Add:
     - `Pico 4 Ultra Touch Controller Profile`
5. Build your Android app.

---

## ▶️ Pico 4 Ultra → Meta Quest 3

1. Open **Project Settings → XR Plug-in Management → OpenXR → Android**
2. **Disable**:
   - `PICO XR Feature Group`
3. **Enable**:
   - `Meta Quest Support`
4. Under **Interaction Profiles**:
   - Remove all profiles
   - Add:
     - `Oculus Touch Controller Profile`
5. Build your Android app.

---

## Notes
- Meta and Pico OpenXR features **cannot be enabled at the same time**.
- Each platform requires its own interaction profile.
- Leaving both enabled will cause blocking errors during build.
