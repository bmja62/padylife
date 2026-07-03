/** @type {import('tailwindcss').Config} */
import daisyui from "daisyui";
export default {
  content: [
    "./app/components/**/*.{js,vue,ts}",
    "./app/layouts/**/*.vue",
    "./app/pages/**/*.vue",
    "./app/plugins/**/*.{js,ts}",
    "./app/app.vue",
    "./app/error.vue",
  ],
  theme: {
    extend: {
      colors: {
        primary: "#01CED1",
        "primary-content": "#565E6D",
        secondary: "#F1F1F1",
        neutral: "#706D6D",
        accent: "#E1E1E1",
      },
      fontFamily: {
        Peyda: ["Peyda", "sans-serif"],
      },
    },
  },
  plugins: [require("@tailwindcss/typography"), daisyui],
  daisyui: {
    themes: [
      {
        light: {
          ...require("daisyui/src/theming/themes")["light"],
          // primary: "#565E6D",
          primary: "#01CED1",
          "primary-content": "#565E6D",
          secondary: "#F1F1F1",
          neutral: "#706D6D",
          accent: "#E1E1E1",
          ".btn": {
            "border-radius": "16px",
            color: "#fff",
          },
          ".btn-secondary": {
            color: "#000",
          },
        },
        // dark: {
        //   ...require("daisyui/src/theming/themes")["dark"],
        //   primary: "#565E6D",
        //   secondary: "#F1F1F1",
        //   neutral: "#706D6D",
        //   accent: "#E1E1E1",
        //   ".btn": {
        //     "border-radius": "16px",
        //   },
        //   ".btn-secondary": {
        //     color: "#000",
        //   },
        // },
      },
    ],
  },
};
