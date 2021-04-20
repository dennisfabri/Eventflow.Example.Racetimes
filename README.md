# Racetimes example for Eventflow

This project contains an example project that uses some basic and extended features.

Basic features:
- Command / CommandHandler
- Event
- Identity
- AggregateRoot
- ReadModel

Configuration:
- MSSQL
- EntityFramework
- EventFlowOptions
- Migration

Extended features:
- Entity (within AggregateRoot)
- ReadModel for an Entity
- Delete on ReadModel
- Snapshots

# Racetimes (Domain)

The domain of this project is storing times from races within competitions. Therefore competitions can be created, renamed and deleted. Racetimes (Entries) can be added and changed. These actions are far from complete but I think they are sufficient for an example.

# Note

This is still a work in progress but I enjoy hearing from you (especially feedback on points I missed, got wrong or could do better).

# Sources

The code is based on the official documentation as well as code from the tests within the [eventflow repository](https://github.com/eventflow/EventFlow).
