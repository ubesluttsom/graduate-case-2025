import { Configuration,  PublicClientApplication } from "@azure/msal-browser";

// MSAL configuration
const configuration: Configuration = {
    auth: {
        clientId: import.meta.env.VITE_AUTH_CLIENT_ID,
    }
};

const pca = new PublicClientApplication(configuration);
export default pca;