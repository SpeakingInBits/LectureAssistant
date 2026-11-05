## Installing

Download the file for your computer from **Assets** below.

- [`SpeakingInBits.LectureAssistant-win-x64-Setup.exe`](https://github.com/SpeakingInBits/LectureAssistant/releases/download/v{{VERSION}}/SpeakingInBits.LectureAssistant-win-x64-Setup.exe) for most PCs.
- [`SpeakingInBits.LectureAssistant-win-arm64-Setup.exe`](https://github.com/SpeakingInBits/LectureAssistant/releases/download/v{{VERSION}}/SpeakingInBits.LectureAssistant-win-arm64-Setup.exe) for PCs with an Arm processor, such as Snapdragon.

The app isn't code-signed yet, so Windows warns you the first time. That's expected.

1. Double-click the Setup file. If Windows says "Windows protected your PC", click **More info**, then **Run anyway**.
2. The app installs for your Windows account and opens. If your PC doesn't have the Microsoft Visual C++ runtime yet, Setup installs that first, which can ask for an administrator's approval.
3. Next time, open **Lecture Assistant** from the Start menu or the desktop.

After that, the app keeps itself up to date. When a new version is out, it downloads it in the background and installs it the next time you open the app (or right away with **Restart now**). Uninstalling the app keeps your lectures and downloaded models.

If you can't run installers, download [`SpeakingInBits.LectureAssistant-win-x64-Portable.zip`](https://github.com/SpeakingInBits/LectureAssistant/releases/download/v{{VERSION}}/SpeakingInBits.LectureAssistant-win-x64-Portable.zip) (or [`SpeakingInBits.LectureAssistant-win-arm64-Portable.zip`](https://github.com/SpeakingInBits/LectureAssistant/releases/download/v{{VERSION}}/SpeakingInBits.LectureAssistant-win-arm64-Portable.zip) for Arm) instead, right-click it, choose **Extract All**, and double-click `Lecture Assistant.exe` in the extracted folder. Transcribing and local question writing need the [Microsoft Visual C++ runtime](https://learn.microsoft.com/cpp/windows/latest-supported-vc-redist), which the portable version can't install for you.

The other files in **Assets** (`.nupkg`, `.json`) are what installed apps download to update. You don't need them.
