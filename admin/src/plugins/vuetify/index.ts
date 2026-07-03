import {createVuetify} from 'vuetify'
import {VBtn} from 'vuetify/components/VBtn'
import defaults from './defaults'
import {icons} from './icons'
import theme from './theme'
// Styles
import { VTreeview } from 'vuetify/labs/VTreeview'
import '@core/scss/template/libs/vuetify/index.scss'
import 'vuetify/styles'

export default createVuetify({
  components: {
    VTreeview,
  },
  aliases: {
    IconBtn: VBtn,
  },
  defaults,
  icons,
  theme,
})
