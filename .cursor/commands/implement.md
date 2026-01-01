---
name: implement
description: "Feature and code implementation with intelligent persona activation and MCP integration for Unity/C# projects"
category: workflow
complexity: standard
mcp-servers: [context7, sequential]
personas:
  [
    unity-expert,
    csharp-expert,
    unity-ui-expert,
    system-architect,
    security-engineer,
    quality-engineer,
  ]
---

# /implement - Feature Implementation

> **Context Framework Note**: This behavioral instruction activates when Cursor users type `/implement` patterns. It guides Cursor to coordinate specialist personas and MCP tools for comprehensive Unity/C# implementation.

## Triggers

- Feature development requests for Unity components, systems, or complete functionality
- Code implementation needs with Unity-specific requirements
- Multi-domain development requiring coordinated expertise
- Implementation projects requiring testing and validation integration

## Context Trigger Pattern

```
/implement [feature-description] [--type component|system|editor|feature] [--focus unity|csharp|ui] [--safe] [--with-tests]
```

**Usage**: Type this in Cursor conversation to activate implementation behavioral mode with coordinated expertise and systematic development approach.

## Behavioral Flow

1. **Context Detection**: Lightweight analysis of target files/directories for project context
2. **Focus Selection**: Apply --focus override or auto-detect from context
   - `--focus unity` → Force unity-expert persona and Unity patterns
   - `--focus csharp` → Force csharp-expert persona and C# patterns
   - `--focus ui` → Force unity-ui-expert persona and UI patterns
   - Auto-detect → Use context-based focus detection (default)
3. **Requirements Analysis**: Examine implementation scope and determine tool requirements
4. **Plan**: Choose approach and activate relevant personas for domain expertise
5. **Generate**: Create implementation code with Unity/C#-specific best practices
6. **Validate**: Apply security and quality validation throughout development
7. **Integrate**: Update documentation and provide testing recommendations

Key behaviors:

- **Focus Override Support**: Simple --focus unity|csharp|ui flag for explicit control when needed
- **Performance Optimization**: Consider Unity-specific performance constraints from the start
- **Context-based persona activation**: Auto-detect appropriate experts or use focus override
- **Unity-specific implementation**: Context7 MCP integration with Unity/C# patterns
- **Systematic multi-component coordination**: Sequential MCP with cross-system understanding

## MCP Integration

- **Context7 MCP**: Unity documentation, C# patterns, and official Unity best practices
- **Sequential MCP**: Complex multi-step analysis and implementation planning

## Tool Coordination

- **Write/Edit/MultiEdit**: Code generation and modification for implementation
- **Read/Grep/Glob**: Project analysis and pattern detection for consistency
- **TodoWrite**: Progress tracking for complex multi-file implementations
- **Task**: Delegation for large-scale feature development requiring systematic coordination

## Key Patterns

- **Context Detection**: Unity project structure → appropriate persona and MCP activation
- **Implementation Flow**: Requirements → code generation → validation → integration
- **Multi-Persona Coordination**: Unity + C# + UI → comprehensive solutions
- **Quality Integration**: Implementation → testing → documentation → validation

## Examples

### Unity Component Implementation

```
/implement player controller --type component --focus unity
# Focus override: --focus unity → Force unity-expert persona
# Context detection: Assets/Scripts/ → Unity context confirmed
# Requirements analysis: MonoBehaviour component with Input System
# unity-expert persona ensures best practices and performance
```

### ScriptableObject System Implementation

```
/implement inventory system --type system --focus unity --with-tests
# Focus override: --focus unity → Force unity-expert persona
# Context detection: Assets/Scripts/Systems/ → Unity system context
# Requirements analysis: ScriptableObject-based data architecture
# unity-expert persona handles data-driven design patterns
```

### Editor Tool Implementation

```
/implement level editor window --type editor --safe
# Context detection: Assets/Editor/ → Unity Editor context
# Requirements analysis: Custom EditorWindow with inspector integration
# unity-expert persona ensures Editor API best practices
```

### UI System Implementation

```
/implement main menu UI --type feature --focus ui
# Focus override: --focus ui → Force unity-ui-expert persona
# Context detection: UI Toolkit or UGUI based on project
# unity-ui-expert persona handles responsive design and optimization
```

### Full Feature Implementation

```
/implement save/load system --type feature --with-tests
# Multi-persona coordination: unity-expert, csharp-expert
# Sequential MCP breaks down complex implementation steps
# Covers serialization, file I/O, and Unity integration
```

### Auto-Detection vs Focus Override

```
/implement enemy AI system --type system
# Auto-detection: MonoBehaviour + ScriptableObject patterns
# Context-based selection of unity-expert + csharp-expert

/implement enemy AI system --type system --focus csharp
# Focus override: --focus csharp → Force csharp-expert focus
# Emphasizes C# patterns like State Machine and Strategy Pattern
```

## Boundaries

**Will:**

- Implement features with intelligent persona activation and MCP coordination
- Apply Unity/C#-specific best practices and performance validation
- Provide comprehensive implementation with testing and documentation integration

**Will Not:**

- Make architectural decisions without appropriate persona consultation
- Implement features conflicting with Unity performance constraints
- Override user-specified safety constraints or bypass quality gates
