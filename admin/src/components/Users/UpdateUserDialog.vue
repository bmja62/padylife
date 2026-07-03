<script setup lang="ts">
import {isAxiosError} from 'axios'
import {IUpdateUser} from '@/services/UserService'
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import GenderTypePicker from "@/components/Users/GenderTypePicker.vue";

// Interfaces
interface IProps {
  dialogState: boolean
  defaultUser: IUpdateUser
}

interface IEmit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  defaultUser: () => {
    return {}
  },
})

// Emits
const emit = defineEmits<IEmit>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()
const refVForm = ref(null)

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}


async function updateAUser() {
  if (props.defaultUser) {
    try {
      spinner.showSpinner()

      const response = await $api?.users.updateAUser(props.defaultUser, props.defaultUser.id)
      if (response?.data.isSuccess) {
        alert.success('کاربر با موفقیت ویرایش شد!')
        emit('update:dialogState', false)
        emit('refetch')
      } else {
        alert.error(response?.data.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
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

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateAUser()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
</script>

<template>
  <CustomUpdateDialog
    persistent
    :dialog-state="props.dialogState"
    :title="`ویرایش کاربر `"
    @update:dialog-state="updateDialogState"
    @update="validateData"

  >
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent
    >
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.fullName"
          variant="outlined"
          density="compact"
          label="نام و نام خانوادگی"
          color="success"
          required
          :rules="[(value) => !!value || 'فیلد اجباری است']"
          hide-details
        />
      </VCol>
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.age"
          variant="outlined"
          density="compact"
          label="سن"
          type="number"
          color="success"
          required
          hide-details
        />
      </VCol>
      <VCol cols="12">
        <GenderTypePicker v-model="props.defaultUser.gender"></GenderTypePicker>
      </VCol>
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.phoneNumber"
          variant="outlined"
          density="compact"
          label="شماره موبایل"
          color="success"
          :rules="[(value) => !!value || 'فیلد اجباری است']"
          required
          hide-details
        />
      </VCol>
<!--      <VCol cols="12">-->
<!--        <CustomDatePicker simple v-model="props.defaultUser.birthDate" label="تاریخ تولد"-->
<!--                          input-id="birthDate"></CustomDatePicker>-->
<!--      </VCol>-->
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.hight"
          variant="outlined"
          type="number"
          density="compact"
          label="قد"
          color="success"
          hide-details
        />
      </VCol>
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.wight"
          variant="outlined"
          type="number"
          density="compact"
          label="وزن"
          color="success"
          hide-details
        />
      </VCol>
      <VCol cols="12">
        <MaritalStatusPicker v-model="props.defaultUser.maritalStatus"></MaritalStatusPicker>
      </VCol>
      <VCol cols="12">
        <VTextField
          v-model.trim="props.defaultUser.instagramId"
          variant="outlined"
          density="compact"
          label="آیدی اینستاگرام"
          color="success"
          hide-details
        />
      </VCol>
    </VForm>

    <!--    <VCol cols="12">-->
    <!--      <RolePicker-->
    <!--        v-model="props.defaultUser.typeId"-->
    <!--      />-->
    <!--    </VCol>-->

  </CustomUpdateDialog>
</template>
