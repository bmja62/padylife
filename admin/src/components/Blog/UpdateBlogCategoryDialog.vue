<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {VForm} from "vuetify/components/VForm";
import {IBlogCategory} from "@/services/BlogService";

// Interfaces
interface Props {
  dialogState: boolean
  defaultCategory: IBlogCategory
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Variables
const props = withDefaults(defineProps<Props>(), {
  defaultCategory: () => {
    return {}
  },
})

const emit = defineEmits<Emit>()
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const imageUrl = ref(null)
const refVForm = ref(null)


// Functions

function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateBlogCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

function setFile(medias) {
  if (medias.length) {
    props.defaultCategory.imageUrl = medias[0]
  }
}

async function updateBlogCategory() {
  if (props.defaultCategory) {
    try {
      spinner.showSpinner()
      imageUrl.value.getFiles()
      const response = await $api?.blog.updateBlogCategories(props.defaultCategory)
      if (response.data.isSuccess) {
      alert.success('دسته بندی با موفقیت ویرایش شد!')
      emit('update:dialogState', false)
      emit('refetch')
      } else {
        alert.error(response.data.message)
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
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    :title="`ویرایش دسته بندی`"
    @update:dialog-state="updateDialogState"
    @update="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.defaultCategory.title"
            variant="outlined"
            density="compact"
            label="نام دسته بندی"
            color="success"
            :rules="[(value) => !!value || 'فیلد اجباری است']"
            required
          />
        </VCol>
        <VCol cols="12">
          <VTextarea
            v-model.trim="props.defaultCategory.description"
            variant="outlined"
            density="compact"
            label="توضیحات دسته بندی"
            color="success"
          />
        </VCol>
        <VCol cols="12" md="12">
          <Uploader ref="imageUrl"
                    :default-medias="props.defaultCategory.imageUrl"
                    @getFiles="setFile" label="فایل مربوطه"
                    :file-type="UploaderTypes.BlogCategories"></Uploader>
        </VCol>
      </VRow>
    </VForm>

  </CustomUpdateDialog>
</template>
