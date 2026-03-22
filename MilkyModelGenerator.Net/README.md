# MilkyModelGenerator.Net

IR to C# models generator for the Milky IR schema.

Output layout:

- `Common`: shared structs and unions from `commonStructs`
- `System/Requests`, `System/Responses`
- `Message/Requests`, `Message/Responses`
- `Friend/Requests`, `Friend/Responses`
- `Group/Requests`, `Group/Responses`
- `File/Requests`, `File/Responses`

Default output:

```powershell
.\output\Generated
```

Default namespace:

```powershell
Milky.Models
```

Run directly:

```powershell
dotnet run --project .\MilkyModelGenerator.Net.csproj
```

Interactive wrapper:

```powershell
.\generate-models.ps1
```

Default wrapper usage:

```powershell
.\generate-models.ps1 -Preset Default
```

Custom target example:

```powershell
dotnet run --project .\MilkyModelGenerator.Net.csproj -- `
  --output C:\path\to\Generated `
  --namespace My.Models `
  --ir-url https://milky.ntqqrev.org/raw/milky-ir/ir.json `
  --ir-source milky-ir/ir.json
```

Equivalent wrapper usage:

```powershell
.\generate-models.ps1 -Preset Custom `
  -Output C:\path\to\Generated `
  -Namespace My.Models `
  -IrUrl https://milky.ntqqrev.org/raw/milky-ir/ir.json `
  -IrSource milky-ir/ir.json
```
