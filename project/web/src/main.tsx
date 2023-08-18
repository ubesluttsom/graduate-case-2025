import { MsalProvider } from '@azure/msal-react';
import { ChakraProvider } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App.tsx';
import authInstance from './auth/AuthInstance.tsx';
import ProtectedRoutes from './auth/ProtectedRoute.tsx';
import theme from './theme';


ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <ChakraProvider theme={ theme }>
      <MsalProvider instance={authInstance}>
        <ProtectedRoutes>
          <BrowserRouter>
            <App />
          </BrowserRouter>
        </ProtectedRoutes>
      </MsalProvider>
    </ChakraProvider>
  </React.StrictMode>,
)
