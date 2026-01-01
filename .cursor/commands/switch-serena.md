---
name: switch-serena
description: "Smart Serena project switching with context detection and validation"
category: session
complexity: basic
mcp-servers: [serena]
personas: []
---

# /switch-serena - Serena Project Switching

## Triggers

- Manual Serena project switching between Go backend and React frontend
- Context validation and project activation confirmation needs
- Cross-project development workflow management
- Project context debugging and verification scenarios

## Usage

```
/switch-serena [go|frontend|auto] [--analyze] [--validate] [--force]
```

## Behavioral Flow

1. **Context Analysis**: Examine current working directory and file operations
2. **Project Detection**: Identify appropriate Serena project based on context
3. **Validation**: Verify project availability and configuration
4. **Switch**: Activate target Serena project with proper context loading
5. **Confirm**: Validate successful project activation and context establishment

Key behaviors:

- **Smart Detection**: Auto-detect appropriate project from file context and working directory
- **Seamless Switching**: Fast project activation with context preservation
- **Validation**: Ensure project availability and proper configuration before switching
- **Cross-Project Awareness**: Maintain understanding of both Go and Frontend project states

## Context Detection Rules

### Go Backend Context

- **File Patterns**: `*.go`, `pkg/`, `cmd/`, `proto/`, root directory operations
- **Target Project**: "portalkey-server"
- **Indicators**: Go imports, ConnectRPC services, Spanner operations

### Frontend Context

- **File Patterns**: `frontend/`, `*.tsx`, `*.ts` (in frontend), React components
- **Target Project**: "portalkey-server-frontend"
- **Indicators**: React imports, TypeScript interfaces, Redux operations

### Auto-Detection Logic

- **Priority**: Current file operations > working directory > last active project
- **Fallback**: Prompt user for clarification if context is ambiguous
- **Memory**: Remember user preferences for mixed-context scenarios

## MCP Integration

- **Serena MCP**: Core project activation and context switching functionality
- **Project Validation**: Verify project availability and configuration integrity
- **Memory Operations**: Preserve context across project switches when possible

## Tool Coordination

- **activate_project**: Core Serena project activation with validation
- **list_memories**: Verify project-specific memory availability
- **Read/Grep**: Context analysis for intelligent project detection
- **Write**: Project switch logging and preference persistence

## Examples

### Auto-Detection Switch

```
/switch-serena auto
# Analyzes current context and switches to appropriate Serena project
# Go files → "portalkey-server", Frontend files → "portalkey-server-frontend"
```

### Manual Project Switch

```
/switch-serena go --validate
# Explicitly switches to "portalkey-server" with validation
# Confirms project availability and proper configuration
```

### Frontend Development Switch

```
/switch-serena frontend --analyze
# Switches to "portalkey-server-frontend" with context analysis
# Loads React/TypeScript project context and memories
```

### Force Switch with Validation

```
/switch-serena go --force --validate
# Forces switch to Go project even in ambiguous context
# Validates project state and reports any configuration issues
```

## Boundaries

**Will:**

- Switch between PortalKey Go and Frontend Serena projects intelligently
- Provide context detection and validation for appropriate project selection
- Maintain cross-project awareness and context preservation

**Will Not:**

- Switch projects without proper validation and context analysis
- Override explicit user project selection without confirmation
- Operate without proper Serena MCP integration and project availability
