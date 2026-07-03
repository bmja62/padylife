<script setup lang="ts">
import type {IBasketItemDetail} from "~/services/BasketService";
import {ProductPersianType} from "~/services/ProductService";

interface IProps {
  basketItem: IBasketItemDetail
}

const props: IProps = defineProps({
  basketItem: {
    type: Object as PropType<IBasketItemDetail>
  }
})
const emits = defineEmits<{
  (e: 'refetch'): void
}>()

function refetch() {
  emits('refetch')
}

const isVariant = computed(() => {
  return props.basketItem.type === ProductPersianType.Variant
})
</script>

<template>
  <div class="p-3 flex gap-2 bg-white rounded-lg shadow">
    <NuxtImg v-if="props.basketItem.imageUrl.main" :src="props.basketItem.imageUrl.main.url"
             class="h-full w-20 object-cover rounded-lg"></NuxtImg>
    <NuxtImg v-else src="/common/no-image.png" class="h-full w-20 object-cover rounded-lg"></NuxtImg>
    <div class="flex w-full flex-col gap-3 line-clamp-1">
      <strong>{{ props.basketItem.title }}</strong>
      <small>{{
          props.basketItem.variantAttributes ? props.basketItem.variantAttributes : props.basketItem.brand
        }}</small>
      <div class="w-full flex items-center gap-3">
        <LazyDashboardBasketQuantityHandler
            dense
            @refetch="refetch"
            :is-variant="isVariant" :quantity="props.basketItem.quantity"
            class="!w-1/2"
            :entity-id="props.basketItem.id"></LazyDashboardBasketQuantityHandler>
        <div style="overflow-wrap: anywhere">
          <span>{{
              Intl.NumberFormat('fa-IR').format(props.basketItem.unitPrice * props.basketItem.quantity)
            }} تومان</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>