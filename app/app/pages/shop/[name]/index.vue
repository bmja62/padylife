<script setup lang="ts">
import {
  type IInventoryStock,
  type IProduct,
  type IProductImage,
  type IProductVariant,
  type IVariantGroup,
  ProductType
} from "~/services/ProductService";
import {CommentType} from "~/models/Enums/CommentType";
import {BasketItemType} from "~/services/BasketService";

definePageMeta({
  layout: "dashboard",
});

// Variables

const {$api} = useNuxtApp()
const route = useRoute()
const router = useRouter()
const productInfo = ref<IProduct>(null)
const selectedImage = ref<IProductImage>(null)
const attributeLimit = ref(6)
const commentsLimit = ref(6)
const productGallery = ref<IProductImage[]>([])
const isRenderingSubmitRateAndCommentDialog = ref(false)
const showGuide = ref(false)
const entityComments = ref([])
const averageRating = ref(null)
const variantStock = ref<IInventoryStock>(null)
const entityCommentFilters = ref({
  entityId: null,
  entityType: CommentType.Product,
  pageNumber: 1,
  count: 100
})
const groupedByVariantsForShow = ref<IVariantGroup[]>([])
const selectedValues = ref([])
const selectedVariant = ref<IProductVariant>(null)
async function getProductById() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.product.getProductById(route.params.name);
    productInfo.value = response.data
    entityCommentFilters.value.entityId = productInfo.value.id
    if (productInfo.value.type === ProductType.Variant) {
      groupByVariants()
    } else {
      getVariantStock()

    }
  } catch (e) {
    console.log(e)
  } finally {
    generateProductGallery()
    useSpinner().hideSpinner()
  }
}

function groupByVariants() {
  const allAttributeValues = []
  let variantGroups;
  let groupedByVariants;

  // extracting all existing attributes from variants
  productInfo?.value?.variants.forEach((variant) => {
    variant?.attributeValues?.forEach((attributeValue) => {
      allAttributeValues.push({...attributeValue, variantId: variant.id})
    })
  })
  // group them by variant name eg رنگ
  variantGroups = Object.groupBy(allAttributeValues, (attributeValue) =>
      attributeValue.attributeName)

  // sort grouped variants into a readable object with corresponding values
  groupedByVariants = Object.keys(variantGroups).map((key) => {
    return {
      attributeId: variantGroups[key][0].attributeId,
      name: key,
      variants: variantGroups[key],
      keys: []
    }
  })

  // equivalent the transformed object into ref for two-way binding
  groupedByVariantsForShow.value = groupedByVariants.map(attribute => {
    const uniqueValues = [...new Set(attribute.variants.map((v) => v.value))];
    return {
      ...attribute,
      keys: uniqueValues
    };
  });

  selectedValues.value = groupedByVariantsForShow.value.map(() => '');

}

watch(() => selectedValues.value, async (val) => {
  await findVariant()
}, {immediate: true, deep: true})

async function findVariant() {
  const res = productInfo?.value?.variants.filter((variant) => {
    let isValid = true
    for (let i = 0; i < variant.attributeValues.length; i++) {
      if (!selectedValues.value.includes(variant.attributeValues[i].value)) {
        isValid = false
      }
    }
    if (isValid) {
      return variant
    }

  })
  if (res?.length) {
    selectedVariant.value = res[0]
    await getVariantStock()
  } else {
    selectedVariant.value = null
  }
}

async function getVariantStock() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.product.getProductOrVariantStock(productInfo.value.id, selectedVariant?.value?.id)
    variantStock.value = response.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}


async function addToBasket() {

  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.addBasketItem({
      quantity: 1,
      objectId: isVariantProduct.value ? selectedVariant.value.id : productInfo.value.id,
      itemType: isVariantProduct.value ? BasketItemType.Variant : BasketItemType.Product
    },)
    if (response.isSuccess) {
      useAlerts().success('به سبد خرید افزوده شد')
      isVariantProduct.value ? selectedVariant.value.basketQuantity++ : productInfo.value.basketQuantity++
    } else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

function generateProductGallery() {
  productGallery.value = productInfo.value.productImages.gallery
  if (productInfo.value.productImages.main) {
    productGallery.value.push(productInfo.value.productImages.main)
  }
  selectedImage.value = productGallery.value[0]
}


async function getEntityComments() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.comment.getEntityComments(entityCommentFilters.value)
    entityComments.value = response.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getAverageRating() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.rate.getAverageRating({
      entityId: entityCommentFilters.value.entityId,
      entityType: CommentType.Product,
    })
    averageRating.value = response.data.avg
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}


function showGuideTransition() {
  showGuide.value = true
  setTimeout(() => {
    showGuide.value = false
  }, 2000)
}

function setNewQuantity(quantity){
  isVariantProduct.value ? selectedVariant.value.basketQuantity = quantity : productInfo.value.basketQuantity = quantity

}

const shouldRenderCarousel = computed(() => {
  return productInfo.value.productImages.main || (productInfo.value.productImages.main && productInfo.value.productImages.gallery.length) || (!productInfo.value.productImages.main && productInfo.value.productImages.gallery.length)
})

const isVariantProduct = computed(() => {
  return productInfo?.value?.type === ProductType.Variant
})
const currentBasketQuantity = computed(() => {
  return isVariantProduct.value ? selectedVariant?.value?.basketQuantity : productInfo?.value?.basketQuantity
})

await getProductById()
await getEntityComments()
await getAverageRating()

useHead({
  title: productInfo?.value?.name
})
useServerHead({
  title: productInfo?.value?.name
})
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper
        v-if="productInfo"
      style="--wrapper-header-height: 80px"
    >
      <template #header>
        <BaseNotificationHeader>
          <template #title> {{ productInfo.name }}</template>
        </BaseNotificationHeader>
      </template>

      <div
        class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start p-2 gap-y-4"
      >

        <div v-if="shouldRenderCarousel" class="w-full flex flex-col items-center gap-y-4">
          <NuxtImg v-if="selectedImage" :src="selectedImage.url"
                   class="w-full h-[200px] bg-white object-contain rounded-[16px] shadow"/>
          <div

              class="p-2 bg-white rounded-xl flex flex-row justify-center items-center shadow gap-x-2"
          >
            <div class="carousel  gap-3 carousel-center custom-scrollbar ">
              <div v-for="image in productGallery" :key="image.id" class="carousel-item">
                <NuxtImg :src="image.url" @click="selectedImage = image"
                         :class="{'border border-primary':image.id === selectedImage.id}"
                         class="w-[42px] object-cover  rounded-lg h-[42px]"/>
              </div>

            </div>
          </div>
        </div>
        <div v-else class="w-full flex flex-col items-center gap-y-4">
          <NuxtImg src="/common/no-image.png" class="w-full h-[200px] object-cover  rounded-[16px] shadow "/>
        </div>
        <div class="w-full flex flex-col gap-2">
          <div class="w-full flex flex-row justify-start items-center">
            <strong class="text-gray-800">{{ productInfo.name }}</strong>
          </div>
          <div class="w-full flex flex-row justify-between items-center">
            <div class="flex flex-row justify-start items-center gap-2">
              <span class="text-gray-700 text-sm">{{ averageRating }} ({{ entityComments.length }})</span>
              <Icon name="icon:star" class="[&_*]:fill-[#E8E128]" size="20" />
            </div>
            <div v-if="productInfo.type === ProductType.Simple"
                 class="flex flex-row justify-center items-center gap-x-2">
              <span class="text-gray-800">{{ Intl.NumberFormat('fa-IR').format(productInfo.price) }}</span>
              <span class="text-gray-600 text-xs">تومان</span>
            </div>
            <div v-else class="flex flex-row justify-center items-center gap-x-2">
              <template v-if="selectedVariant">

                <span class="text-gray-800">{{ Intl.NumberFormat('fa-IR').format(selectedVariant.price) }}</span>
              <span class="text-gray-600 text-xs">تومان</span>
              </template>
              <span v-else @click="showGuideTransition" class="text-primary animate-bounce">انتخاب کنید</span>
            </div>
          </div>
        </div>

        <div
            v-if="productInfo.type === ProductType.Variant"
            :class="{'border !border-primary transition-all':showGuide}"
            class="w-full flex  p-1 border border-transparent rounded-[16px] flex-col justify-start  overflow-x-auto gap-x-2 pb-4"
        >
          <div
              v-for="(variant, idx) in groupedByVariantsForShow"
              :key="idx"
              class="flex flex-col gap-1">
            <strong>{{ variant.name }}</strong>
            <div class="flex flex-wrap items-center gap-2">

              <div
                  v-for="(key,keyIdx) in variant.keys"
                  :key="keyIdx"
                  class="min-w-12 w-12 h-12 rounded-[10px] flex flex-row justify-center items-center"
          >
                <input :id="key" ref="InputRadio"
                       v-model="selectedValues[idx]"
                       :checked="selectedValues.includes(key)"
                       :name="variant.attributeId" :value="key" type="radio"
                >
                <label :for="key" class="Cursor px-3 labelText">
                  <span class="text-DarkGrey"> {{ key }}</span>
                </label>

              </div>
            </div>
        </div>
        </div>

        <!--        <div class="w-full flex flex-col gap-4">-->
        <!--          <div class="w-full flex flex-row justify-between items-center">-->
        <!--            <span class="text-gray-700">زمان تحویل</span>-->
        <!--            <Icon-->
        <!--              name="icon:chevron-right"-->
        <!--              class="transform rotate-180"-->
        <!--              size="15"-->
        <!--            />-->
        <!--          </div>-->
        <!--          <div-->
        <!--            class="w-full flex flex-row justify-between items-center p-2 bg-[#ffff] rounded-[10px]"-->
        <!--          >-->
        <!--            <div class="flex flex-row justify-start items-center gap-x-2">-->
        <!--              <NuxtImg src="/junks/package.png" class="w-10 h-10" />-->
        <!--              <span class="text-[#4D4F55]">ارسال عادی</span>-->
        <!--            </div>-->
        <!--            <span class="text-[#6B6E78] text-sm">24 آذر 1403</span>-->
        <!--          </div>-->
        <!--        </div>-->
        <template v-if="(isVariantProduct && selectedVariant) || (productInfo.type === ProductType.Simple)">
          <template
              v-if="!currentBasketQuantity">
          <button
              v-if="variantStock?.summery?.totalAvailable"
              @click="addToBasket" type="button" class="w-full btn btn-primary !rounded-full ">
          افزودن به سبد خرید
        </button>
          <button
              v-else
              disabled
              type="button" class="w-full btn btn-primary !rounded-full ">
            ناموجود
          </button>
        </template>
          <template v-else>
            <LazyDashboardBasketQuantityHandler @refetch="setNewQuantity"
                                                :is-variant="isVariantProduct" :quantity="currentBasketQuantity"
                                                :entity-id="isVariantProduct ? selectedVariant?.id : productInfo?.id"></LazyDashboardBasketQuantityHandler>
          </template>
        </template>

        <div v-if="productInfo.attributes.length" class="w-full flex flex-col gap-4 bg-white p-2 rounded-[16px] shadow">
          <div class="w-full flex flex-row border-b pb-2 justify-between items-center">
            <span class="text-gray-700">مشخصات</span>

          </div>

          <div class="w-full grid grid-cols-2 relative gap-2 ">
            <div v-for="attribute in productInfo.attributes.filter((e,idx)=> idx<attributeLimit )"
                 :key="attribute.attributeId"
                 class="rounded-[16px] col-span-1 p-3 bg-gray-50 shadow flex flex-col gap-1">
              <span>{{ attribute.attributeName }}</span>
              <strong>{{ attribute.value }}</strong>
            </div>
            <LazyUtilsShowMoreOrLess v-model:total-count="productInfo.attributes.length" v-model:limit="attributeLimit"
                                     :capacity="6"></LazyUtilsShowMoreOrLess>
          </div>
        </div>
        <div class="w-full flex flex-col gap-4 bg-white p-2 rounded-[16px] shadow">
          <div class="w-full flex flex-row border-b pb-2 justify-between items-center">
            <span class="text-gray-700">نظرات خریداران</span>
            <button @click="isRenderingSubmitRateAndCommentDialog = true" type="button"
                    class="btn btn-sm bg-white border-primary !text-primary">
              ثبت نظر
            </button>
          </div>

          <div v-if="entityComments.length" class="w-full flex flex-col gap-4 relative">
            <div class="w-full flex flex-col gap-4 ">
              <LazySharedCommentCard v-for="comment in entityComments.filter((e,idx)=> idx<commentsLimit )"
                                     :key="comment.id" :comment="comment"
                                     @refetch="getEntityComments"></LazySharedCommentCard>
            </div>
            <LazyUtilsShowMoreOrLess
                v-model:total-count="entityComments.length" v-model:limit="commentsLimit"
                :capacity="6"></LazyUtilsShowMoreOrLess>

          </div>
          <span v-else class="text-gray-400 text-center">نظری ثبت نشده است</span>
        </div>
      </div>

    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsSubmitRateAndCommentDialog
        v-model="isRenderingSubmitRateAndCommentDialog" :entity-id="productInfo.id"
        label="محصول"
        :comment-type="CommentType.Product"></LazyUtilsDialogsSubmitRateAndCommentDialog>
  </div>
</template>

<style scoped>

.radio_container {
  display: flex;
  gap: 1rem;
  align-items: center;
  background-color: transparent;
  //width: 100%;
  height: 50px;
  border-radius: 9999px;
  cursor: pointer;
}

input[type="radio"] {
  appearance: none;
  display: none;
}

label {
  font-family: "iranSans", sans-serif;
  font-size: 12px;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: inherit;
  text-align: center;
  border-radius: 9999px;
  overflow: hidden;
  transition: linear 0.3s;
  color: #6e6e6edd;
}

.labelColor {
  width: 40px;
  height: 40px;
}

.labelText {
  min-width: 5rem;
  width: 100%;
  height: 30px;
  font-family: IRANYekan !important;

}

input[type="radio"]:checked + label {
  @apply text-primary !important;
  color: #f1f3f5;
  font-weight: 900;
  transition: 0.3s;
}


</style>
