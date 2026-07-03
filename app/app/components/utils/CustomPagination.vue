<script setup lang="ts">
interface IProps {
  pageNumber: number;
  count: number;
  totalCount: number;
}

interface IEmits {
  (e: "changePage", value: number): void;
}

const emits = defineEmits<IEmits>();

const props = defineProps<IProps>();

const totalPages = computed(() => Math.ceil(props.totalCount / props.count));

const pagination = computed(() => {
  const pages: (number | string)[] = [];

  // Always include the first page
  pages.push(1);

  if (props.pageNumber > 3) pages.push("...");

  for (
    let i = Math.max(2, props.pageNumber - 1);
    i <= Math.min(totalPages.value - 1, props.pageNumber + 1);
    i++
  ) {
    pages.push(i);
  }

  if (props.pageNumber < totalPages.value - 2) pages.push("...");

  // Always include the last page
  if (totalPages.value > 1) pages.push(totalPages.value);

  return pages;
});

const goToPage = (page: number | string) => {
  if (page !== "...") {
    emits("changePage", page as number);
  }
};
</script>

<template>
  <div class="flex items-center gap-1">
    <button
      v-for="page in pagination"
      :key="page"
      :class="[
        'bg-gray-300 min-w-12',
        'btn',
        page === '...' ? 'bg-gray-500' : '',
        page === props.pageNumber ? '!bg-primary' : '',
      ]"
      @click="goToPage(page)"
    >
      {{ page }}
    </button>
  </div>
</template>
