<script setup lang="ts">

import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";
const route = useRoute()
const productBasic = defineModel()
</script>

<template>
  <VRow>
    <VCol cols="12">
      <h2 class="border-b pb-2">
        اطلاعات پایه
      </h2>
    </VCol>
    <VCol cols="12" md="6">
      <VTextField
        v-model="productBasic.name"
        type="text"
        :rules="[(value) => !!value || 'این فیلد اجباری است']"
        required
        label="عنوان محصول"
      />
    </VCol>
    <VCol cols="12" md="6">
      <ProductCategoryPicker :return-object="false" required
                             v-model="productBasic.categoryId"></ProductCategoryPicker>
    </VCol>
    <VCol cols="12" md="6">
      <ProductTypePicker :disabled="route.params.id" :return-object="false" required
                         v-model="productBasic.type"></ProductTypePicker>
    </VCol>

    <VCol cols="12" md="12">
      <FroalaEditor
        v-model="productBasic.description"
        editor-placeholder="توضیحات "
      />
    </VCol>

  </VRow>

</template>

<style scoped lang="scss">

</style>
