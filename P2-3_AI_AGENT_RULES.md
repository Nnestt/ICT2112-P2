# PRO-RENTAL PROJECT: STRICT ARCHITECTURE & AI AGENT INSTRUCTIONS
**TARGET AUDIENCE:** Developers and AI Coding Assistants (Team P2-3 / Module 2).
**SYSTEM PROMPT:** You are an expert .NET enterprise developer. You MUST strictly adhere to the following architectural constraints, file structures, and naming conventions. Deviation is strictly prohibited.

## 1. THE 3-LAYERED ARCHITECTURE (STRICT ENFORCEMENT)
* **FORBIDDEN:** Controllers MUST NEVER instantiate or inject `AppDbContext` directly. 
* **REQUIRED FLOW:** Controllers -> Interface (`I...Control`) -> Control Class -> Interface (`I...Mapper`) -> Mapper Class -> `AppDbContext`.
* **DEPENDENCY INJECTION:** If a feature "works" but isn't registered in `Program.cs`, it is a violation. All Interfaces, Controls, and Mappers MUST be explicitly registered in `Program.cs` under the `//Team P2-3` section.

## 2. ENTITIES AND DATABASE CONTEXT (DO NOT TOUCH SCAFFOLDED FILES)
* **SCAFFOLDED ENTITIES:** Files inside `Domain/Entities/` are auto-generated. **DO NOT EDIT THEM.**
* **MANUAL PARTIALS:** All custom entity logic MUST go into `Domain/P2-3/Entities/`.
* **ENCAPSULATION RULE:** Entity properties with capitalized names (e.g., `Name`, `Status`) are strictly for EF Core mapping. Business logic and controllers MUST use the private backing fields (e.g., `_name`) or the custom getter/setter methods defined in the manual partial classes (e.g., `.GetName()`, `.UpdateStatus()`).
* **DB CONTEXT:** `Data/UnitOfWork/AppDbContext.cs` is auto-generated. **DO NOT EDIT IT.** * **CUSTOM DB CONTEXT:** All enums and custom model building MUST be placed in `Data/UnitOfWork/AppDbContext.custom.cs`. Best if there is no need to touch both these files at all, especially since they are both comprehensive enough to cover the whole database, columns and enums.

## 3. STRICT NAMESPACE MAPPING
You must use the exact namespaces below based on the class type. Do not invent new namespaces.
* **Data Gateways & Mappers:** `namespace ProRental.Data;`
* **Scaffolded & Manual Entities:** `namespace ProRental.Domain.Entities;`
* **Control Classes:** `namespace ProRental.Domain.Controls;`
* **All Interfaces:** `namespace ProRental.Interfaces.Data;` OR `namespace ProRental.Interfaces.Domain;`
* **Controllers:** `namespace ProRental.Controllers;`

## 4. STRICT FILE LOCATION DIRECTORY
Place new files exactly in these directories.
* **Mappers:** `Domain/Module2/Gateways/*.cs`
* **Data Interfaces (IMappers):** `Domain/Module2/Interfaces/*.cs`
* **DbContexts:** `Data/UnitOfWork/`
* **Scaffolded Entities (Read-Only):** `Domain/Entities/`
* **Manual Partial Entities (Edit Here):** `Domain/P2-3/Entities/`
* **Control Classes:** `Domain/Module2/Controls/` (Subfolders allowed here)
* **Domain Interfaces (IControls):** `Interfaces/P2-3/`
* **Controller Classes:** `Controllers/Module2/*.cs`
* **Views:** `Views/Home/` AND `Views/Module2/`
* **Program Registration:** `Program.cs` (Root directory)

## 6. DATETIME & TIMESTAMPTZ HANDLING (STRICT POSTGRESQL RULES)
PostgreSQL strictly enforces time zones. You MUST follow these rules to prevent Npgsql conversion crashes:
* **The Rule of UTC:** NEVER use `DateTime.Now`. All internal system times must be generated using `DateTime.UtcNow`.
* **User Input (Forms/Views):** Any `DateTime` received from an HTTP request or form submission MUST be explicitly forced to UTC before sending it to a Mapper or Domain Control. You must use `DateTime.SpecifyKind(dateVariable, DateTimeKind.Utc)` or `.ToUniversalTime()`.
* **Calculations:** Perform all date math (duration, expiry, overdue checks) strictly in UTC.
* **Displaying to Users:** Only convert UTC dates to local time at the very last step in the Presentation Layer (inside the Razor View or Controller) using `.ToLocalTime()` or standard formatting strings.

## 6. CODE REVIEW CHECKLIST FOR AI AGENTS
Before generating or finalizing code, verify:
1. Did I bypass the Domain layer by putting DbContext in a Controller? (If yes, rewrite).
2. Did I use `entity.Name` instead of `entity.GetName()` or `_name`? (If yes, rewrite).
3. Did I put a new interface in the `ProRental.Data` namespace instead of `ProRental.Interfaces.Data`? (If yes, fix the namespace).
4. Did I modify a scaffolded entity instead of the partial class in `P2-3`? (If yes, move the code).
5. Did I use `DateTime.Now` or pass an unspecified DateTime to the database instead of enforcing `DateTime.UtcNow` or `DateTimeKind.Utc`? (If yes, rewrite).