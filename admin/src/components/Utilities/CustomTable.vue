<script setup lang="ts">
// eslint-disable-next-line no-restricted-imports
import {VDataTable} from 'vuetify/components'
import {storeToRefs} from 'pinia'
import type {ITableHeaders} from '@/models/ITableHeader'
import {useSpinnerStore} from '@/stores/spinner'

// Variables
const props = withDefaults(defineProps<IProps>(), {
  tableHeaders: () => [],
  itemsList: () => [],
  pageNumber: 1,
  count: 10,
  totalCount: null,
  isEditing: false,
})

const emit = defineEmits<IEmits>()

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
const spinnerStore = useSpinnerStore()
const {isRenderingSpinner} = storeToRefs(spinnerStore)
const formRef = ref<HTMLFormElement[]>([])

// Exposes
defineExpose({
  formRef,
})

// Functions
function changePage(pageNumber: string | number) {
  emit('changePage', pageNumber)
}

function priceHandler(isFormatting: boolean, idx: number, key: string, val: string) {
  if (isFormatting) {
    if (props.itemsList && props.itemsList.length > 0)
      // eslint-disable-next-line vue/no-mutating-props
      props.itemsList[idx][key] = val.replaceAll(',', '').replace(/\B(?=(\d{3})+(?!\d))/g, ',')
    if (val === '' && props.itemsList && props.itemsList.length > 0)
      // eslint-disable-next-line vue/no-mutating-props
      props.itemsList[idx][key] = null
  }
}
</script>

<template>
  <VDataTable
    hide-default-footer
    hover
    :headers="props.tableHeaders as VDataTable['headers']"
    :items="props.itemsList || []"
    :loading="isRenderingSpinner"
    :items-per-page="props.count"
    class="border rounded-lg"
    item-key="id"
    no-data-text="هیچ اطلاعاتی وجود ندارد!"
  >
    <template
      v-if="!props.itemsList && !props.totalCount"
      #loading
    >
      <VSkeletonLoader type="table-row@10"/>
    </template>

    <template #item="{ index, internalItem: { raw, columns } } ">
      <tr>
        <td
          v-for="(header, idx) in props.tableHeaders"
          :key="idx"
        >
          <template v-if="!$slots[header.key as string]">
            <template v-if="!header.editable || header.editable && !props.isEditing">
              {{ columns[header.key as string] }}
            </template>
            <template v-else-if="header.editable && props.isEditing">
              <!-- eslint-disable vue/no-mutating-props -->
              <VForm
                ref="formRef"
                @submit.prevent
              >
                <VTextField
                  v-if="itemsList && itemsList.length > 0"
                  v-model="itemsList[index][header.key]"
                  :rules="[header.rules]"
                  variant="outlined"
                  density="compact"
                  :label="header.title"
                  color="success"
                  hide-details
                  @input="priceHandler(header.formattedPrice, index, header.key, itemsList[index][header.key])"
                />
              </VForm>
            </template>
          </template>
          <template v-else>
            <slot
              :name="header.key"
              :item="raw"
            />
          </template>
        </td>
      </tr>
    </template>
    <template #bottom>
      <div
        style="overflow-wrap: anywhere;"
        v-if="props.totalCount"
        class="text-center pb-4"
      >
        <VPagination
          :length="Math.ceil(+props.totalCount / +props.count)"
          :total-visible="props.count"
          @update:model-value="changePage"
        />
      </div>
    </template>
  </VDataTable>
</template>
