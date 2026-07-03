<script lang="ts" setup>
import {isAxiosError} from 'axios'
import type {IUserCreatePayload} from '@/services/UserService'
import type {IRole} from '@/services/RoleService'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

// Interfaces
interface Props {
  dialogState: boolean
  staticRole?: string
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  staticRole: 1,
  dialogState: false,
})

const emit = defineEmits<Emit>()
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')
const tempRole = ref<IRole | null>(null)

const userPayload = ref<IUserCreatePayload>({
  userFullName: "",
  nationalCode: "",
  phoneNumber: "",
  typeId: null,
  password: ""
})

// Functions
const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

function setRoles(role: IRole): void {
  tempRole.value = role
}

function clearRoles(): void {
  tempRole.value = null
}

async function createANewUser() {
  try {
    spinner.showSpinner()

    const response = await $api?.users.createANewUser(userPayload.value)
    if (response?.data.isSuccess) {
      tempRole.value = null
      userPayload.value = {
        userFullName: "",
        nationalCode: "",
        phoneNumber: "",
        typeId: null,
        password: ""
      }
      alert.success('کاربر با موفقیت ایجاد شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      alert.error(response?.data.errorMessage || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
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
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد کاربر جدید"
    @create="createANewUser"
    @update:dialog-state="updateDialogState"
  >
    <VCol cols="12">
      <VTextField
        v-model.trim="userPayload.phoneNumber"
        color="success"
        density="compact"
        hide-details
        label="شماره همراه کاربر"
        required
        variant="outlined"
      />
    </VCol>
    <VCol cols="12">
      <VTextField
        v-model.trim="userPayload.userFullName"
        color="success"
        density="compact"
        hide-details
        label="نام و نام خانوادگی"
        required
        variant="outlined"
      />
    </VCol>
    <VCol cols="12">
      <VTextField
        v-model.trim="userPayload.nationalCode"
        color="success"
        density="compact"
        hide-details
        label="کد ملی"
        required
        variant="outlined"
      />
    </VCol>
    <VCol cols="12">
      <VTextField
        v-model.trim="userPayload.password"
        color="success"
        density="compact"
        hide-details
        label="رمز عبور کاربر"
        required
        variant="outlined"
      />
    </VCol>
    <VCol
      cols="12"
    >
      <RolePicker
        v-model="userPayload.typeId"
      />
    </VCol>
  </CustomCreateDialog>
</template>
