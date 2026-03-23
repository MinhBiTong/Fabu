import { createTheme } from '@mui/material/styles';

  export const ThemeColor = createTheme({
    palette: {
      primary: {
        main: '#ca1f66',
      },
      secondary: {
        main: '#000000ff',
      },
      error: {
        main: '#ff0000',
      },
      warning: {
        main: '#ffeb3b',
      },
      info: {
        main: '#2196f3',
      },
      success: {
        main: '#4caf50',
      },
      background: {
        default: '#f5f5f5',
      },
    },
  });