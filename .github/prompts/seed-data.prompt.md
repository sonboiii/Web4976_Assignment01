---
agent: agent
description: Add or update development seed data
---
Update the seed data in `Data/` to add: ${input:what:What to seed, e.g. a new municipality with positions and candidates}.

Requirements:
- Seed only when the relevant table is empty, so startup stays idempotent.
- Respect unique rules: municipality names, and position names within a municipality.
- Use fictional candidates only, with realistic profiles, taglines and optional website/social/video links.
- For a new municipality, also create its admin role (`"{Name} Admin"`) and, if requested, an admin user with `MunicipalityId` set.
- Keep the required accounts unchanged: `super` (Super Admin) and `pm` (Pitt Meadows Admin), password `P@$$w0rd`.

Follow `.github/copilot-instructions.md`.
