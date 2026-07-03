<script setup lang="ts">
import type {IProduct} from "~/services/ProductService";
import type {IProductCategory} from "~/services/ProductCategoryService";

definePageMeta({
  layout: "dashboard",
});
useHead({
  title: 'فروشگاه'
})
const {$api} = useNuxtApp()
const productsFilters = ref({
  pageNumber: 1,
  count: 10
})
const products = ref<IProduct[]>([])
const productCategoryDetail = ref<IProductCategory>(null)
const totalCount = ref(null)
const route = useRoute()

async function getAllProducts() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.product.getAllProduct(productsFilters.value);
    products.value = response.data.data
    totalCount.value = response.data.totalCount

  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getProductCategoryDetail() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.productCategory.getProductCategoryById(route.params.slug);
    productCategoryDetail.value = response.data
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
function changePage(page: number) {
  productsFilters.value.pageNumber = page
  getAllProducts()
}
getAllProducts()
getProductCategoryDetail()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper
        style="--wrapper-header-height: 80px"
        inner-class="!pt-0 !px-0"
    >
      <template #header>
        <BaseNotificationHeader>
          <template #title> {{ productCategoryDetail?.name }}</template>
        </BaseNotificationHeader>
      </template>

      <div
          class="w-full h-full bg-[#F7F8FE] py-3 rounded-t-[32px] flex flex-col justify-start items-start gap-y-4"
      >

        <div
            class="w-full flex flex-col justify-start items-start gap-y-4 px-5"
        >

          <div class="w-full grid grid-cols-2 gap-4">
            <nuxt-link
                v-for="product in products"
                :key="product.id"
                :to="`/shop/${product.id}`"
                class="w-full flex flex-col items-center gap-2"
            >
              <NuxtImg
                  v-if="product.productImages.main"
                  :src="product.productImages.main.url"
                  class="w-full h-[8rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
              />
              <NuxtImg
                  v-else
                  src="/common/no-image.png"
                  class="w-full h-[8rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
              />
              <span class="text-gray-700">{{ product.name }}</span>
            </nuxt-link>
          </div>
          <div class="w-full flex items-center justify-center">
            <UtilsCustomPagination
                v-if="totalCount"
                :page-number="productsFilters.pageNumber"
                :count="productsFilters.count"
                :total-count="totalCount"
                @change-page="changePage"
            />
          </div>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
