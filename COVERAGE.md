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

| Component | Coverage |
| --- | --- |
| Credfeto.Notification.Bot.Server | 41.30% |
| Credfeto.Notification.Bot.Shared | 100.00% |
| Credfeto.Notification.Bot.Twitch.DataTypes | 100.00% |
| Credfeto.Notification.Bot.Twitch.Models | 100.00% |
| Credfeto.Notification.Bot.Twitch | 78.71% |
| **Overall** | **77.43%** |
