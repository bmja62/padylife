<script setup lang="ts">
import type {ITableHeaders} from "@/models/ITableHeader";

const props = defineProps({
  transactionsFilters: {},
  transactions: []
})
const tableHeaders: ITableHeaders = [
  {title: 'مقدار', key: 'amount',  value: (item: any) => `${Intl.NumberFormat('fa-IR').format(item.amount)}  تومان `},
  {title: 'تاریخ ', key: 'createDate', value: (item: any) => new Date(item.createDate).toLocaleDateString('fa-IR'),},
  {title: 'وضعیت', key: 'isPayed', },

]

</script>

<template>
  <VCard title="تراکنش های اخیر">


    <VDivider/>
    <CustomTable
      :count="10"
      :items-list="transactions"
      :page-number="1"
      :table-headers="tableHeaders"
      :total-count="10"
    >
      <template #isPayed="data">
        <VChip color="success" v-if="data.item.isPayed">موفق</VChip>
        <VChip color="error" v-else>ناموفق</VChip>

      </template>
    </CustomTable>
  </VCard>
</template>

<style lang="scss" scoped>
.v-table {
  tbody {
    tr:not(:last-child) {
      td {
        border: none !important;
      }
    }
  }
}
</style>
