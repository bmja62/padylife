`
<script setup lang="ts">
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {ICreateProductCategoryPayload} from "@/services/ProductCategories";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

interface IProps {
  dialogState: boolean
}

const props = defineProps<IProps>()

const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()
const spinner = useSpinner()
const alert = useAlerts()
const $api = inject<IApiProvider>('$api')

const refVForm = ref(null)
const productCategoryPayload = ref<ICreateProductCategoryPayload>({
  name: '',
  description: '',
  parentCategoryId: null,
  imageUrl: ''
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createProductCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createProductCategory() {
  try {
    spinner.showSpinner()
    imageUrl.value.getFiles()
    const response = await $api?.productCategories.createProductCategory(productCategoryPayload.value)
    if (response.data.isSuccess) {
      productCategoryPayload.value = {
        name: '',
        description: '',
        parentCategoryId: null,
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


const imageUrl = ref(null)

function setFile(medias) {
  if (medias.length) {
    productCategoryPayload.value.imageUrl = medias[0]
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
            v-model.trim="productCategoryPayload.name"
            variant="outlined"
            density="compact"
            label="نام دسته بندی"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="productCategoryPayload.description"
            variant="outlined"
            density="compact"
            label="توضیحات دسته بندی"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <ProductCategoryPicker :required="false"  dropdown-label="انتخاب دسته بندی مادر" :return-object="false"
                                 v-model="productCategoryPayload.parentCategoryId"></ProductCategoryPicker>
        </VCol>
        <VCol cols="12" md="12">
          <Uploader ref="imageUrl"
                    @getFiles="setFile" label="تصویر دسته بندی"
                    :file-type="UploaderTypes.ProductCategory"></Uploader>
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
