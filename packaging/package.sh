#!/usr/bin/env bash
# Builds the app as a self-contained folder for each runtime and packages it with Velopack, so installed copies can
# update themselves from the GitHub releases.
#
#   VERSION=1.2.3 packaging/package.sh <rid>...
#
# Run from the repository root in Git Bash, with vpk (dotnet tool install -g vpk) on the PATH at the same version as the
# Velopack package in Directory.Packages.props. Each runtime is its own Velopack channel, named after the runtime, so an
# installed app only ever updates to its own architecture's package. Writes to dist/:
#
#   SpeakingInBits.LectureAssistant-<rid>-Setup.exe      installer (installs for the current user)
#   SpeakingInBits.LectureAssistant-<rid>-Portable.zip   a folder that runs from anywhere and still updates
#   SpeakingInBits.LectureAssistant-<version>-<rid>-full.nupkg, releases.<rid>.json   what installed apps download to update
set -euo pipefail

version=${VERSION:?set VERSION, e.g. VERSION=1.2.3}
mkdir -p dist
dist=$(cd dist && pwd)

for rid in "$@"; do
  case "$rid" in
    win-x64)   platform=x64 ;;
    win-arm64) platform=ARM64 ;;
    *)
      echo "Don't know how to package $rid." >&2
      exit 1
      ;;
  esac

  out="out/$rid/LectureAssistant"
  rm -rf "out/$rid"

  # Unpackaged, with the Windows App SDK inside the app, so it needs neither an MSIX nor the SDK's runtime installed.
  # Not a single file: Whisper.net and LLamaSharp load their native libraries from the runtimes folder.
  dotnet publish src/LectureAssistant.App --configuration Release --runtime "$rid" --self-contained --output "$out" \
    -p:Platform="$platform" -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true \
    -p:DebugType=none -p:Version="$version"

  # Some native packages ship their own debug symbols, which instructors don't need.
  find "$out" -name '*.pdb' -delete

  # Whisper.net and LLamaSharp ship every platform's native libraries, which this app never loads.
  find "$out/runtimes" "$out/runtimes/vulkan" -mindepth 1 -maxdepth 1 -type d ! -name "$rid" ! -name vulkan -exec rm -rf {} +
  rm -f "$out/ggml-metal.metal"

  # The packId sets the install folder, %LOCALAPPDATA%\<packId>, which uninstalling empties. It must not be
  # LectureAssistant, the folder the app keeps its data in (see AppPaths.cs).
  # The native libraries need the Visual C++ runtime, which Setup installs if the PC doesn't have it.
  # No delta packages: each update downloads the whole app, which keeps the release simple.
  # The icon is Setup.exe's; the app's own icon is built into LectureAssistant.exe.
  vpk pack --packId SpeakingInBits.LectureAssistant --packTitle "Lecture Assistant" --packAuthors "Speaking In Bits" \
    --packVersion "$version" --channel "$rid" --runtime "$rid" --delta None --outputDir "$dist" \
    --packDir "$out" --mainExe LectureAssistant.exe --framework "vcredist143-${rid#win-}" \
    --icon src/LectureAssistant.App/Assets/AppIcon.ico
done
