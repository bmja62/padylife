<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IProductCategory} from "@/services/ProductCategories";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IProductCategory
}

// Props
const props = withDefaults(defineProps<IProps>(), {})

// Emits
const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateProductCategory()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateProductCategory() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      imageUrl.value.getFiles()
      const response = await $api?.productCategories.updateProductCategory(props.selectedItem, props.selectedItem.id)
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

const imageUrl = ref(null)

function setFile(medias) {
  if (medias.length) {
    props.selectedItem.imageUrl = medias[0]
  }

}
</script>

<template>
  <CustomUpdateDialog
    :dialog-state="props.dialogState"
    title="ویرایش دسته بندی"
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
            v-model.trim="props.selectedItem.name"
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
            v-model.trim="props.selectedItem.description"
            variant="outlined"
            density="compact"
            label="توضیحات دسته بندی"
            color="success"
          />
        </VCol>
        <VCol cols="12">
          <ProductCategoryPicker dropdown-label="انتخاب دسته بندی مادر" :return-object="false"
                                 v-model="props.selectedItem.parentCategoryId"></ProductCategoryPicker>
        </VCol>
        <VCol cols="12" md="12">
          <Uploader ref="imageUrl"
                    :default-medias="props.selectedItem.imageUrl"
                    @getFiles="setFile" label="تصویر دسته بندی"
                    :file-type="UploaderTypes.ProductCategory"></Uploader>
        </VCol>
      </VRow>
    </VForm>


  </CustomUpdateDialog>
</template>
