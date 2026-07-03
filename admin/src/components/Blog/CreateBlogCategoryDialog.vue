<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateBlogCategoryPayload} from "@/services/BlogService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {VForm} from "vuetify/components/VForm";

// Interfaces
interface Props {
  dialogState: boolean
}

interface Emit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}

// Variables
const props = defineProps<Props>()
const emit = defineEmits<Emit>()
const spinner = useSpinner()
const alert = useAlerts()
const imageUrl = ref(null)
const refVForm = ref(null)
const $api = inject<IApiProvider>('$api')

const blogCategoryPayload = ref<ICreateBlogCategoryPayload>({
  title: '',
  description: '',
  imageUrl: '',
})

const updateDialogState = (val: boolean) => {
  emit('update:dialogState', val)
}

// Functions

function setFile(medias) {
  if (medias.length) {
    blogCategoryPayload.value.imageUrl = medias[0]
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createBlogCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
async function createBlogCategory() {
  try {
    spinner.showSpinner()
    imageUrl.value.getFiles()
    const response = await $api?.blog.createBlogCategories(blogCategoryPayload.value)
    if (response.data.isSuccess) {
    blogCategoryPayload.value = {
      title: '',
      imageUrl: '',
      description: ''
    }
    alert.success('دسته بندی با موفقیت ایجاد شد!')
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
</script>

<template>
  <CustomCreateDialog
    :dialog-state="props.dialogState"
    title="ایجاد دسته بندی جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
    <VCol cols="12">
      <VTextField
        v-model.trim="blogCategoryPayload.title"
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
            v-model.trim="blogCategoryPayload.description"
            variant="outlined"
            density="compact"
            label="توضیحات دسته بندی"
            color="success"
          />
        </VCol>
        <VCol cols="12" md="12">
          <Uploader ref="imageUrl"
                    @getFiles="setFile" label="فایل مربوطه"
                    :file-type="UploaderTypes.BlogCategories"></Uploader>
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
