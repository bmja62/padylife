<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IAddress} from "~/services/AddressService";

interface IProps {
  selectedAddress: IAddress
}
const props:IProps = defineProps({
  selectedAddress:{
    type:Object as PropType<IAddress>
  }
})
const isRendering = defineModel()
const emits = defineEmits<{
  (e: 'refetch'): void
}>()

const {$api} = useNuxtApp<IApiProvider>()

async function deleteAddress() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.address.deleteAddress(props.selectedAddress.id)
    if (response.isSuccess) {
      useAlerts().success('آدرس با موفقیت حذف شد')
      isRendering.value = false
      emits('refetch')

    } else {
      useAlerts().error(response.message)
    }
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

const authStore = useAuthStore()


</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>حذف آدرس</span>
    </template>
    <template #default>
      <div class="w-full  mb-2">
        <span>آیا از حذف این آدرس اطمینان دارید ؟ </span>
      </div>
      <div class="w-full flex items-center gap-2 p-2">
        <button @click="isRendering = false" type="button" class="btn w-1/2 bg-white border-primary !text-primary">
          خیر
        </button>
        <button type="button" @click="deleteAddress" class="btn w-1/2 bg-primary text-white">
          بله
        </button>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>