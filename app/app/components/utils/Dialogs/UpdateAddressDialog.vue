<script setup lang="ts">
import type {IAddress} from "~/services/AddressService";
import * as Yup from "yup";
import CountryPicker from "~/components/utils/Pickers/CountryPicker.vue";
import ProvincePicker from "~/components/utils/Pickers/ProvincePicker.vue";
import CityPicker from "~/components/utils/Pickers/CityPicker.vue";
import type {IApiProvider} from "~/models/IApiProvider";

interface IProps {
  selectedAddress: IAddress
}

const props: IProps = defineProps({
  selectedAddress: {
    type: Object as PropType<IAddress>
  }
})
const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'refetch'): void
}>()
const {$api} = useNuxtApp<IApiProvider>()

async function updateAddress() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.address.updateAddress(props.selectedAddress)
    if (response.isSuccess) {
      useAlerts().success('آدرس با موفقیت ویرایش شد')
      isRendering.value = false
      emits('refetch')

    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    if (error.statusCode === 400) {
      useAlerts().error(error.message)
      isRendering.value = false
    }
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

const authStore = useAuthStore()

const addressSchema = Yup.object<IAddress>({
  addressText: Yup.string().required("آدرس کامل اجباری است"),
  recipientName: Yup.string().required("نام گیرنده اجباری است"),
  recipientPhone: Yup.string().required("شماره تماس گیرنده اجباری است"),
});
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>ویرایش آدرس</span>
    </template>
    <template #default>
      <UtilsFormWrapper :schema="addressSchema" @submit="updateAddress">
        <div class="w-full flex flex-col gap-3">
          <LazyUtilsPickersAddressTypePicker
              v-model="props.selectedAddress.addressType"></LazyUtilsPickersAddressTypePicker>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.recipientName"
              name="recipientName"
              bordered
              placeholder="نام گیرنده"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.recipientPhone"
              name="recipientPhone"
              bordered
              placeholder="شماره تماس  گیرنده"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.landlinePhone"
              name="landlinePhone"
              bordered
              placeholder="شماره تلفن خط ثابت"
          ></UtilsInputsBaseInput>
          <CountryPicker v-model="props.selectedAddress.countryId"></CountryPicker>
          <ProvincePicker v-if="props.selectedAddress.countryId" :countryId="props.selectedAddress.countryId"
                          v-model="props.selectedAddress.provinceId"></ProvincePicker>
          <CityPicker v-if="props.selectedAddress.provinceId" :provinceId="props.selectedAddress.provinceId"
                      v-model="props.selectedAddress.cityId"></CityPicker>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.addressText"
              name="addressText"
              bordered
              placeholder="آدرس کامل"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.plaque"
              name="plaque"
              bordered
              placeholder="پلاک"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.floor"
              name="floor"
              bordered
              placeholder="طبقه"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.unit"
              name="unit"
              bordered
              placeholder="واحد"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="props.selectedAddress.postalCode"
              name="postalCode"
              bordered
              placeholder="کدپستی"
          ></UtilsInputsBaseInput>
          <button type="submit" class="btn bg-primary text-white">
            ویرایش
          </button>
        </div>
      </UtilsFormWrapper>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>