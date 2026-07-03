<script setup lang="ts">

import {IProduct, IProductAttribute} from "@/services/ProductService";
import {useSpinner} from "@/composables/spinner";
import {useAlerts} from "@/composables/alert";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";
import {VForm} from "vuetify/components/VForm";
import {IAttribute} from "@/services/ProductAttributes";

const productInfo: IProduct = defineModel()
const selectedAttributes = ref<IProductAttribute[]>([])
const route = useRoute()
const refVForm = ref(null)
const $api = inject<IApiProvider>('$api')
const emits = defineEmits<{
  (e: 'refetch'): void
}>()

async function removeProperty(item: IProductAttribute, idx: number) {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.RemoveProductAttributeValue({
      attributeId: item.attributeId,
      productId: route.params.id
    })
    if (data.data.isSuccess) {
      productInfo.value.attributes.splice(idx, 1)
    } else {
      useAlerts().error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    await addProductAttributeValues()
  } else {
    useAlerts().error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function addProductAttributeValues() {
  for (let i = 0; i < selectedAttributes.value.length; i++) {
    await addProductAttributeValue(selectedAttributes.value[i])
  }
  emits('refetch')
  useAlerts().success('مشخصات با موفقیت به محصول اضافه شدند')
  selectedAttributes.value = []

}

async function addProductAttributeValue(item: IAttribute) {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.addProductAttributeValue({
      attributeId: item.id,
      productId: route.params.id,
      value: item.value
    })
    if (data.data.isSuccess) {
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
  <VRow>
    <VCol cols="12">
      <h2 class="border-b pb-2">
        مشخصات محصول
      </h2>
    </VCol>
    <VCol cols="12">
      <h3 class="">
        افزودن مشخصه جدید
      </h3>
    </VCol>
    <VCol cols="12" md="12">
      <ProductAttributePicker dropdown-label="انتخاب مشخصه" multiple chips
                              v-model="selectedAttributes"></ProductAttributePicker>
    </VCol>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <VCol v-for="(item,idx) in selectedAttributes" :key="item.attributeId" cols="12">
        <VRow>
          <VCol md="2" cols="12" class="cursor-pointer" @click="selectedAttributes.splice(idx,1)">
            <VBtn
              color="red"
              elevation="0"
            >
              حذف از انتخاب‌ها
            </VBtn>
          </VCol>
          <VCol md="5" cols="12" class="col-md-2">
            <VTextField
              v-model="item.name"
              disabled
              color="success"
              density="compact"
              hide-details="auto"
              label="عنوان مشخصه"
              type="text"
              variant="outlined"
            />
          </VCol>
          <VCol md="5" cols="12" class="col-md-3">
            <VTextField
              v-model="item.value"
              color="success"
              density="compact"
              hide-details
              label="مقدار"
              :rules="[(value) => !!value || 'این فیلد اجباری است']"
              required
              type="text"
              variant="outlined"
            />

          </VCol>
        </VRow>
      </VCol>
      <VCol v-if="selectedAttributes.length" cols="12" class="d-flex justify-end">
        <VBtn
          type="submit"
          id="buy-now-btn"
          class="product-buy-now"
          color="warning"
        >
          افزودن مشخصات به محصول
        </VBtn>
      </VCol>
    </VForm>

    <VCol v-if="productInfo.attributes.length" cols="12">
      <h3 class="">
        مشخصات محصول
      </h3>
    </VCol>

    <VCol v-for="(item,idx) in productInfo.attributes" :key="item.attributeId" cols="2">
      <VCard density="compact" color="grey-lighten-4">
        <VCardTitle>
          {{ item.attributeName }}
        </VCardTitle>
        <VCardText>{{ item.value }}</VCardText>
        <VCardActions>
          <VBtn
            @click="removeProperty(item,idx)"
            color="red"
          >
            حذف
          </VBtn>
        </VCardActions>
      </VCard>
      <!--      <VRow>-->
      <!--        <VCol md="1" cols="12" class="cursor-pointer" @click="removeProperty(item,idx)">-->
      <!--          <VBtn-->
      <!--            density="compact"-->
      <!--            color="red"-->
      <!--            elevation="0"-->
      <!--            icon="mdi-close"-->
      <!--          />-->
      <!--        </VCol>-->
      <!--        <VCol md="5" cols="12" class="col-md-2">-->
      <!--          <VTextField-->
      <!--            v-model="item.attributeName"-->
      <!--            disabled-->
      <!--            color="success"-->
      <!--            density="compact"-->
      <!--            hide-details="auto"-->
      <!--            label="مقدار جزئیات"-->
      <!--            type="text"-->
      <!--            variant="outlined"-->
      <!--          />-->
      <!--        </VCol>-->
      <!--        <VCol md="6" cols="12" class="col-md-3">-->
      <!--          <VTextField-->
      <!--            v-model="item.value"-->
      <!--            color="success"-->
      <!--            density="compact"-->
      <!--            disabled-->
      <!--            hide-details="auto"-->
      <!--            label="مقدار جزئیات"-->
      <!--            type="text"-->
      <!--            variant="outlined"-->
      <!--          />-->

      <!--        </VCol>-->
      <!--      </VRow>-->
    </VCol>


  </VRow>

</template>

<style scoped lang="scss">

</style>
