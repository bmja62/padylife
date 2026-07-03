<script setup lang="ts">
import type { ITableHeaders } from '@/models/ITableHeader'
import CustomTable from '@/components/Utilities/CustomTable.vue'

// Interfaces
interface IProps {
  tableHeaders: ITableHeaders
  itemsList: any[] | null
  pageNumber: string | number
  count: string | number
  totalCount: string | number | null
  isEditing?: boolean
}
interface IEmits {
  (e: 'changePage', value: number | string): void
}

// Variables
const props = defineProps<IProps>()
const emit = defineEmits<IEmits>()

const tableRef = ref<InstanceType<typeof CustomTable>>()

// Functions
function changePage(pageNumber: string | number) {
  emit('changePage', pageNumber)
}

// Exposes
defineExpose({
  tableRef,
})
</script>

<template>
  <VRow>
    <VCol
      v-if="props.itemsList && props.tableHeaders"
      cols="12"
    >
      <VToolbar
        v-if="$slots.header"
        flat
        height="auto"
        class="rounded-t-lg has-header"
      >
        <slot name="header" />
      </VToolbar>
      <CustomTable
        ref="tableRef"
        v-bind="$props"
        @change-page="changePage"
      >
        <template
          v-for="(_, slot) in $slots"
          #[slot]="data"
        >
          <slot
            :name="slot"
            :item="data.item"
          />
        </template>
      </CustomTable>
      <VToolbar
        v-if="$slots.footer"
        flat
        height="auto"
        class="rounded-b-lg has-footer"
      >
        <slot name="footer" />
      </VToolbar>
    </VCol>
  </VRow>
</template>

<style scoped>
.has-header + .v-data-table {
  border-block-start: none !important;
  border-start-end-radius: 0 !important;
  border-start-start-radius: 0 !important;
}

.v-data-table:has(+ .has-footer) {
  border-block-end: none !important;
  border-end-end-radius: 0 !important;
  border-end-start-radius: 0 !important;
}
</style>
