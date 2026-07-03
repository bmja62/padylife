<script setup lang="ts">
import { isAxiosError } from 'axios'
import type { IApiProvider } from '@/models/IApiProvider'
import type { IDynamicPage } from '@/services/DynamicPageService'
import { useSpinner } from '@/composables/spinner'
import { useAlerts } from '@/composables/alert'

// Interfaces
interface IProps {
  dialogState: boolean
  defaultDynamicPage: IDynamicPage | null
}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  defaultDynamicPage: () => {
    return {
      content: '',
      id: 0,
      seoDescription: '',
      seoTitle: '',
      slug: '',
      title: '',
    }
  },
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()

const tempDynamicPage = ref<IDynamicPage>({
  content: '',
  id: 0,
  seoDescription: '',
  seoTitle: '',
  slug: '',
  title: '',
})

// Watchers
watch(() => props.defaultDynamicPage, val => {
  tempDynamicPage.value = JSON.parse(JSON.stringify(val))
}, {
  immediate: true,
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function deleteADynamicPage() {
  if (tempDynamicPage.value) {
    try {
      spinner.showSpinner()

      const response = await $api?.dynamicPages.deleteADynamicPage(tempDynamicPage.value.id)

      if (response?.data.isSuccess) {
        alert.success('برگه با موفقیت حذف شد!')
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
    v-if="props.defaultDynamicPage && props.defaultDynamicPage.id"
    :dialog-state="props.dialogState"
    :title="`حذف برگه ${props.defaultDynamicPage.title}`"
    @update:dialog-state="updateDialogState"
    @delete="deleteADynamicPage"
  >
    <VCol cols="12">
      <p>
        آیا از حذف برگه‌ی
        <span class="font-weight-bold text-error">
          {{ props.defaultDynamicPage.title }}
        </span>
        اطمینان دارید؟
      </p>
    </VCol>
  </CustomDeleteDialog>
</template>
