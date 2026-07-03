<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IAddress, IGetAddressesParams} from "~/services/AddressService";

definePageMeta({
  auth: true
})

definePageMeta({
  layout: "dashboard",
});
useHead({
  title: 'آدرس‌ها'
})
const {$api} = useNuxtApp<IApiProvider>()
const authStore = useAuthStore()
const addressesListFilters = ref<IGetAddressesParams>({
  pageNumber: 1,
  count: 10,
  search: '',
  userId: authStore.getUser.id
})
const addressesList = ref<IAddress[]>([])
const debounceTimeout = ref(null)
const totalCount = ref(null)
const isRenderingCreateDialog = ref(false)
const isRenderingDeleteDialog = ref(false)
const isRenderingUpdateDialog = ref(false)
const selectedAddress = ref<IAddress>(null)
const debouncedSearch = computed({
  get() {
    return addressesListFilters.value.search;
  },
  set(val) {
    if (debounceTimeout.value) {
      clearTimeout(debounceTimeout.value);
    }
    debounceTimeout.value = setTimeout(() => {
      addressesListFilters.value.search = val
      addressesListFilters.value.pageNumber = 1
      getAllAddresses()
    }, 600);
  },
});

async function getAllAddresses() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.address.getUserAddresses(addressesListFilters.value)
    addressesList.value = response.data.data
    totalCount.value = response.data.totalCount
  } catch (error) {
    console.error(error.message);
  } finally {
    useSpinner().hideSpinner()
  }
}

getAllAddresses()

function changePage(page: number) {
  addressesListFilters.value.pageNumber = page
  getAllAddresses()
}

function openUpdateDialog(address: IAddress) {
  selectedAddress.value = JSON.parse(JSON.stringify(address))
  isRenderingUpdateDialog.value = true
}

function openDeleteDialog(address: IAddress) {
  selectedAddress.value = JSON.parse(JSON.stringify(address))
  isRenderingDeleteDialog.value = true
}
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper style="--wrapper-header-height: 80px">
      <template #header>
        <BaseNotificationHeader>
          <template #title>آدرس‌ها</template>
        </BaseNotificationHeader>
      </template>
      <div
          class="w-full h-full bg-[#F7F8FE] rounded-t-[32px] flex flex-col justify-start items-start pt-5 gap-y-4"
      >
        <div class="w-full grid grid-cols-12 gap-2">
          <input
              v-model="debouncedSearch"
              class="col-span-10 bg-[#ECEFFF] rounded-[8px] border border-[#E0E4E8] text-xs text-gray-700 placeholder:text-[#6F6F6F] px-4 py-2"
              placeholder="جستجو در آدرس‌ها"
          />
          <div class="col-span-2 flex flex-row justify-start gap-2">
            <div
                @click="isRenderingCreateDialog = true"
                class="flex flex-row justify-center items-center rounded-[8px] border border-[#E0E4E8] p-2 bg-primary "
            >
              <Icon
                  name="icon:plus"
                  color="#000000"
                  class="[&_*]:stroke-white [&_*]:fill-white transform transition-all"
                  size="20"
              />
            </div>

          </div>
        </div>

        <div class="w-full flex flex-col gap-y-4">
          <template v-if="addressesList.length">
            <LazyDashboardAddressCard
                v-for="(address, index) in addressesList"
                :key="index"
                :address="address"
                @openUpdateDialog="openUpdateDialog"
                @openDeleteDialog="openDeleteDialog"
            >
            </LazyDashboardAddressCard>
            <div class="w-full flex items-center justify-center">
              <UtilsCustomPagination
                  :page-number="addressesListFilters.pageNumber"
                  :count="addressesListFilters.count"
                  :total-count="totalCount"
                  @change-page="changePage"
              />
            </div>
          </template>
          <span v-else>  آدرسی یافت نشد</span>
        </div>
      </div>
    </UtilsWrappersPageWrapper>
    <LazyUtilsDialogsCreateAddressDialog @refetch="getAllAddresses"
                                         v-model="isRenderingCreateDialog"></LazyUtilsDialogsCreateAddressDialog>
    <LazyUtilsDialogsDeleteAddressDialog @refetch="getAllAddresses"
                                         :selectedAddress="selectedAddress"
                                         v-model="isRenderingDeleteDialog"></LazyUtilsDialogsDeleteAddressDialog>
    <LazyUtilsDialogsUpdateAddressDialog @refetch="getAllAddresses"
                                         :selectedAddress="selectedAddress"
                                         v-model="isRenderingUpdateDialog"></LazyUtilsDialogsUpdateAddressDialog>
  </div>
</template>

<style scoped></style>
