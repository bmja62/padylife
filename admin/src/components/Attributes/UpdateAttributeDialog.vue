<script setup lang="ts">
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {IAttribute} from "@/services/ProductAttributes";
import ProductAttributeTypePicker from "@/components/Attributes/ProductAttributeTypePicker.vue";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IAttribute
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
    updateProductAttribute()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateProductAttribute() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.productAttributes.updateProductAttribute(props.selectedItem)
      if (response.data.isSuccess) {
        alert.success('ویژگی با موفقیت ویرایش شد!')
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
    title="ویرایش ویژگی"
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
            label="نام ویژگی"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <ProductAttributeTypePicker required :return-object="false"
                                      v-model="props.selectedItem.type"></ProductAttributeTypePicker>
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.description"
            variant="outlined"
            density="compact"
            label="توضیحات ویژگی"
            color="success"
          />
        </VCol>
      </VRow>
    </VForm>

  </CustomUpdateDialog>
</template>
