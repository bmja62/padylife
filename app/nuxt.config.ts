// https://nuxt.com/docs/api/configuration/nuxt-config

export default defineNuxtConfig({
  compatibilityDate: "2024-11-01",

  future: {
    compatibilityVersion: 4,
  },
  vite: {
    optimizeDeps: {
      exclude: ["vee-validate", "Propeller"],
    },
  },
  app: {
    baseURL: "/",
    head: {
      titleTemplate: "پادی لایف | %s",
      charset: "utf-8",
      viewport:
        "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no",
      htmlAttrs: {
        class: "transition-all duration-200",
        lang: "fa",
        dir: "rtl",
      },
      link: [
        { rel: 'manifest', href: '/manifest.webmanifest' },
      ],
    },
  },

  ssr: false,
  nitro: {
    preset: 'static',
    publicAssets: [{ dir: 'public' }]

  },
  routeRules: {
    '/api/_nuxt_icon/**': { prerender: true },
  },
  css: ["~/assets/css/tailwind.css"],
  devtools: { enabled: true },

  image: {
    provider: process.env.ENABLE_IPX === 'true' ? 'ipx' : 'none',

    format: ["avif", "webp", "jpg", "jpeg", "png"],
    screens: {
      xs: 320,
      sm: 640,
      md: 768,
      lg: 1024,
      xl: 1280,
      xxl: 1536,
    },

    ...(process.env.ENABLE_IPX === 'true' ? {
      ipx: {
        maxAge: 60 * 60 * 24 * 7,
      }
    } : {})
  },

  veeValidate: {
    // Set autoImports to false due to nuxt bug in version 3.13.2
    // With modules component imports, remove after fix and change
    // According to component names
    // etc: Form as VeeForm...
    autoImports: false,
  },
  imports: {
    presets: [
      {
        from: "yup",
        imports: [
          {
            name: "object",
            as: "yupObject",
          },
          {
            name: "string",
            as: "yupString",
          },
          {
            name: "number",
            as: "yupNumber",
          },
          {
            name: "date",
            as: "yupDate",
          },
          {
            name: "InferType",
            as: "yupInferType",
          },
        ],
      },
    ],
  },

  fonts: {
    provider: "local",
    defaults: {
      weights: [400, 500, 600, 700, 800],
    },
    families: [
      {
        name: "Peyda",
      },
    ],
  },

  icon: {
    provider: 'iconify',
    clientBundle: {
      scan: true,
    },
    customCollections: [
      {
        prefix: "icon",
        dir: "app/assets/icons",
      },
      {
        prefix: "feelings",
        dir: "app/assets/icons/feelings",
      },
    ],
  },

  postcss: {
    plugins: {
      tailwindcss: {},
      autoprefixer: {},
    },
  },

  modules: [
    "@nuxt/image",
    "@nuxt/fonts",
    "@nuxt/eslint",
    "@nuxt/test-utils/module",
    "@nuxt/icon",
    "@vee-validate/nuxt",
    '@vite-pwa/nuxt',
    "nuxt-swiper",
    "@pinia/nuxt",
    "@vueuse/nuxt",
    "pinia-plugin-persistedstate/nuxt",
  ],
  pwa: {
    registerType: "autoUpdate",
    manifest: {
      // this restricts user navigation to some directories of website
      // scope: "https://example.com/subdirectory/",
      name: "Pady Life",
      short_name: "PLife",
      theme_color: "#01CED1",
      description: "",
      background_color: "#F1F1F1",
      dir: "rtl",
      lang: "fa",
      display: "standalone",
      start_url: "/",
      icons: [
        {
          src: "/pwa/web-app-manifest-192x192.png",
          sizes: "192x192",
          type: "image/png",
        },
        {
          src: "/pwa/web-app-manifest-512x512.png",
          sizes: "512x512",
          type: "image/png",
        },
        {
          src: "/pwa/favicon-96x96.png",
          sizes: "16x16",
          type: "image/png",
        },
        {
          src: "/pwa/apple-touch-icon.png",
          sizes: "180x180",
          type: "image/png",
        },

        {
          src: "/pwa/faviconv.ico",
          sizes: "16x16",
          type: "image/ico",
        },
      ],
    },
    workbox: {
      cleanupOutdatedCaches: true,
      clientsClaim: true,
      skipWaiting: true,
      runtimeCaching: [
        // this is used for caching third-party resources.
        // {
        //     urlPattern: /^https:\/\/fonts\.googleapis\.com\/.*/i,
        //     handler: 'CacheFirst',
        //     options: {
        //         cacheName: 'google-fonts-cache',
        //         expiration: {
        //             maxEntries: 10,
        //             maxAgeSeconds: 60 * 60 * 24 * 365 // <== 365 days
        //         },
        //         cacheableResponse: {
        //             statuses: [0, 200]
        //         }
        //     }
        // },
      ]
      // this is for caching internal assets, by default it only supports js css html.
      // configure it to include or exclude some type of assets
      // globPatterns: ["**/*.{ico,png,svg,gif,webp}"],
      // runtimeCaching: [
      //     {
      //         urlPattern: /^https:\/\/simagar\.com\/.*/,
      //         handler: 'NetworkFirst',
      //         method: 'GET',
      //         strategyOptions: {
      //             cacheName: 'site-cache',
      //             networkTimeoutSeconds: 10,
      //             cacheExpiration: {
      //                 maxEntries: 100,
      //                 maxAgeSeconds: 24 * 60 * 60 // 24 hours
      //             }
      //         }
      //     },
      //     {
      //         urlPattern: /^https:\/\/.*\.(?:png|jpg|jpeg|svg|gif|webp)$/,
      //         handler: 'CacheFirst',
      //         method: 'GET',
      //         strategyOptions: {
      //             cacheName: 'image-cache',
      //             cacheExpiration: {
      //                 maxEntries: 60,
      //                 maxAgeSeconds: 7 * 24 * 60 * 60 // 7 days
      //             }
      //         }
      //     }
      //     // this is used for caching third-party resources.
      //     // {
      //     //     urlPattern: /^https:\/\/fonts\.googleapis\.com\/.*/i,
      //     //     handler: 'CacheFirst',
      //     //     options: {
      //     //         cacheName: 'google-fonts-cache',
      //     //         expiration: {
      //     //             maxEntries: 10,
      //     //             maxAgeSeconds: 60 * 60 * 24 * 365 // <== 365 days
      //     //         },
      //     //         cacheableResponse: {
      //     //             statuses: [0, 200]
      //     //         }
      //     //     }
      //     // },
      // ],
    },
  },

  auth: {
    globalAppMiddleware: false,
    baseURL: `${process.env.NUXT_PUBLIC_API_ADDRESS}/api/v1`,
    provider: {
      type: "local",
      endpoints: {
        signUp: { path: "/Users/Register", method: "post" },
        signIn: { path: "/Users/Token/Token", method: "post" },
        getSession: { path: "/Users/GetByToken", method: "get" },
        signOut: false,
      },
      pages: {
        login: "/",
      },
      token: {
        signInResponseTokenPointer: "/data/accessToken/access_token",
        type: "Bearer",
        headerName: "Authorization",
        maxAgeInSeconds: 86400,
      },
      session: {
        dataType: {
          email: "string",
          emailConfirmed: "boolean",
          fullName: ' null | string',
          gender: ' null | number',
          id: 'number',
          introduceCode: ' null | string',
          isActive: 'boolean',
          phoneNumber: 'string',
          roles: ' {name: string, description: string}[]',
          shabaNo: 'null | string',
          userName: 'string',
          walletCredit: 'number',
        },
        dataResponsePointer: "/data",
      },
    },
    sessionRefresh: {
      enableOnWindowFocus: false,
    },
  },
  devServer: {
    host: ' 192.168.1.116',
    port: 3036
  },
  runtimeConfig: {
    public: {
      apiAddress: process.env.NUXT_PUBLIC_API_ADDRESS,
      seqAddress: process.env.NUXT_PUBLIC_SEQ_ADDR,
      seqApiKey: process.env.NUXT_PUBLIC_SEQ_API_KEY,
      googleClientId: process.env.NUXT_PUBLIC_GOOGLE_CLIENT_ID
    },
  },
});
