<script setup lang="ts">
import type {IProduct} from "~/services/ProductService";

interface IProps {
  product: IProduct
}

const props: IProps = defineProps({
  product: {
    type: Object as PropType<object>
  }
})
</script>

<template>
  <NuxtLink
      :to="`/shop/${props.product.id}`"
      class="w-[10rem] flex flex-col items-center gap-2"
  >
    <div
        class="w-full h-[10rem] flex flex-col justify-start bg-white border border-[#E2E6FF] rounded-[16px]"
    >
      <div class="w-full h-2/3">
        <NuxtImg
            v-if="props.product.productImages.main"
            :src="props.product.productImages.main.url"
            class="w-full rounded-t-[16px] h-[6rem] object-cover"
        />
        <NuxtImg
            v-else
            src="/common/no-image.png"
            class="w-full rounded-t-[16px] h-[6rem] object-cover"
        />
      </div>
      <div class="w-full h-1/3 flex flex-col gap-2 px-2">
        <p class="text-xs font-bold">{{ props.product.name }}</p>
        <div v-if="props.product.price" class="w-full  flex items-center">
          <p  class="text-[#464850] text-xs font-bold">{{ Intl.NumberFormat('fa-IR').format(props.product.price) }}</p>
          <p class="text-[#828282] text-[10px] font-bold mr-2">تومان</p>
        </div>
      </div>
    </div>
  </NuxtLink>
</template>

<style scoped>

</style>