# web

This project was created with [Vite](https://vitejs.dev/)

1. [Available scripts](#available-scripts)
2. [Deployment](#deployment)
3. [Storybook](#storybook)
4. [Style guide](#style-guide)
5. [Testing](#testing)

## Available Scripts

In the web directory, you can run:

### `npm run dev`

Runs the app in the development mode.
Open [http://localhost:5174](http://localhost:5174) to view it in the browser.

The page will reload if you make edits.
You will also see any lint errors in the console.

### `npm run storybook`

Runs storybook in the development mode on [http://localhost:6006](http://localhost:6006).

### `npm run build`

Builds the app for production to the `dist` folder.

See the section about [deployment](https://vitejs.dev/guide/build.html) and the [Build and deploy workflow](../../doc/workflows.md) for more information.

### `npm run lint`

Lints the code using [ESLint](https://eslint.org/).

### `npm run preview`

Runs the app in the production mode. Needs `npm run build` to be run first.

### `npm run build-storybook`

Builds storybook for production to the `storybook-static` folder.

## Deployment

The app is deployed to Azure using the github action [Deploy web app](../../.github/actions/deploy-web-app/action.yaml). The action is configured in the [build-and-deploy-webapp workflow](../../.github/workflows/build-and-deploy-webapp.yml). See [workflows](../../doc/workflows.md) for more information.

## Storybook

[Storybook](./.storybook) is used to develop and test components in isolation. See [Storybook](https://storybook.js.org/) for more information. Stories are defined in [./src/stories](./src/stories).

## Style guide

This project uses [ESLint](https://eslint.org/) and [Prettier](https://prettier.io/) to enforce a consistent code style. See [ESLint](https://eslint.org/) and [Prettier](https://prettier.io/) for more information. Prettier is configured in [.prettierrc](../../.prettierrc) and ESLint is configured in [.eslintrc.cjs](./.eslintrc.cjs).

The web project uses Chackra UI for styling. See [Chakra UI](https://chakra-ui.com/) for more information.

## Testing

See the [Test project](../../test/) for testing.

## Authentication

We make use of the MSAL library to handle authentication. See [MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md) for more information.
One thing to get aquainted with is the [MSAL Hooks](https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/hooks.md).

### Authentication configuration

The authentication configuration is defined in [./src/auth/AuthInstance.tsx](./src/auth/AuthInstance.tsx). The configuration is used in [./src/main.tsx](./src/main.tsx) to initialize the MSAL instance.

To be able to use the authentication configuration, you need to create a file called `.env.local` in the env folder. The file should contain the following:

```env
VITE_AUTH_CLIENT_ID=<client-id>
```

The client-id is the id of the Azure AD app registration. See [Azure AD app registration](https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/40d44444-0d3e-4876-a8fd-16fa8014f2bf) for more information.

### Authentication hooks

We created a custom hook to get the access token. See [./src/auth/useAccessToken.tsx](./src/auth/useAccessToken.tsx) for more information.
