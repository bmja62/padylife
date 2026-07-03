<script lang="ts" setup>
import {isAxiosError} from 'axios'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import EntityIsActivePicker from "@/components/Countries/EntityIsActivePicker.vue";
import {IEntity} from "@/services/CityService";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: IEntity
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
    updateEntity()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateEntity() {
  if (props.selectedItem) {
    try {
      spinner.showSpinner()
      const response = await $api?.cities.update(props.selectedItem)
      if (response.data.isSuccess) {
        alert.success('عملیات با موفقیت انجام شد')
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
    title="ویرایش شهر"
    @update="validateData"
    @update:dialog-state="updateDialogState"
  >
    <VForm
      class="w-100"
      ref="refVForm"
    >
      <VRow>
        <VCol cols="12">
          <ProvincePicker :return-object="false" v-model="props.selectedItem.provinceId"></ProvincePicker>
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.cityNameFa"
            variant="outlined"
            density="compact"
            label="نام فارسی شهر"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.cityName"
            variant="outlined"
            density="compact"
            label="نام انگلیسی شهر"
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <VTextField
            v-model.trim="props.selectedItem.cityCode"
            variant="outlined"
            density="compact"
            label="کد شهر "
            color="success"
            required
            :rules="[(value) => !!value || 'فیلد اجباری است']"
          />
        </VCol>
        <VCol cols="12">
          <EntityIsActivePicker v-model="props.selectedItem.isActive"></EntityIsActivePicker>
        </VCol>
      </VRow>
    </VForm>


  </CustomUpdateDialog>
</template>
