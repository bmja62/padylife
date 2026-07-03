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
  count: 8
})
const products = ref<IProduct[]>([])
const productCategories = ref<IProductCategory[]>([])
const totalCount = ref(null)

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

async function getAllProductCategories() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.productCategory.getAllProductCategories({
      count: 6,
      pageNumber: 1
    });
    productCategories.value = response.data.data
  } catch (e) {
    console.log(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

getAllProducts()
getAllProductCategories()
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper
      style="--wrapper-header-height: 80px"
      inner-class="!pt-0 !px-0"
    >
      <template #header>
        <BaseNotificationHeader>
          <template #title> فروشگاه </template>
        </BaseNotificationHeader>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start gap-y-4"
      >
        <div
          class="w-full h-[10rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[30px]"
        >
          <span class="text-gray-700">بنر تبلیغاتی</span>
        </div>
        <div
          class="w-full flex flex-col justify-start items-start gap-y-4 px-5"
        >
          <!--          <div class="w-full flex flex-row justify-between items-center px-4">-->
          <!--            <div class="flex flex-col items-center">-->
          <!--              <div class="w-16 h-16 bg-[#D9D9D9] rounded-full"></div>-->
          <!--              <span class="text-gray-800 text-sm">پرفروش‌ها</span>-->
          <!--            </div>-->
          <!--            <div class="flex flex-col items-center">-->
          <!--              <div class="w-16 h-16 bg-[#D9D9D9] rounded-full"></div>-->
          <!--              <span class="text-gray-800 text-sm">پرتخفیف‌ها</span>-->
          <!--            </div>-->
          <!--            <div class="flex flex-col items-center">-->
          <!--              <div class="w-16 h-16 bg-[#D9D9D9] rounded-full"></div>-->
          <!--              <span class="text-gray-800 text-sm">شگفت انگیز‌ها</span>-->
          <!--            </div>-->
          <!--          </div>-->

          <div class="w-full grid grid-cols-2 gap-x-4">
            <div
              class="w-full h-[8rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
            >
              <span class="text-gray-700">بنر تبلیغاتی</span>
            </div>
            <div
              class="w-full h-[8rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
            >
              <span class="text-gray-700">بنر تبلیغاتی</span>
            </div>
          </div>

          <div class="w-full flex flex-col gap-y-4 mt-3">
            <strong class="text-gray-800">آخرین محصولات</strong>

            <div class="w-full grid grid-cols-2 gap-4">
              <nuxt-link :to="`/shop/${product.id}`"
                         v-for="product in products.filter((e,idx)=> idx<4)"
                         :key="product.id"
                class="w-full flex flex-col items-center gap-2"
              >
                <NuxtImg
                    v-if="product.productImages.main"
                    :src="product.productImages.main.url"
                    class="w-full h-[8rem] flex flex-row object-contain justify-center items-center bg-[#E2E6FF] rounded-[16px]"
                />
                <NuxtImg
                    v-else
                    src="/common/no-image.png"
                    class="w-full h-[8rem] flex flex-row object-contain justify-center items-center bg-[#E2E6FF] rounded-[16px]"
                />
                <span class="text-gray-700">{{ product.name }}</span>
              </nuxt-link>
            </div>
          </div>

          <div class="w-full flex flex-col gap-y-4 mt-3">
            <strong class="text-gray-800">دسته‌بندی‌ها</strong>

            <div class="w-full grid grid-cols-3 gap-4">
              <nuxt-link :to="`/shop/category/${item.id}`"
                  v-for="item in productCategories"
                  :key="item.id"
                class="w-full flex flex-col items-center gap-2"
              >
                <NuxtImg
                    class="w-full h-[6rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
                    v-if="item.imageUrl" :src="item.imageUrl"></NuxtImg>
                <NuxtImg
                    class="w-full h-[6rem] flex flex-row justify-center items-center bg-[#E2E6FF] rounded-[16px]"
                    v-else src="/common/no-image.png"></NuxtImg>
                <span class="text-gray-700">{{ item.name }}</span>
              </nuxt-link>
            </div>
          </div>


        </div>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped></style>
