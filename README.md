# Microsoft Agent Framework Esencial

Repositorio con los proyectos prácticos que voy desarrollando durante el curso [**Microsoft Agent Framework Esencial**](https://www.linkedin.com/learning/microsoft-agent-framework-esencial) de LinkedIn Learning.

## Progreso

- [x] Módulo 1
- [x] Módulo 2
- [x] Módulo 3
- [ ] Módulo 4
- [ ] Módulo 5

## Tecnologías

- [.NET 10](https://dotnet.microsoft.com/)
- [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) (`Microsoft.Agents.AI.*`)
- OpenAI API

## Proyectos

| Proyecto | Descripción |
|----------|-------------|
| [myfirstagent](myfirstagent/) | Primer agente: crea un `ChatClientAgent` sobre OpenAI con opciones de chat (tokens máximos, nivel de razonamiento), responde en streaming sobre un PDF adjunto y muestra el uso de tokens. |

## Requisitos

- .NET SDK 10
- Una API key de OpenAI

## Configuración

Las credenciales **nunca** se guardan en el repositorio. Define la variable de entorno antes de ejecutar:

```bash
export OPENAI_API_KEY="tu-api-key"
```

## Ejecutar un proyecto

```bash
cd myfirstagent
dotnet run
```
