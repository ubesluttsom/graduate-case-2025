# Graduate case 2024

This repository is for the graduate case in 2024.

## Quick start

Clone the repository with `git clone git@github.com:Itera/graduate-case-2024.git`.

### Web

Run the following commands from the root of the repository:

Change directory to web

```bash
cd excursion/web 
```

Install dependencies

```bash
npm ci
```

Start the development server

```bash
npm run dev
```

### Api

Run the following commands from the root of the repository:

Change directory to server

```bash
cd exursion/api/Explore.Excursion
```

Run server

```bash
func host start --csharp
```

### CMS

Run the following commands from the root of the repository:

Change directory to server

```bash
cd cms/Explore.Cms
```

Run server

```bash
func host start --csharp
```

## Repository structure

The repository is structured as a monorepo, with a `project` directory containing all the code for the project. The `project` directory contains two subdirectories, `web` and `api`, which contain the code for the web and server projects respectively. The `web` and `api` directories are both structured as standalone projects, with their own `package.json` and `dotnet` files. 

See documentation for the [web](excursion/web/README.md), [api](excursion/api/README.md) and [cms](cms/README.md) projects for detailed information.

## Infrastructure

The infrastructure for the project is defined in the `infrastructure` directory. The infrastructure is defined using Bicep templates. 

## Deployment

The project is deployed to Azure using GitHub Actions. The deployment is configured in the `.github/workflows` directory. A workflow run for building and testing is triggered on every pull request to the `main` branch, and deployment is triggered on every push.

See more information about the workflows and deployment in their [documentation](doc/workflows.md).
