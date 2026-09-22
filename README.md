# Fakestore project for "Interoperability of information systems" course

Fakestore project for the "Interoperability of information systems" that implements MVC, Rest API, GraphQL API, SOAP API and PostgreSQL database

- Project requirements [here](docs/project_requirements.md)
- *Course assigned Public REST API*: https://app.beeceptor.com/mock-server/fake-store-api

## Installation

Prerequisites: Docker + Docker Compose, and the .NET 8 SDK.

Run these in order, from the repo root:

1. **Start the database:**

   ```sh
   docker compose up --build --abort-on-container-failure -d
   ```

2. **Seed it**

   ```sh
   ./scripts/run-dataseeder.sh
   ```

3. **Pick a mode to run:**

    - **Public** — WebApp, gRPC and SOAP against the public course API.
    1. Run this command to build the project and run it

    ```sh
    docker compose -f docker-compose.public.yml up --build --abort-on-container-failure
    ```

    - **Custom** — WebApp, gRPC, SOAP MyRestApi and MyGraphQLApi, backed by the database from
    1. Run this command to build the project and run it

     ```sh
     docker compose -f docker-compose.custom.yml up --build --abort-on-container-failure
     ```

Endpoints: 
- WebApp http://localhost:5197
- RestApi (Swagger) http://localhost:5250/swagger
- MyGraphQLApi http://localhost:5046/graphql
- SOAP WSDL http://localhost:5123/ProductService.asmx?WSDL

## Documentation

- [Projects](docs/projects.md) — overview of each project in this repo