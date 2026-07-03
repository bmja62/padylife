<script setup lang="ts">
import type {ICreateAddressPayload} from "~/services/AddressService";
import * as Yup from "yup";
import CountryPicker from "~/components/utils/Pickers/CountryPicker.vue";
import ProvincePicker from "~/components/utils/Pickers/ProvincePicker.vue";
import CityPicker from "~/components/utils/Pickers/CityPicker.vue";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const emits = defineEmits<{
  (e:'refetch'):void
}>()
const {$api} = useNuxtApp<IApiProvider>()
async function createAddress() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.address.createAddress(addressPayload.value)
    if (response.isSuccess) {
      useAlerts().success('آدرس با موفقیت ثبت شد')
      isRendering.value = false
      addressPayload.value = {
        userId: authStore.getUser.id,
        countryId: null,
        provinceId: null,
        cityId: null,
        postalCode: '',
        addressText: '',
        plaque: null,
        unit: null,
        floor: null,
        recipientName: '',
        recipientPhone: '',
        landlinePhone: '',
        isDefault: false,
        addressType: '',
      }
      emits('refetch')

    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

const authStore = useAuthStore()
const addressPayload = ref<ICreateAddressPayload>({
  userId: authStore.getUser.id,
  countryId: null,
  provinceId: null,
  cityId: null,
  postalCode: '',
  addressText: '',
  plaque: null,
  unit: null,
  floor: null,
  recipientName: '',
  recipientPhone: '',
  landlinePhone: '',
  isDefault: false,
  addressType: '',
  // geoLocation: '',
})
const addressSchema = Yup.object<ICreateAddressPayload>({
  addressText: Yup.string().required("آدرس کامل اجباری است"),
  recipientName: Yup.string().required("نام گیرنده اجباری است"),
  recipientPhone: Yup.string().required("شماره تماس گیرنده اجباری است"),
});
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>ایجاد آدرس جدید</span>
    </template>
    <template #default>
      <UtilsFormWrapper :schema="addressSchema" @submit="createAddress">
        <div class="w-full flex flex-col gap-3">
          <LazyUtilsPickersAddressTypePicker v-model="addressPayload.addressType"></LazyUtilsPickersAddressTypePicker>
          <UtilsInputsBaseInput
              v-model="addressPayload.recipientName"
              name="recipientName"
              bordered
              placeholder="نام گیرنده"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.recipientPhone"
              name="recipientPhone"
              bordered
              placeholder="شماره تماس  گیرنده"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.landlinePhone"
              name="landlinePhone"
              bordered
              placeholder="شماره تلفن خط ثابت"
          ></UtilsInputsBaseInput>
          <CountryPicker v-model="addressPayload.countryId"></CountryPicker>
          <ProvincePicker v-if="addressPayload.countryId" :countryId="addressPayload.countryId" v-model="addressPayload.provinceId"></ProvincePicker>
          <CityPicker v-if="addressPayload.provinceId" :provinceId="addressPayload.provinceId" v-model="addressPayload.cityId"></CityPicker>
          <UtilsInputsBaseInput
              v-model="addressPayload.addressText"
              name="addressText"
              bordered
              placeholder="آدرس کامل"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.plaque"
              name="plaque"
              bordered
              placeholder="پلاک"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.floor"
              name="floor"
              bordered
              placeholder="طبقه"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.unit"
              name="unit"
              bordered
              placeholder="واحد"
          ></UtilsInputsBaseInput>
          <UtilsInputsBaseInput
              v-model="addressPayload.postalCode"
              name="postalCode"
              bordered
              placeholder="کدپستی"
          ></UtilsInputsBaseInput>
          <button type="submit" class="btn bg-primary text-white">
            ثبت
          </button>
        </div>
      </UtilsFormWrapper>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>