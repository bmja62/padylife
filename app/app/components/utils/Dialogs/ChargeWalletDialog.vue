<script setup lang="ts">
import * as Yup from "yup";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const {$api}: IApiProvider = useNuxtApp()
const amount = ref(null)
const localRegisterSchema = Yup.object<ILocalRegisterPayload>({
  amount: Yup.string().required("مبلغ شارژ اجباری است"),
});
const formattedPrice = computed({
      get() {
        return amount.value
      },
      set(newValue) {
        // This setter is getting number, replace all commas with empty str
        // Then start to separate numbers with ',' from beginning to prevent
        // from data corruption
        if (newValue) {
          amount.value = useUtils().convertNumbers2English(newValue)
              .toString()
              .replaceAll(',', '')
              .replace(/\B(?=(\d{3})+(?!\d))/g, ',')

          // Remove all characters that are NOT number
          amount.value = amount.value.replace(
              /[a-zA-Z+*!@#$%^&*()_;:'"|<>/?{}\u0600-\u06EC/\-/\.]/g,
              '',
          )
        } else if (!newValue || newValue === '') {
          amount.value = ''
        }
      },
    },
)

async function chargeWallet() {
  try {
    useSpinner().renderSpinner()
    if (amount.value.includes(','))
      amount.value = amount.value.replaceAll(',', '')
    const response = await $api.wallet.chargeWallet(amount.value)
    if (response.isSuccess) {
      window.location.replace(response.data.link)
    } else {
      useAlerts().error(response.message)
    }
      isRendering.value = false
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>شارژ کیف پول</span>
    </template>
    <template #default>
      <UtilsFormWrapper
          :schema="localRegisterSchema"
          @submit="chargeWallet"
      >
        <div class="w-full flex flex-col gap-2">

          <UtilsInputsBaseInput
              v-model="formattedPrice"
              name="amount"
              bordered
              placeholder="مبلغ (تومان)"
          ></UtilsInputsBaseInput>
          <button type="submit"
                  class="btn  bg-primary text-white !rounded-xl">شارژ کیف پول
          </button>
        </div>
      </UtilsFormWrapper>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>