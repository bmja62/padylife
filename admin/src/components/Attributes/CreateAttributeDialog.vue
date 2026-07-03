`
<script setup lang="ts">
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {ICreateAttributePayload} from "@/services/ProductAttributes";
import ProductAttributeTypePicker from "@/components/Attributes/ProductAttributeTypePicker.vue";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

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
const attributePayload = ref<ICreateAttributePayload>({
  name: '',
  description: '',
  type: '',
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createAttribute()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createAttribute() {
    try {
      spinner.showSpinner()
      const response = await $api?.productAttributes.createProductAttribute(attributePayload.value)
      if (response.data.isSuccess) {
      attributePayload.value = {
        name: '',
        description: '',
        type: '',
      }
        alert.success('ویژگی با موفقیت ایجاد شد!')
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
    title="ایجاد ویژگی جدید"
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
        v-model.trim="attributePayload.name"
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
                                  v-model="attributePayload.type"></ProductAttributeTypePicker>
    </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="attributePayload.description"
            variant="outlined"
            density="compact"
            label="توضیحات ویژگی"
            color="success"
          />
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
