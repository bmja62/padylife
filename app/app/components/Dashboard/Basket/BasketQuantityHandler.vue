<script setup lang="ts">
import {BasketItemType} from "~/services/BasketService";

const {$api} = useNuxtApp()

interface IProps {
  quantity: number,
  isVariant: boolean,
  dense: boolean,
  entityId: number
}

const props: IProps = defineProps({
  quantity: {
    type: Number as PropType<number>
  },
  entityId: {
    type: Number as PropType<number>
  },
  isVariant: {
    type: Boolean as PropType<boolean>
  },
  dense: {
    type: Boolean as PropType<boolean>,
    default: false
  },
})
const emits = defineEmits<{
  (e: 'refetch',quantity:number): void
}>()

async function removeBasketItem() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.removeBasketItem({
      itemId: props.entityId,
      basketItemType: props.isVariant ? BasketItemType.Variant : BasketItemType.Product
    })
    if (response.isSuccess) {
      emits('refetch')
    } else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function updateBasketItemQuantity(actionType) {
  let newQuantity = props.quantity
  actionType === 1 ? newQuantity++ : newQuantity--
  try {
    useSpinner().renderSpinner()
    const response = await $api.basket.updateItemQuantity({
      newQuantity: newQuantity,
      itemType: props.isVariant ? BasketItemType.Variant : BasketItemType.Product,
      itemId: props.entityId,
    })
    if (response.isSuccess) {
      emits('refetch',newQuantity)
    } else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <div :class="{'py-1 px-2':props.dense}"
       class="w-full flex p-3 items-center justify-between rounded-full border border-primary">
    <Icon
        @click="updateBasketItemQuantity(1)"
        name="icon:plus"
        class="[&_*]:fill-primary cursor-pointer"
        size="18"
    />
    <span>{{ props.quantity }}</span>
    <Icon
        @click="updateBasketItemQuantity(2)"
        v-if="props.quantity > 1"
        name="icon:minus"
        class="[&_*]:fill-primary cursor-pointer"
        size="2"
    />
    <Icon
        v-else
        @click="removeBasketItem"
        name="icon:trash"
        class="[&_*]:fill-primary cursor-pointer"
        size="18"
    />
  </div>
</template>

<style scoped>

</style>