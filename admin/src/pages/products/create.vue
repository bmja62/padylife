<script setup lang="ts">
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";
import {VForm} from "vuetify/components/VForm";
import {IProductBasicPayload} from "@/services/ProductService";
import ProductBasicForm from "@/components/Products/ProductBasicForm.vue";

const spinner = useSpinner()
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const refVForm = ref(null)
const router = useRouter()
const productBasic = ref<IProductBasicPayload>({
  name: '',
  description: '',
  categoryId: null,
  type: ''
})

async function createProductBasic() {
  try {
    spinner.showSpinner()
    const data = await $api?.product.createProduct(productBasic.value)
    if (data.data.isSuccess) {
      alert.success('محصول با موفقیت ساخته شد')
      router.push(`/products/${data.data.data.id}`)
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    createProductBasic()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

</script>

<template>
  <PageWrapper
  >
    <template #title>
      ایجاد یک محصول جدید
    </template>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <ProductBasicForm v-model="productBasic"></ProductBasicForm>
      <VCol md="12" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="green"
        >
          ثبت
        </VBtn>
      </VCol>
    </VForm>

  </PageWrapper>
</template>

<style scoped lang="scss">

</style>
