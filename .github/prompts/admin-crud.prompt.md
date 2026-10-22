---
agent: agent
description: Scaffold a municipality-scoped admin CRUD controller and views
---
Create admin CRUD for the `${input:entity:Entity name, e.g. Position}` entity in `Areas/Admin`.

Requirements:
- Controller in `Areas/Admin/Controllers/` with `[Area("Admin")]` and `[Authorize]` restricted to admin roles.
- Actions: Index, Details, Create (GET/POST), Edit (GET/POST), Delete (GET/POST).
- Apply the municipality scope service in **every** action. If the record is outside the user's municipality, return `NotFound()` or `Forbid()`.
- Use a view model for Create/Edit, data annotations for validation, and `[ValidateAntiForgeryToken]` on POSTs.
- Views use Bootstrap, include `_ValidationScriptsPartial`, and reuse the shared list/details/delete layouts in `Areas/Admin/Views/Shared/`.
- Handle restricted-delete failures with a friendly error instead of an exception page.

Follow `.github/copilot-instructions.md`.
