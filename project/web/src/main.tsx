import { ChakraProvider } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import theme from './theme';
import { MsalProvider } from '@azure/msal-react';
import  authInstance  from './auth/AuthInstance.tsx';


ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <ChakraProvider theme={ theme }>
      <MsalProvider instance={ authInstance }>
        <App />
      </MsalProvider>
    </ChakraProvider>
  </React.StrictMode>,
)
