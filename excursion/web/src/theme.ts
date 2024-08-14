import { extendTheme, withDefaultColorScheme } from "@chakra-ui/react";

const theme = {
  colors: {
    'explore-blue': {
      main: '#0a2242',
      50: '#e5f1ff',
      100: '#bdd4f5',
      200: '#92b8eb',
      300: '#689be3',
      400: '#417fdc',
      500: '#2a65c2',
      600: '#204f98',
      700: '#16386d',
      800: '#0a2242', // main
      900: '#000b1a'
    },
    'explore-yellow': {
      main: '#ffbe80',
      50: '#fff1dd',
      100: '#ffd8b0',
      200: '#ffbe80', // main
      300: '#fea34e',
      400: '#fe8a1f',
      500: '#e57107',
      600: '#b25702',
      700: '#803e00',
      800: '#4e2400',
      900: '#1f0a00'
    },
    'explore-light-blue': {
      main: '#a5bed9',
      50: '#e7f3ff',
      100: '#c7d8eb',
      200: '#a5bed9', // main
      300: '#82a3c9',
      400: '#6089b8',
      500: '#4770a0',
      600: '#36577d',
      700: '#263e5a',
      800: '#152538',
      900: '#030c18'
    },
    'explore-red': {
      main: '#9f1f14',
      50: '#ffe7e4',
      100: '#fabfb9',
      200: '#f2958d',
      300: '#eb6c60',
      400: '#e54334',
      500: '#cb291a',
      600: '#9f1f14', // main
      700: '#72150d',
      800: '#460a05',
      900: '#1e0000'
    },
    'explore-gray': {
      main: '#f2f2f2', 
      50: '#f2f2f2', // main
      100: '#d9d9d9',
      200: '#bfbfbf',
      300: '#a6a6a6',
      400: '#8c8c8c',
      500: '#737373',
      600: '#595959',
      700: '#404040',
      800: '#262626',
      900: '#0d0d0d'
    }
  }
};

export default extendTheme(theme, withDefaultColorScheme({ colorScheme: 'explore' }));
