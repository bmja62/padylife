/* eslint-disable import/order */
import '@/@iconify/icons-bundle'
import App from '@/App.vue'
import api from '@/plugins/apiProvider'
import ability from '@/plugins/casl/ability'
import i18n from '@/plugins/i18n'
import layoutsPlugin from '@/plugins/layouts'
import vuetify from '@/plugins/vuetify'
import { loadFonts } from '@/plugins/webfontloader'
import router from '@/router'
import { abilitiesPlugin } from '@casl/vue'
import '@core/scss/template/index.scss'
import '@styles/styles.scss'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import { createApp } from 'vue'
import { LMap, LMarker, LTileLayer } from 'leaflet'
import 'leaflet/dist/leaflet.css'
// eslint-disable-next-line import/extensions
import 'froala-editor/js/plugins.pkgd.min.js'

import 'froala-editor/js/third_party/embedly.min'
import 'froala-editor/js/third_party/font_awesome.min'
import 'froala-editor/js/third_party/spell_checker.min'
import 'froala-editor/js/third_party/image_tui.min'

import 'froala-editor/css/froala_editor.pkgd.min.css'
import VueFroala from 'vue-froala-wysiwyg'

import 'froala-editor/css/froala_style.min.css'

loadFonts()

const pinia = createPinia()

pinia.use(piniaPluginPersistedstate)

// Create vue app
const app = createApp(App)

app.provide('$api', api)

// Use plugins
app.component('LMap', LMap)
app.component('LTileLayer', LTileLayer)
app.component('LMarker', LMarker)
app.use(vuetify)
app.use(pinia)
app.use(router)
app.use(layoutsPlugin)
app.use(i18n)
app.use(VueFroala)
app.use(abilitiesPlugin, ability, {
  useGlobalProperties: true,
})

// Mount vue app
app.mount('#app')
