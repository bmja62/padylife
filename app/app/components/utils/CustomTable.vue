<script setup lang="ts" generic="T">
interface IProps {
  tableHeaders: ITableHeaders<T>[];
  tableItems: ITableItems<T>[];
  pageNumber: number;
  count: number;
  totalCount: number;
}

interface IEmits {
  (e: "changePage", value: number): void;
}

const emits = defineEmits<IEmits>();

const props = defineProps<IProps>();

function changePage(pageNumber: number) {
  if (pageNumber != props.pageNumber) {
    emits("changePage", pageNumber);
  }
}
</script>

<template>
  <div class="w-full">
    <table class="table my-3 shadow-md">
      <thead>
        <tr class="bg-white rounded-t-2xl">
          <th
            v-for="column in props.tableHeaders"
            :key="column.key"
            class="first:rounded-tr-2xl last:rounded-tl-2xl"
          >
            {{ column.label }}
          </th>
        </tr>
      </thead>
      <tbody class="rounded-b-2xl">
        <template
          v-for="(row, rowIndex) in tableItems"
          :key="rowIndex * tableHeaders.length + 1"
        >
          <tr
            class="hover:bg-gray-200 bg-gray-100 last:rounded-b-2xl last:[&_td]:last-of-type:rounded-bl-2xl first:[&_td]:last-of-type:rounded-br-2xl"
          >
            <td
              v-for="(col, colIndex) in tableHeaders"
              :key="colIndex + rowIndex * rowIndex + 1"
            >
              <slot :name="col.key" :index="rowIndex" :item="row">
                {{
                  col.formatter // @ts-ignore-next-line
                    ? col.formatter(row[col.key], col.label, row) // @ts-ignore-next-line
                    : row[col.key]
                }}
              </slot>
            </td>
          </tr>
        </template>
      </tbody>
    </table>
    <div class="w-full flex items-center justify-center">
      <UtilsCustomPagination
        :page-number="props.pageNumber"
        :count="props.count"
        :total-count="props.totalCount"
        @change-page="changePage"
      />
    </div>
  </div>
</template>
