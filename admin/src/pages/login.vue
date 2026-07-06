<script setup lang="ts">
import {isAxiosError} from 'axios'

import {VForm} from 'vuetify/components/VForm'
import {inject} from 'vue'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ILoginPayload, IRole} from '@/services/UserService'
import {useGenerateImageVariant} from '@core/composable/useGenerateImageVariant'
import authV2LoginIllustrationBorderedDark from '@images/pages/auth-v2-login-illustration-bordered-dark.png'
import authV2LoginIllustrationBorderedLight from '@images/pages/auth-v2-login-illustration-bordered-light.png'
import authV2LoginIllustrationDark from '@images/pages/auth-v2-login-illustration-dark.png'
import authV2LoginIllustrationLight from '@images/pages/auth-v2-login-illustration-light.png'
import authV2MaskDark from '@images/pages/misc-mask-dark.png'
import authV2MaskLight from '@images/pages/misc-mask-light.png'
import {VNodeRenderer} from '@layouts/components/VNodeRenderer'
import {themeConfig} from '@themeConfig'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {useAlertStore} from '@/stores/alerts'
import {userAbilities} from '@/plugins/casl/UserAbilities'
import {useAppAbility} from '@/plugins/casl/useAppAbility'
import {useAuthStore} from '@/stores/auth'
import type {UserAbility} from '@/plugins/casl/AppAbility'

// Variables
const alertStore = useAlertStore()
const spinner = useSpinner()
const alert = useAlerts()
const {isRenderingErrorAlert, isRenderingSuccessAlert, alertMessage} = storeToRefs(alertStore)
const auth = useAuthStore()

const $api = inject<IApiProvider>('$api')

const authThemeImg = useGenerateImageVariant(authV2LoginIllustrationLight, authV2LoginIllustrationDark, authV2LoginIllustrationBorderedLight, authV2LoginIllustrationBorderedDark, true)

const authThemeMask = useGenerateImageVariant(authV2MaskLight, authV2MaskDark)

const isPasswordVisible = ref(false)

const refVForm = ref<VForm>()

const router = useRouter()

const route = useRoute()

const ability = useAppAbility()

const loginPayload = ref<ILoginPayload>({
  username: "",
  password: ""
})

function closeAlert() {
  alert.close()
}

async function login() {
  try {
    spinner.showSpinner()

    const response = await $api?.users.login(loginPayload.value)
    if (response.data.isSuccess) {
      auth.setUser(response.data.data.user)
      auth.setToken(response.data.data.accessToken)
      const roleAbilities: UserAbility[] = []
      if (response.data.data.user.roles.length) {
        if (response.data.data.user.roles[0].name === 'User')
          return alert.error('شما اجازه دسترسی به این پنل را ندارید')
        response.data.data.user.roles.forEach((role: IRole) => {
          roleAbilities.push(...userAbilities[role.name])
        })
        localStorage.setItem('userAbilities', JSON.stringify(roleAbilities))
        ability.update(roleAbilities)
        // Redirect to requested route, otherwise land on dashboard
        router.replace(route.query.to ? String(route.query.to) : { name: 'dashboard-detail' })
      }

    } else {
      alert.error(response.data.errorMessage)
    }

  } catch (error: unknown) {
    if (isAxiosError(error))
      alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

    else
      console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}
</script>

<template>
  <VRow
    no-gutters
    class="auth-wrapper bg-surface"
  >
    <VAlert
      v-if="isRenderingErrorAlert || isRenderingSuccessAlert"
      class="custom-alert"
      :title="isRenderingErrorAlert ? 'مشکلی پیش آمد' : 'عملیات موفق'"
      :type="isRenderingSuccessAlert ? 'success' : 'error'"
      @click.stop="closeAlert"
    >
      {{ alertMessage }}
    </VAlert>
    <VCol
      lg="8"
      class="d-none d-lg-flex"
    >
      <div class="position-relative bg-background rounded-lg w-100 ma-8 me-0">
        <div class="d-flex align-center justify-center w-100 h-100">
          <VImg
            max-width="505"
            :src="authThemeImg"
            class="auth-illustration mt-16 mb-2"
          />
        </div>

        <VImg
          :src="authThemeMask"
          class="auth-footer-mask"
        />
      </div>
    </VCol>

    <VCol
      cols="12"
      lg="4"
      class="auth-card-v2 d-flex align-center justify-center"
    >
      <VCard
        flat
        :max-width="500"
        class="mt-12 mt-sm-0 pa-4"
      >
        <VCardText>
          <VNodeRenderer
            :nodes="themeConfig.app.logo"
            class="mb-6"
          />

          <h5 class="text-h5 mb-1">
            به پنل <span class="text-capitalize"> {{ themeConfig.app.title }} </span> خوش آمدید!
          </h5>

          <p class="mb-0">
            لطفا با اکانت خود وارد شوید.
          </p>
        </VCardText>

        <VCardText>
          <VForm
            ref="refVForm"
            @submit.prevent="login"
          >
            <VRow>
              <!-- mobile -->
              <VCol cols="12">
                <AppTextField
                  v-model.trim="loginPayload.username"
                  placeholder="نام کاربری"
                  label="نام کاربری"
                  type="text"
                  autofocus
                />
              </VCol>

              <!-- password -->
              <VCol cols="12">
                <AppTextField
                  v-model="loginPayload.password"
                  label="رمز عبور"
                  :type="isPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isPasswordVisible ? 'tabler-eye-off' : 'tabler-eye'"
                  @click:append-inner="isPasswordVisible = !isPasswordVisible"
                />
                <VBtn
                  block
                  type="submit"
                  class="mt-6"
                >
                  ورود
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </VCol>
  </VRow>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth.scss";
</style>

<route lang="yaml">
meta:
 layout: blank
 auth: false
</route>
