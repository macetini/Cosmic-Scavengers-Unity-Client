# Cosmic Scavengers — Unity Client

Unity client for the Cosmic Scavengers game. This project contains the Unity scenes, assets, scripts, and client-side logic that communicates with the Java server.

Status
- Draft README — please provide your Unity version and any project-specific build/integration details to finalize.
- Language: C# (Unity)

Overview
- Player input & controls
- Rendering & scene management
- Client-side game logic and UI
- Networking client to connect to the Java server (configure server address/port in project settings)

Requirements
- Unity Editor (recommend an LTS version — specify exact version used by the repo; e.g., 2021.3 LTS or later)
- Unity Hub recommended
- .NET / Mono (managed by Unity)
- Optional: Android/iOS SDKs for mobile builds, or platform-specific build tools

Quickstart
1. Clone the repo:
   git clone https://github.com/macetini/Cosmic-Scavengers-Unity-Client.git
2. Open in Unity:
   - Open Unity Hub → Add the cloned folder → Open project with the correct Unity version
   - If the project contains a ProjectVersion.txt in ProjectSettings, use that Unity version
3. Play in Editor:
   - Set the server IP/port in the project's configuration (see Configuration below)
   - Press Play in the Editor to run the client locally and connect to the server

Configuration
- Server address & port:
  - Look for a config file or scriptable object under Assets/Config or Assets/Scripts/ that holds server settings
  - Example: Assets/Config/NetworkConfig.asset (update to match actual repo)
- Input and control mapping:
  - Check Project Settings → Input Manager or the Input System package configuration
- Build settings:
  - File → Build Settings → Choose platform and scenes to include

Build & Run
- For Editor testing:
  - Use Play button after configuring server information
- For standalone build:
  - File → Build Settings → Choose target platform → Build
- Common build targets:
  - Windows: Build with target Windows, produce an .exe
  - macOS: Build for macOS
  - WebGL / Mobile: Add SDKs and configure player settings

Networking
- The client connects to the Java server. Document the exact transport and message formats here:
  - Transport used (WebSocket / TCP / UDP / UNET / Mirror / custom)
  - Protocol details (JSON / protobuf / binary)
  - Authentication steps (token exchange or simple username/password)

Project Structure (suggested)
- Assets/
  - Scenes/
  - Scripts/
  - Prefabs/
  - Art/
  - Config/
- ProjectSettings/
- Packages/

Testing
- Play mode testing in the Editor
- Unit tests (if using Unity Test Framework) — run via Window → Test Runner

Troubleshooting
- If the client fails to connect:
  - Ensure the server is running and reachable at the configured IP and port
  - Check firewall and networking configuration
  - Verify the transport and protocol versions match between client and server

Contributing
- Fork → branch → PR
- Follow the C# style conventions used in the project
- Provide reproducible steps for any bug fixes
- Add tests for new functionality where possible

License
- Include license information here.

Contact
- Maintainer: macetini
- Open issues on GitHub for bugs or feature requests

Notes / To do
- Specify exact Unity Editor version used by the project (ProjectSettings/ProjectVersion.txt)
- Document the networking library used (Mirror, MLAPI, custom) and message formats
- Add platform-specific build instructions (Windows/macOS/Android/iOS)
