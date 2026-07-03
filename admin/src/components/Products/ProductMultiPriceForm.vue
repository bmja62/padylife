<script setup lang="ts">

import {IProduct} from "@/services/ProductService";
import type {VDataTable} from "vuetify/components";

const emits = defineEmits<{
  (e:'refetch'):void
}>()
const isRenderingAddVariantDialog = ref(false)
const productInfo: IProduct = defineModel()
const tableHeaders: VDataTable['headers'] = [
  {title: 'شناسه', key: 'id'},
  {title: 'SKU', key: 'sku'},
  {title: 'قیمت', key: 'price', value: (item) => `${Intl.NumberFormat('fa-IR').format(item.price)} تومان`},
  {title: 'مشخصه', key: 'attributeValues'},
  {title: 'عملیات', key: 'actions'},
]

</script>

<template>
  <VRow>
    <VCol cols="12" class="d-flex flex-md-row flex-column gap-2 align-items-center justify-space-between">
      <h2>
        قیمت گذاری پیشرفته محصول {{ productInfo.name }}
      </h2>
      <VBtn
        color="success"
        variant="flat"
        @click="isRenderingAddVariantDialog = true"
      >
        افزودن قیمت
      </VBtn>
    </VCol>
    <CustomTable
      :items-list="productInfo.variants"
      :count="100"
      :page-number="1"
      :table-headers="tableHeaders"
      :total-count="100"
    >

      <template #attributeValues="data">
        <div class="d-flex gap-1 align-content-center">
          <VChip color="primary" v-for="el in data.item.attributeValues">{{ el.attributeName }} {{ el.value }}</VChip>

        </div>
      </template>
      <template #actions="data">
        <VBtn
          color="warning"
          :to="`/products/inventory/${productInfo.id}/${data.item.id}`"
          variant="flat"
        >
          مدیریت موجودی
        </VBtn>
      </template>
    </CustomTable>

  <AddProductVariantDialog
    v-model:dialogState="isRenderingAddVariantDialog"
    @refetch="emits('refetch')"
  ></AddProductVariantDialog>
  </VRow>

</template>

<style scoped lang="scss">

</style>
