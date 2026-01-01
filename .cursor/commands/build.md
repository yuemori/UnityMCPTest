---
name: build
description: "Build Unity projects for multiple platforms with intelligent error handling and optimization"
category: utility
complexity: enhanced
mcp-servers: []
personas: [devops-engineer]
---

# /build - Unity Project Building and Packaging

## Triggers

- Unity project build requests for different platforms (Windows, macOS, iOS, Android, WebGL, etc.)
- Build optimization and asset bundle generation needs
- Error debugging during build processes
- Deployment preparation and artifact packaging requirements

## Usage

```
/build [platform] [--type development|release] [--clean] [--addressables] [--verbose]
```

## Behavioral Flow

1. **Analyze**: Project structure, build settings, and platform configurations
2. **Validate**: Build environment, SDK installations, and required dependencies
3. **Execute**: Build process with real-time monitoring and error detection
4. **Optimize**: Build artifacts, apply compression, and minimize build size
5. **Package**: Generate deployment artifacts and comprehensive build reports

Key behaviors:

- Platform-specific build orchestration with dependency validation
- Intelligent error analysis with actionable resolution guidance
- Development/Release configuration handling with proper settings
- Comprehensive build reporting with timing metrics and artifact analysis

## MCP Integration

- **DevOps Engineer Persona**: Activated for build optimization and deployment preparation
- **Enhanced Capabilities**: Build pipeline integration, asset validation, size analysis

## Tool Coordination

- **Bash**: Unity Editor batch mode execution and process management
- **Read**: Build configuration analysis and settings inspection
- **Grep**: Error parsing and build log analysis
- **Glob**: Artifact discovery and validation
- **Write**: Build reports and deployment documentation

## Key Patterns

- **Platform Builds**: Windows/macOS/iOS/Android/WebGL → appropriate SDK and configuration
- **Error Analysis**: Build failures → diagnostic analysis and resolution guidance
- **Optimization**: Asset analysis → compression and size reduction
- **Validation**: Build verification → quality gates and deployment readiness

## Examples

### Standard Development Build

```
/build windows
# Builds for Windows using development configuration
# Generates executable with debug symbols
```

### Release Build with Optimization

```
/build android --type release --clean
# Clean release build for Android platform
# Applies IL2CPP, compression, and size optimization
```

### Multi-Platform Build

```
/build ios --type release --addressables
# iOS release build with Addressables asset bundles
# Generates Xcode project ready for App Store submission
```

### WebGL Build

```
/build webgl --type release --verbose
# WebGL build with detailed output
# Applies compression and streaming assets optimization
```

### Build with Addressables

```
/build windows --addressables
# Windows build with Addressables content build
# Generates asset bundles and content catalog
```

## Unity Build Commands Reference

### Editor Batch Mode

```bash
# Windows Development Build
Unity.exe -batchmode -projectPath . -buildTarget Win64 -executeMethod BuildScript.Build -quit

# Android Release Build
Unity.exe -batchmode -projectPath . -buildTarget Android -executeMethod BuildScript.BuildRelease -quit

# iOS Build (generates Xcode project)
Unity.exe -batchmode -projectPath . -buildTarget iOS -executeMethod BuildScript.BuildiOS -quit
```

### Common Build Targets

| Platform | Build Target | Output        |
| -------- | ------------ | ------------- |
| Windows  | Win64        | .exe          |
| macOS    | OSXUniversal | .app          |
| Linux    | Linux64      | Executable    |
| Android  | Android      | .apk / .aab   |
| iOS      | iOS          | Xcode Project |
| WebGL    | WebGL        | HTML5         |

## Error Resolution Patterns

### Common Build Errors

- **Missing SDK**: Ensure platform SDK is installed via Unity Hub
- **Script Compilation**: Check for C# errors in Unity Console
- **Asset Import**: Verify asset formats and import settings
- **IL2CPP Errors**: Check for reflection usage and stripping levels

### Build Size Optimization

- Enable code stripping (Link.xml for preserved types)
- Use texture compression appropriate for platform
- Configure Addressables for asset loading
- Remove unused assets and packages

## Boundaries

**Will:**

- Execute Unity builds using batch mode and existing configurations
- Provide comprehensive error analysis and optimization recommendations
- Generate deployment-ready artifacts with detailed reporting

**Will Not:**

- Modify Unity project settings or create new build configurations
- Install missing SDKs or platform modules
- Execute deployment operations beyond artifact preparation
