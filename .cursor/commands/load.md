---
name: load
description: "Session lifecycle management with Serena MCP integration for project context loading"
category: session
complexity: standard
mcp-servers: [serena]
personas: []
---

# /load - Project Context Loading

## Triggers

- Session initialization and project context loading requests
- Cross-session persistence and memory retrieval needs
- Project activation and context management requirements
- Session lifecycle management and checkpoint loading scenarios

## Usage

```
/load [target] [--type project|config|deps|checkpoint] [--refresh] [--analyze]
```

## Behavioral Flow

1. **Context Detection**: Lightweight analysis of current working directory and recent file operations
2. **Session Requirements Analysis**: Determine if cross-session persistence or memory access needed
3. **Conditional Project Selection**: Only activate Serena if session persistence required
   - Memory access needed → Choose appropriate Serena project and activate
   - Simple session start → Skip Serena activation for performance
4. **Initialize**: Establish necessary MCP connections based on requirements
5. **Discover**: Analyze project structure and identify context loading requirements
6. **Load**: Retrieve project memories, checkpoints, and cross-session persistence data (if activated)
7. **Activate**: Establish project context and prepare for development workflow
8. **Validate**: Ensure loaded context integrity and session readiness

Key behaviors:

- **Smart Session Loading**: Conditional Serena activation only when memory access or persistence needed
- **Performance Optimization**: Skip unnecessary project switching for simple session starts
- **Dual-Project Support**: Seamless switching between Go backend and React frontend contexts when required
- **Serena MCP integration**: Memory management and cross-session persistence with project awareness (when activated)
- **Project activation**: Comprehensive context loading and validation for appropriate project
- **Performance-critical operation**: <200ms initialization for simple loads, <500ms for full context loading
- **Session lifecycle management**: Checkpoint and memory coordination with project context

## MCP Integration

- **Serena MCP**: Conditional integration for project activation, memory retrieval, and session management
  - **Activation Triggers**: Cross-session persistence, checkpoint loading, memory access requests
  - **Skip Conditions**: Simple session starts, basic project loading without memory requirements
- **Memory Operations**: Cross-session persistence, checkpoint loading, and context restoration (when activated)
- **Performance Critical**: <200ms for simple loads, <500ms for full context loading, <1s for checkpoint creation

## Tool Coordination

- **activate_project**: Core project activation and context establishment
- **list_memories/read_memory**: Memory retrieval and session context loading
- **Read/Grep/Glob**: Project structure analysis and configuration discovery
- **Write**: Session context documentation and checkpoint creation

## Key Patterns

- **Project Activation**: Directory analysis → memory retrieval → context establishment
- **Session Restoration**: Checkpoint loading → context validation → workflow preparation
- **Memory Management**: Cross-session persistence → context continuity → development efficiency
- **Performance Critical**: Fast initialization → immediate productivity → session readiness

## Examples

### Basic Project Loading

```
/load
# Loads current directory project context with Serena memory integration
# Establishes session context and prepares for development workflow
```

### Specific Project Loading

```
/load /path/to/project --type project --analyze
# Loads specific project with comprehensive analysis
# Activates project context and retrieves cross-session memories
```

### Checkpoint Restoration

```
/load --type checkpoint --checkpoint session_123
# Restores specific checkpoint with session context
# Continues previous work session with full context preservation
```

### Dependency Context Loading

```
/load --type deps --refresh
# Loads dependency context with fresh analysis
# Updates project understanding and dependency mapping
```

## Boundaries

**Will:**

- Load project context using Serena MCP integration for memory management
- Provide session lifecycle management with cross-session persistence
- Establish project activation with comprehensive context loading

**Will Not:**

- Modify project structure or configuration without explicit permission
- Load context without proper Serena MCP integration and validation
- Override existing session context without checkpoint preservation
