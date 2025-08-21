
import { ChakraProvider } from '@chakra-ui/react';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App.tsx';
import theme from './theme.ts';


ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <ChakraProvider theme={ theme }>
          <BrowserRouter>
            <App />
          </BrowserRouter>
    </ChakraProvider>
  </React.StrictMode>,
)
