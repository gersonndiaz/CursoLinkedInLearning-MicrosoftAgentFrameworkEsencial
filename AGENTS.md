# AGENTS.md

Guía para agentes de IA (Claude Code, Copilot, etc.) que trabajen en este repositorio.

## Contexto

Repositorio de aprendizaje del curso **Microsoft Agent Framework Esencial** (LinkedIn Learning). Cada subdirectorio es un proyecto independiente de .NET que corresponde a un paso o lección del curso.

## Stack

- C# / .NET 10 (`net10.0`), `ImplicitUsings` y `Nullable` habilitados.
- Microsoft Agent Framework (paquetes `Microsoft.Agents.AI.*`).
- Proveedor de modelos: OpenAI (`OPENAI_API_KEY` por variable de entorno).

## Estructura

```
/
├── AGENTS.md
├── README.md
├── .gitignore
└── <proyecto>/          # un proyecto por lección
    ├── <proyecto>.csproj
    ├── <proyecto>.slnx
    └── Program.cs
```

## Comandos

```bash
dotnet restore    # dentro de la carpeta del proyecto
dotnet build
dotnet run
```

## Convenciones

- Idioma: comentarios, prompts y documentación en español.
- Cada proyecto nuevo va en su propia carpeta y se añade a la tabla de proyectos del `README.md`.
- Mantener los ejemplos simples y didácticos; preferir top-level statements en `Program.cs`.
- **Nunca** escribir API keys ni secretos en el código o en archivos versionados. Usar variables de entorno o `dotnet user-secrets`.
- No versionar `bin/`, `obj/` ni archivos locales del IDE (ver `.gitignore`).
