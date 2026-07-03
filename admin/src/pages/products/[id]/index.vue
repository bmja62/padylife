<script setup lang="ts">

import {VForm} from "vuetify/components/VForm";
import ProductBasicForm from "@/components/Products/ProductBasicForm.vue";
import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";
import {IProduct, ProductType} from "@/services/ProductService";
import ProductAttributesForm from "@/components/Products/ProductAttributesForm.vue";
import ProductPriceForm from "@/components/Products/ProductPriceForm.vue";
import ProductMultiPriceForm from "@/components/Products/ProductMultiPriceForm.vue";
import {useAlerts} from "@/composables/alert";

onMounted(() => {
  getProductInfo()
})
const productInfo = ref<IProduct>(null)
const route = useRoute()
const $api = inject<IApiProvider>('$api')
const refVForm = ref(null)
async function getProductInfo() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.getProductById(route.params.id)
    productInfo.value = data.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    updateProduct()
  } else {
    useAlerts().error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function updateProduct() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.updateProduct({
      name: productInfo.value.name,
      description: productInfo.value.description,
      categoryId: productInfo.value.categoryId
    }, route.params.id)
    if (data.data.isSuccess) {
      useAlerts().success('اطلاعات پایه با موفقیت ویرایش شد')
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
  <PageWrapper
    v-if="productInfo"
  >
    <template #title>
      ویرایش محصول {{ productInfo.name }}
    </template>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <ProductBasicForm v-model="productInfo"></ProductBasicForm>
      <VCol md="12" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="warning"
        >
          ویرایش اطلاعات پایه
        </VBtn>
      </VCol>
    </VForm>

    <ProductMedia v-model="productInfo" @refetch="getProductInfo"></ProductMedia>
    <ProductAttributesForm v-model="productInfo" @refetch="getProductInfo"></ProductAttributesForm>
    <ProductPriceForm v-if="productInfo.type === ProductType.Simple" v-model="productInfo"></ProductPriceForm>
    <ProductMultiPriceForm v-if="productInfo.type === ProductType.Variant" @refetch="getProductInfo"
                           v-model="productInfo"></ProductMultiPriceForm>
  </PageWrapper>


</template>

<style scoped lang="scss">

</style>
