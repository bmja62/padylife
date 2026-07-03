<script setup lang="ts">

import {useSpinner} from "@/composables/spinner";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";
import {IProduct} from "@/services/ProductService";
import type {VDataTable} from "vuetify/components";
import {IInventoryStock, IWarehouseStock} from "@/services/WarehouseService";
import DecreaseWarehouseStockDialog from "@/components/Warehouses/DecreaseWarehouseStockDialog.vue";
import TransferWarehouseStockDialog from "@/components/Warehouses/TransferWarehouseStockDialog.vue";

onMounted(async () => {
  await getProductInfo()
  await getProductInventory()
})
const isRenderingCreateStockDialog = ref(false)
const isRenderingDecreaseWarehouseStock = ref(false)
const isRenderingTransferWarehouseStock = ref(false)
const productInfo = ref<IProduct>(null)
const warehousesStock = ref<IInventoryStock>(null)
const selectedWarehouseStock = ref<IWarehouseStock>(null)
const route = useRoute()
const $api = inject<IApiProvider>('$api')
const tableHeaders: VDataTable['headers'] = ref(
  [
    {title: 'نام انبار', key: 'warehouseName'},
    {title: 'نام منطقه انبار', key: 'zoneName'},
    {title: 'موجودی کل', key: 'quantity'},
    // {title: 'موجودی رزرو', key: 'reservedQuantity'},
    // {title: 'موجودی آزاد', key: 'availableQuantity'},
    {title: 'حداقل موجودی', key: 'minimumStock'},
    {
      title: 'تاریخ آخرین بروزرسانی', key: 'lastStockUpdate',
      value: (item: IWarehouseStock) => new Date(item.lastStockUpdate).toLocaleDateString('fa-IR')
    },
    {title: 'عملیات', key: 'actions'},
  ]
)

function renderDecreaseStockDialog(item: IWarehouseStock) {
  selectedWarehouseStock.value = item
  isRenderingDecreaseWarehouseStock.value = true
}


function renderTransferWarehouseStock(item: IWarehouseStock) {
  selectedWarehouseStock.value = item
  isRenderingTransferWarehouseStock.value = true
}

async function getProductInfo() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.getProductById(route.params.productId)
    productInfo.value = data.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function getProductInventory() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.warehouse.getProductInventory(productInfo.value.id, route.params.variantId)
    warehousesStock.value = data.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

const getProductVariant = computed(() => {
  if (productInfo?.value?.variants?.length) {
    const idx = productInfo?.value?.variants.findIndex(e => e.id === +route.params.variantId)
    if(idx>-1){
      return productInfo.value.variants[idx]
    }
  }
})
</script>

<template>
  <PageWrapper
    v-if="warehousesStock"
  >
    <template #title>
      مدیریت موجودی {{ productInfo.name }}
    </template>
    <template #append>
      SKU :  {{ getProductVariant?.sku }}
    </template>
    <VRow>
      <VCol cols="12" md="3">
        <v-card

          class="mx-auto"
          prepend-icon="mdi-database"
          :subtitle="warehousesStock.summery.totalQuantity"
        >
          <template v-slot:title>
            <span class="font-weight-black">موجودی کل</span>
          </template>
        </v-card>
      </VCol>
      <VCol cols="12" md="3">
        <v-card
          class="mx-auto"
          prepend-icon="mdi-arrow-up-bold-box-outline"
          :subtitle="warehousesStock.summery.totalAvailable"
        >
          <template v-slot:title>
            <span class="font-weight-black">موجودی آزاد</span>
          </template>

        </v-card>
      </VCol>
      <VCol cols="12" md="3">
        <v-card
          class="mx-auto"
          prepend-icon="mdi-lock-reset"
          :subtitle="warehousesStock.summery.totalReserved"
        >
          <template v-slot:title>
            <span class="font-weight-black">موجودی رزرو</span>
          </template>
        </v-card>
      </VCol>
      <VCol cols="12" md="3">
        <v-card
          class="mx-auto"
          prepend-icon="mdi-mailbox"
          :subtitle="warehousesStock.summery.warehouseCount"
        >
          <template v-slot:title>
            <span class="font-weight-black">تعداد انبارها</span>
          </template>
        </v-card>
      </VCol>
      <VCol cols="12" class="d-flex flex-md-row flex-column gap-2 align-items-center justify-space-between">
        <h3>موجودی کالا در انبارها</h3>
        <VBtn
          color="success"
          variant="flat"
          @click="isRenderingCreateStockDialog  = true"
        >
          افزودن موجودی جدید
        </VBtn>
      </VCol>
      <VCol cols="12">
        <CustomTable
          :items-list="warehousesStock.data"
          :count="100"
          :page-number="1"
          :table-headers="tableHeaders"
          :total-count="100"
        >
          <template #actions="data">
            <VBtn
              color="transparent"
              elevation="0"
              @click="renderDecreaseStockDialog(data.item as IWarehouseStock)"
            >
              <VIcon color="error" icon="mdi-decimal-decrease"></VIcon>
              <VTooltip
                activator="parent"
              >
                کاهش موجودی
              </VTooltip>
            </VBtn>
            <VBtn
              color="transparent"
              elevation="0"
              @click="renderTransferWarehouseStock(data.item as IWarehouseStock)"
            >
              <VIcon color="info" icon="mdi-swap-horizontal"></VIcon>
              <VTooltip
                activator="parent"
              >
                حواله موجودی
              </VTooltip>
            </VBtn>
            <!--            <VBtn-->
            <!--              color="transparent"-->
            <!--              elevation="0"-->
            <!--              @click="renderDecreaseStockDialog(data.item as IWarehouseStock)"-->
            <!--            >-->
            <!--              <VIcon color="secondary" icon="mdi-calendar-clock"></VIcon>-->
            <!--              <VTooltip-->
            <!--                activator="parent"-->
            <!--              >-->
            <!--                رزرو موجودی-->
            <!--              </VTooltip>-->
            <!--            </VBtn>-->
            <!--            <VBtn-->
            <!--              color="transparent"-->
            <!--              elevation="0"-->
            <!--              @click="renderDecreaseStockDialog(data.item as IWarehouseStock)"-->
            <!--            >-->
            <!--              <VIcon color="success" icon="mdi-calendar-multiple-check"></VIcon>-->
            <!--              <VTooltip-->
            <!--                activator="parent"-->
            <!--              >-->
            <!--                آزادسازی موجودی رزرو-->
            <!--              </VTooltip>-->
            <!--            </VBtn>-->
          </template>

        </CustomTable>

      </VCol>
    </VRow>
    <CreateWarehouseStockDialog
      v-model:dialogState="isRenderingCreateStockDialog"
      @refetch="getProductInventory"
    ></CreateWarehouseStockDialog>
    <DecreaseWarehouseStockDialog
      :selectedItem="selectedWarehouseStock"
      v-model:dialogState="isRenderingDecreaseWarehouseStock"
      @refetch="getProductInventory"
    ></DecreaseWarehouseStockDialog>
    <TransferWarehouseStockDialog
      :selectedItem="selectedWarehouseStock"
      v-model:dialogState="isRenderingTransferWarehouseStock"
      @refetch="getProductInventory"
    ></TransferWarehouseStockDialog>
  </PageWrapper>


</template>

<style scoped lang="scss">

</style>
