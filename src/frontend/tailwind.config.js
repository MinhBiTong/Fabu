/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,ts,jsx,tsx,mdx}", // Quét tất cả trong src
    "./app/**/*.{js,ts,jsx,tsx,mdx}", // Nếu folder app nằm ngoài src
    "./components/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}