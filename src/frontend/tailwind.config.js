/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,ts,jsx,tsx,mdx}",
    "./app/**/*.{js,ts,jsx,tsx,mdx}",
    "./components/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    container: {
      center: true,
      padding: {
        DEFAULT: "16px",
        md: "20px",
        lg: "32px",
      },
      screens: {
        "2xl": "1400px",
      },
    },
    extend: {
      colors: {
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        card: "hsl(var(--card))",
        "card-foreground": "hsl(var(--card-foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
        accent: {
          DEFAULT: "hsl(var(--accent))",
          foreground: "hsl(var(--accent-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        border: "hsl(var(--border))",
        input: "hsl(var(--input))",
        ring: "hsl(var(--ring))",
        fabu: {
          red: "#EE0033",
          "red-hover": "#D11313",
          burgundy: "#C41E3A",
          rose: "#F899AD",
          blue: "#1890FF",
          link: "#0066CC",
          orange: "#FE9A00",
          charcoal: "#333333",
          ink: "#2C2F31",
          gray: "#666666",
          border: "#D1D1D1",
          muted: "#F4F4F4",
          surface: "#FFFFFF",
          info: "#E6F7FF",
        },
      },
      borderRadius: {
        fabu: "8px 8px 8px 0px",
        card: "8px",
      },
      boxShadow: {
        subtle: "0px 2px 4px rgba(0, 0, 0, 0.06)",
        elevated: "0px 2px 8px rgba(0, 0, 0, 0.08)",
        prominent: "0px 4px 12px rgba(0, 0, 0, 0.1)",
        modal: "0px 8px 24px rgba(0, 0, 0, 0.15)",
      },
      fontFamily: {
        display: [
          "FS PFBeauSansPro",
          "-apple-system",
          "BlinkMacSystemFont",
          "Segoe UI",
          "sans-serif",
        ],
        body: [
          "Roboto",
          "-apple-system",
          "BlinkMacSystemFont",
          "Segoe UI",
          "sans-serif",
        ],
      },
    },
  },
  plugins: [],
};
