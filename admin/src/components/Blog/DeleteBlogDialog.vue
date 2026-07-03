<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import type {ICreateOrUpdateBrandPayload} from '@/services/BrandService'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'

interface ITempBrand extends ICreateOrUpdateBrandPayload {
  id: string | number
}

interface Props {
  dialogState: boolean
  defaultBlog: ITempBrand
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultBlog: () => {
    return {}
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()


// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteBlog() {
  if (props.defaultBlog) {
    try {
      spinner.showSpinner()
      const response = await $api?.blog.deleteBlog(props.defaultBlog.id)
      if (response.data.isSuccess) {
        alert.success('خبر با موفقیت حذف شد!')
        emit('update:dialogState', false)
        emit('refetch')
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
}
</script>

<template>
  <CustomDeleteDialog
    v-if="defaultBlog && defaultBlog.title"
    :dialog-state="props.dialogState"
    :title="`حذف خبر ${defaultBlog.title}`"
    @update:dialog-state="updateDialogState"
    @delete="deleteBlog"
  >
    <VCol cols="12">
      <p>
        آیا از حذف خبر
        <span class="font-weight-bold text-error">
          {{ defaultBlog.title }}
        </span>
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
