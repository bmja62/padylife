`
<script setup lang="ts">
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {ICreateZonePayload} from "@/services/WarehouseService";

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
const route = useRoute()
const zonePayload = ref<ICreateZonePayload>({
  name: '',
  code: '',
  capacity: null,
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createWarehouseZone()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createWarehouseZone() {
  try {
    spinner.showSpinner()
    const response = await $api?.warehouse.createWarehouseZone(zonePayload.value,route.params.id)
    if (response.data.isSuccess) {
      zonePayload.value = {
        name: '',
        code: '',
        capacity: null,
      }
      alert.success('منطقه با موفقیت ایجاد شد!')
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
    title="ایجاد منطقه جدید"
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
            v-model.trim="zonePayload.name"
            variant="outlined"
            density="compact"
            label="نام منطقه"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="zonePayload.code"
            variant="outlined"
            density="compact"
            label="کد منطقه"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="zonePayload.capacity"
            variant="outlined"
            density="compact"
            type="number"
            label="ظرفیت (عدد)"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
