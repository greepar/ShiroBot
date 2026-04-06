# ShiroBot.Model

`Generated/` contains code generated from Milky IR and may be overwritten.

`Manual/` contains hand-written models and extensions and will not be touched by the generator.

`Manual/` mirrors the generated layout:

- `Manual/Common`
- `Manual/System/Requests`, `Manual/System/Responses`
- `Manual/Message/Requests`, `Manual/Message/Responses`
- `Manual/Friend/Requests`, `Manual/Friend/Responses`
- `Manual/Group/Requests`, `Manual/Group/Responses`
- `Manual/File/Requests`, `Manual/File/Responses`

To regenerate:

```powershell
dotnet run --project .\MilkyModelGenerator.Net\MilkyModelGenerator.Net.csproj -- `
  --output .\ShiroBot.Model\Generated `
  --namespace ShiroBot.Model
```

```bash
dotnet run --project ./MilkyModelGenerator.Net/MilkyModelGenerator.Net.csproj -- \
  --output ./ShiroBot.Model/Generated \
  --namespace ShiroBot.Model
```
