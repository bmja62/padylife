<script setup lang="ts">
import {isAxiosError} from 'axios'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {ICreateEntityPayload} from "@/services/ProvinceService";
import EntityIsActivePicker from "@/components/Countries/EntityIsActivePicker.vue";

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
const entityPayload = ref<ICreateEntityPayload>({
  countryId: null,
  provinceName: "",
  provinceNameFa: "",
  provinceCode: "",
  isActive: true,
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createEntity()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function createEntity() {
  try {
    spinner.showSpinner()
    const response = await $api?.provinces.create(entityPayload.value)
    if (response.data.isSuccess) {
      entityPayload.value = {
        countryId: null,
        provinceName: "",
        provinceNameFa: "",
        provinceCode: "",
        isActive: true,
      }
      alert.success('عملیات با موفقیت انجام شد !')
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
    title="ایجاد استان جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <CountryPicker :return-object="false" v-model="entityPayload.countryId"></CountryPicker>
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="entityPayload.provinceNameFa"
            variant="outlined"
            density="compact"
            label="نام فارسی استان"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="entityPayload.provinceName"
            variant="outlined"
            density="compact"
            label="نام انگلیسی استان"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="entityPayload.provinceCode"
            variant="outlined"
            density="compact"
            label="کد استان "
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <EntityIsActivePicker v-model="entityPayload.isActive"></EntityIsActivePicker>
        </VCol>
      </VRow>
    </VForm>
  </CustomCreateDialog>
</template>
