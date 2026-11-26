# Code Coverage Baseline

This file tracks the coverage ratchet baseline. Future PRs must keep each language's
`Overall` figure at or above the value recorded here; it is updated after each PR
whose AI Coverage phase passes.

Method: pooled `lines-covered` / `lines-valid` (cobertura) summed across all
`*.Tests` unit-test projects (integration and benchmark test projects excluded),
one `dotnet test ... --coverage --coverage-output-format cobertura` run per
project. `dotnet reportgenerator` was unavailable in the environment this
baseline was measured in, so the cobertura XML totals were read directly rather
than via a generated report; `coverage-ratchet.instructions.md` (referenced by
the orchestrator's AI Coverage phase) does not exist in this repo's `ai/`
tree — this baseline and method were established ad hoc pending that file
being added.

## .NET

| Project | Line Coverage | Covered | Coverable |
| --- | --- | --- | --- |
| Credfeto.Notification.Bot.Server | 83.2% | 104 | 125 |
| Credfeto.Notification.Bot.Shared | 100.0% | 7 | 7 |
| Credfeto.Notification.Bot.Twitch | 82.0% | 740 | 902 |
| Credfeto.Notification.Bot.Twitch.DataTypes | 100.0% | 16 | 16 |
| Credfeto.Notification.Bot.Twitch.Models | 100.0% | 68 | 68 |
| **Overall (.NET)** | **83.6%** | **935** | **1118** |

---

Captured at commit `84f4ac9b7fae0f8370aa8983c6ab78ca420d793d` on 2026-11-26.
