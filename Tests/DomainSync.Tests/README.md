# Domain synchronization

Run `dotnet run --project Tests/DomainSync.Tests` from the repository root.
The tests use the actual MAUI HTTP client with a fake HTTP server handler and minimal
platform stubs; no credentials, device or database are required.

The domain timestamp lives only in the web process memory. A restart creates a fresh
timestamp. It changes after EF saves, transaction commits, bulk writes and domain
cache invalidation. Direct database edits from outside Diva2 cannot update this cache.
Multiple web processes would need a shared cache instead of process-local memory.

MAUI checks immediately and every 10 seconds while the window is active. It downloads
lessons, branch configuration, reservations, permitted reservation participants and
credits before replacing its session snapshot. The timestamp is intentionally kept
with that in-memory snapshot so a fresh session always downloads its data again.
