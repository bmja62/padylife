<script setup lang="ts">
import { isAxiosError } from 'axios'
import type { IApiProvider } from '@/models/IApiProvider'
import type { IUserUpdateOrDeletePayload } from '@/services/UserService'
import { useSpinner } from '@/composables/spinner'
import { useAlerts } from '@/composables/alert'

// Interfaces
interface IProps {
  dialogState: boolean
  defaultUser: IUserUpdateOrDeletePayload
}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  defaultUser: () => {
    return {
      firstName: '',
      lastName: '',
      isActive: false,
      userId: 0,
      fullName: '',
    }
  },
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()

const tempUser = ref<IUserUpdateOrDeletePayload>({
  firstName: '',
  lastName: '',
  isActive: false,
  userId: 0,
  fullName: '',
})

// Watchers
watch(() => props.defaultUser, val => {
  tempUser.value = JSON.parse(JSON.stringify(val))
}, {
  immediate: true,
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteAUnit() {
  if (tempUser.value) {
    try {
      spinner.showSpinner()

      const response = await $api?.users.deleteAUser(tempUser.value.userId)

      if (response?.data.isSuccess) {
        alert.success('کاربر با موفقیت حذف شد!')
        emit('update:dialogState', false)
        emit('refetch')
      }
      else {
        alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
      }
    }
    catch (error: unknown) {
      if (isAxiosError(error))

        alert.error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')

      else
        console.error(error)
    }
    finally {
      spinner.hideSpinner()
    }
  }
}
</script>

<template>
  <CustomDeleteDialog
    v-if="defaultUser && (defaultUser.userId || defaultUser.id)"
    :dialog-state="props.dialogState"
    :title="`حذف کاربر ${defaultUser.firstName} ${defaultUser.lastName}`"
    @update:dialog-state="updateDialogState"
    @delete="deleteAUnit"
  >
    <VCol cols="12">
      <p>
        آیا از حذف کاربر
        <span class="font-weight-bold text-error">

          {{ defaultUser.firstName }} {{ defaultUser.lastName }}
        </span>
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
