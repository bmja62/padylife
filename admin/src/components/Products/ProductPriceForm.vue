<script setup lang="ts">
import {IProduct, ISimplePricePayload} from "@/services/ProductService";
import {VForm} from "vuetify/components/VForm";
import {useSpinner} from "@/composables/spinner";
import {useAlerts} from "@/composables/alert";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";

const productInfo: IProduct = defineModel()
const $api = inject<IApiProvider>('$api')
const refVForm = ref(null)
const route = useRoute()
const pricePayload = ref<ISimplePricePayload>({
  basePrice: 0,
  variantPrices: null
})

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    await setProductPrice()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function setProductPrice() {
  try {
    useSpinner().showSpinner()
    pricePayload.value.basePrice = productInfo.value.price
    const data = await $api?.product.setSimplePrice(pricePayload.value, route.params.id)
    if (data.data.isSuccess) {
      useAlerts().success('قیمت با موفقیت ثبت شد')
    } else {
      useAlerts().error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <VForm
    class="w-100 pt-8"
    ref="refVForm"
    @submit.prevent="validateData"
  >
    <VRow>
      <VCol cols="12" class="d-flex border-t   flex-md-row flex-column gap-2 align-items-center justify-space-between">
        <h2 class="">
          قیمت گذاری ساده محصول {{ productInfo.name }}
        </h2>
        <VBtn
          color="warning"
          :to="`/products/inventory/${productInfo.id}`"
          variant="flat"
        >
          مدیریت موجودی
        </VBtn>
      </VCol>


      <VCol md="10">
        <VTextField
          v-model="productInfo.price"
          color="success"
          density="compact"
          hide-details="auto"
          label="قیمت محصول (تومان)"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol md="2">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="success"
        >
          ثبت قیمت
        </VBtn>
      </VCol>

    </VRow>
  </VForm>
</template>

<style scoped lang="scss">

</style>
