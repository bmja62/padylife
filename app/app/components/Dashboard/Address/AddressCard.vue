<script setup lang="ts">

import EditIcon from "~/components/Icons/EditIcon.vue";
import TrashIcon from "~/components/Icons/TrashIcon.vue";
import type {IAddress} from "~/services/AddressService";

interface IProps {
  address: IAddress
}

const props: IProps = defineProps({
  address: {
    type: Object as PropType<IAddress>
  }
})
const emits = defineEmits<{
  (e: 'openDeleteDialog', address: IAddress): void
  (e: 'openUpdateDialog', address: IAddress): void
}>()
</script>

<template>
  <div class="w-full p-3 rounded-lg shadow-lg bg-white">
    <div class="flex items-center my-2 justify-between w-full">
      <span>نام گیرنده : {{ props.address.recipientName }}</span>
      <div @click="emits('openDeleteDialog',props.address)">
        <TrashIcon class="fill-primary"></TrashIcon>
      </div>
    </div>
    <div class="flex items-center my-2 justify-between w-full"><span
        class="line-clamp-1">{{ props.address.addressText }}</span>
      <div @click="emits('openUpdateDialog',props.address)">
        <EditIcon class="fill-primary "></EditIcon>
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>