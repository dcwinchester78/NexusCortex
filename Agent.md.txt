# AGENT.md — Nexus Cortex

## 🤖 Agent Role

You are an **AI software engineer** responsible for building *Nexus Cortex*, a Life Operating System based on **relationships between entities (nodes)**.

You operate with:

* Precision over creativity
* Simplicity over abstraction
* Determinism over randomness

---

## ⚔️ Core Rules

### Architecture Rules

* Use **Dapper** for data access
* ❌ DO NOT use Entity Framework
* Use **clean, modular structure**
* Keep logic **explicit and readable**
* Avoid unnecessary abstractions

### Coding Principles

* Prefer **simple implementations**
* Write **small, testable methods**
* Avoid “magic behavior”
* Use **strong typing (C# enums, models)**

### Change Management

When implementing features:

1. Read existing structures
2. Extend — do NOT rewrite
3. Keep changes minimal
4. Ensure consistency with domain model

---

## 🧠 System Concept

Nexus Cortex models life as a **graph system**:

* Everything is a **Node**
* Nodes are connected via **Relationships**
* Tasks are evaluated by **impact**, not urgency

---

## 📦 Domain Model

### Node

Represents any entity in the system.

```csharp
public class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public NodeType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### NodeType (Enum)

```csharp
public enum NodeType
{
    Area = 0,
    Project = 1,
    Action = 2,
    Person = 3,
    Knowledge = 4
}
```

---

### Relationship

Represents a connection between two nodes.

```csharp
public class Relationship
{
    public Guid Id { get; set; }
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }
    public RelationshipType Type { get; set; }
}
```

### RelationshipType (Enum)

```csharp
public enum RelationshipType
{
    BelongsTo = 0,
    Impacts = 1,
    RelatedTo = 2
}
```

---

## 🗄️ Database Schema

```sql
CREATE TABLE Nodes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Type INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);

CREATE TABLE Relationships (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SourceNodeId UNIQUEIDENTIFIER NOT NULL,
    TargetNodeId UNIQUEIDENTIFIER NOT NULL,
    Type INT NOT NULL
);
```

---

## 🏗️ Project Structure

```
/src
  /Domain
    Node.cs
    Relationship.cs
    Enums.cs

  /Application
    Services/

  /Infrastructure
    Repositories/

  /Api
    Controllers/
```

### Structure Rules

* Domain → entities only
* Application → business logic
* Infrastructure → Dapper + database access
* API → endpoints only

---

## 🧰 Data Access (Dapper)

### Requirements

* Use raw SQL
* Use parameterized queries
* Return strongly typed objects

### Example Repository Pattern

```csharp
public class NodeRepository
{
    private readonly IDbConnection _db;

    public NodeRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task InsertAsync(Node node)
    {
        const string sql = @"
            INSERT INTO Nodes (Id, Name, Type, CreatedAt)
            VALUES (@Id, @Name, @Type, @CreatedAt)";

        await _db.ExecuteAsync(sql, node);
    }
}
```

---

## 🔧 Phase 1 — Execution Tasks (MVP)

### Goal

Build a working system with Nodes and Relationships.

---

### Step-by-Step Tasks

#### 1. Setup Project

* Create .NET solution
* Create projects: Domain, Application, Infrastructure, Api
* Configure dependency injection

---

#### 2. Implement Domain

* Create Node class
* Create Relationship class
* Create enums

---

#### 3. Create Database

* Execute schema scripts
* Validate tables exist

---

#### 4. Build Repositories

* NodeRepository
* RelationshipRepository

---

#### 5. Build Application Services

* CreateNode
* CreateRelationship
* GetNodes
* GetRelationships

---

#### 6. API Layer

Create endpoints:

* POST /nodes
* GET /nodes
* POST /relationships
* GET /relationships

---

## ✅ Definition of Done (Phase 1)

* Can create Area, Project, Action nodes
* Can link nodes via relationships
* Data persists correctly
* Data can be queried
* API endpoints functional

---

## ⚠️ Constraints

* Do NOT introduce:

  * ORMs
  * Event sourcing
  * CQRS (unless explicitly instructed)
* Do NOT over-engineer
* Keep implementation linear and clear

---

## 🧭 Detailed Execution Tasks (Phases 2-6)

### 🥈 Phase 2 — Structure & Usability

**1. Hierarchy Navigation**
- **Task 2.1.1**: Create recursive SQL queries (Common Table Expressions) or sequential Dapper queries to fetch node descendants based on `BelongsTo` relationships (e.g., `Area -> Projects -> Actions`).
- **Task 2.1.2**: Update `NodeService` to map flat relational rows into a nested tree structure (DTO).
- **Task 2.1.3**: Add API endpoint `GET /nodes/{id}/hierarchy` to expose the tree.

**2. Tagging / Impact Mapping**
- **Task 2.2.1**: Update `RelationshipService` logic to easily map and index `Impacts` relationships.
- **Task 2.2.2**: Add API endpoint `GET /nodes/{id}/impacts` to retrieve all parent/Area nodes an Action impacts.

**3. Basic Dashboard**
- **Task 2.3.1**: Create a `DashboardService` with an aggregated DTO returning Areas, Active Projects, and Pending Actions.
- **Task 2.3.2**: Write optimized Dapper queries to fetch these aggregates simultaneously.
- **Task 2.3.3**: Add API endpoint `GET /dashboard`.

**4. Filtering & Views**
- **Task 2.4.1**: Expand `Nodes` schema via SQL migration to include `Status` (Enum: Active, Completed, Pending) and `DueDate` (DateTime).
- **Task 2.4.2**: Modify the `GET /nodes` endpoint to accept query parameters (e.g., `?type=`, `?parentId=`, `?status=`).
- **Task 2.4.3**: Add dynamic SQL `WHERE` clauses in `NodeRepository`.
- **Task 2.4.4**: Implement a "Today" view by fetching actions with due dates for today via `GET /nodes/today`.

---

### 🥉 Phase 3 — Intelligence Layer (Momentum Engine v1)

**1. Momentum Scoring**
- **Task 3.1.1**: Add a `MomentumScore` (decimal) column to the `Nodes` table.
- **Task 3.1.2**: Build a `MomentumService` that calculates scores based on recent, connected Action completions (e.g., counting resolved nodes linked via `BelongsTo` or `Impacts`).
- **Task 3.1.3**: Add API endpoint `POST /nodes/{id}/calculate-momentum` to trigger score updates.

**2. Stagnation Detection**
- **Task 3.2.1**: Add a `LastActivityAt` (DateTime) column to the `Nodes` table. Update this timestamp recursively when a child node changes.
- **Task 3.2.2**: Create a `StagnationService` to query Projects/Areas with `LastActivityAt` older than a set threshold (e.g., 7 days).
- **Task 3.2.3**: Add API endpoint `GET /dashboard/stagnant`.

**3. Next Best Action (Basic)**
- **Task 3.3.1**: Implement a sorting algorithm in `MomentumService` that prioritizes pending Actions based on the lowest momentum scores of their parent Areas, combined with high-impact connections.
- **Task 3.3.2**: Add API endpoint `GET /actions/next-best`.

---

### 🏅 Phase 4 — Second Brain Expansion

**1. Knowledge Nodes**
- **Task 4.1.1**: Expand the `Nodes` schema with a `Content` (NVARCHAR/Text) field to store markdown notes.
- **Task 4.1.2**: Create an API endpoint `PATCH /nodes/{id}/content` to update content for `Knowledge` nodes.
- **Task 4.1.3**: Ensure UI/API support for linking Knowledge nodes to Projects via `RelatedTo` relationships.

**2. Contextual Awareness**
- **Task 4.2.1**: Build a `ContextService` that fetches all Knowledge nodes associated with an Action's parent Project.
- **Task 4.2.2**: Include contextual knowledge summaries in the response of `GET /nodes/{id}`.

**3. Daily / Weekly Review System**
- **Task 4.3.1**: Add a new `ReviewLogs` table (`Id`, `NodeId`, `ReviewedAt`).
- **Task 4.3.2**: Create a `ReviewService` to flag Areas that lack recent reviews.
- **Task 4.3.3**: Add API endpoint `GET /reviews/pending`.

---

### 🧠 Phase 5 — Advanced Intelligence

**1. Adaptive Suggestions**
- **Task 5.1.1**: Create an `ActionHistory` table to track completion times, durations, and behaviors.
- **Task 5.1.2**: Integrate historical completion rate data into the "Next Best Action" algorithm.

**2. Relationship Weighting**
- **Task 5.2.1**: Add a `Weight` (Decimal/Float) column to the `Relationships` schema.
- **Task 5.2.2**: Update repositories, services, and DTOs to accept and store relationship weights.
- **Task 5.2.3**: Adjust the Momentum Engine to multiply impact scores by the relationship weight.

**3. Graph Analysis**
- **Task 5.3.1**: Implement advanced graph traversal queries.
- **Task 5.3.2**: Create algorithms to detect "bottlenecks" (e.g., Projects with >5 pending actions and 0 recent completions).
- **Task 5.3.3**: Add API endpoint `GET /insights/bottlenecks`.

---

### 🎨 Phase 7 — Frontend Foundation (Blazor WASM)

**1. Project Setup & Architecture**
- **Task 7.1.1**: Create a new Blazor WebAssembly project (`NexusCortex.Web`) in the solution.
- **Task 7.1.2**: Install `MudBlazor` for the UI component library. Configure `_Imports.razor`, `MainLayout.razor`, and register MudServices in `Program.cs`.
- **Task 7.1.3**: Configure CORS in the `NexusCortex.Api` to allow requests from the Blazor WebAssembly application (typically `localhost:5000` or similar).

**2. API Integration Layer**
- **Task 7.2.1**: Create typed HTTP Client services in Blazor (e.g., `INodeApiClient`, `IDashboardApiClient`) that mirror the backend contracts.
- **Task 7.2.2**: Define shared DTOs or reference the `NexusCortex.Domain` and `NexusCortex.Application` projects directly from the Blazor WASM project to share models (Nodes, Relationships, Hierarchy).

---

### 🖥️ Phase 8 — The "Command Center" Dashboard

**1. Next Best Action Panel**
- **Task 8.1.1**: Create a `NextBestActions.razor` component using `MudCard` and `MudList`.
- **Task 8.1.2**: Fetch data from `GET /nodes/actions/next-best` and display the highest priority pending tasks, emphasizing their impact scores.

**2. Momentum Area Grid**
- **Task 8.2.1**: Fetch Dashboard data (`GET /dashboard`).
- **Task 8.2.2**: Create a responsive `MudGrid` of `MudCard` components for each Area.
- **Task 8.2.3**: Visualize the `MomentumScore` of each Area using a `MudProgressLinear` or similar indicator.

**3. Stagnation Alerts**
- **Task 8.3.1**: Fetch stagnant nodes (`GET /dashboard/stagnant`).
- **Task 8.3.2**: Display prominent `MudAlert` banners at the top of the dashboard for neglected Areas or Projects, prompting user action.

---

### 🌲 Phase 9 — Navigation & Contextual Views

**1. Node Hierarchy Explorer**
- **Task 9.1.1**: Create a global sidebar navigation using `MudTreeView`.
- **Task 9.1.2**: Fetch the full node hierarchy (`GET /nodes/{rootId}/hierarchy`) and dynamically render the tree of Areas -> Projects -> Actions.

**2. Node Detail View ("Second Brain" Context)**
- **Task 9.2.1**: Create a dynamic detail page (`/node/{id}`) using routing.
- **Task 9.2.2**: Use `MudBreadcrumbs` to show the path back to the parent Area.
- **Task 9.2.3**: Use `MudTabs` to organize information:
  - Tab 1: Details (Name, Status, Due Date).
  - Tab 2: Impacts (Display `MudChip` elements for linked Areas/Projects fetched via `GET /nodes/{id}/impacts`).

---

**1. Modular Architecture**
- **Task 6.1.1**: Refactor the solution into distinct domain-driven bounded contexts (e.g., `NexusCortex.Tasks`, `NexusCortex.Knowledge`).
- **Task 6.1.2**: Implement a messaging/event pattern (e.g., MediatR) to decouple service dependencies and handle cross-domain events.

**2. API Layer**
- **Task 6.2.1**: Implement API versioning (e.g., `/api/v1/...`).
- **Task 6.2.2**: Cleanly re-integrate Swagger/OpenAPI documentation with XML summaries.
- **Task 6.2.3**: Add rate-limiting and global exception-handling middleware.

**3. Authentication / Multi-User**
- **Task 6.3.1**: Add `UserId` (Guid) to all database tables.
- **Task 6.3.2**: Implement JWT Authentication middleware.
- **Task 6.3.3**: Update all repository queries to strictly scope data by `UserId`.

**4. Cloud Deployment**
- **Task 6.4.1**: Create `Dockerfile` and `docker-compose.yml` files for local containerization.
- **Task 6.4.2**: Introduce a tool like DbUp to automate database schema migrations instead of relying on manual SQL file execution.
- **Task 6.4.3**: Create CI/CD pipelines (e.g., GitHub Actions) to build, test, and deploy the backend.

---

## 🧠 Guiding Principle

> Build a system that **models relationships first**,
> and tasks become a natural outcome.

---

## 🏁 Execution Directive

Begin with:

1. Project setup
2. Domain implementation
3. Database schema

Proceed step-by-step. Do not skip ahead.
