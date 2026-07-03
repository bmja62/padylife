<template>
  <div class="w-full flex flex-col gap-y-4">
    <div
      class="w-full flex flex-row justify-start items-center gap-x-2 overflow-x-auto pb-2"
    >
      <div
        class="min-w-8 w-8 h-8 border border-gray-300 flex flex-row justify-center items-center rounded-full"
      >
        <Icon name="icon:search-v1" color="#01CED1" size="15" />
      </div>
      <div class="h-full border-l border-gray-300"></div>
      <div
        v-for="item in searchOptions"
        :key="item"
        class="min-w-max flex flex-row justify-center items-center rounded-full border border-gray-300 py-1 px-2"
      >
        <span class="text-gray-500 text-sm">{{ item }}</span>
      </div>
    </div>

    <div class="w-full flex flex-col gap-y-4">
      <div v-for="i in 3" :key="i" class="w-full h-[140px] relative">
        <div
          class="w-full h-32 custom-card-shadow rounded-2xl bg-white z-[2] absolute top-0 right-0 p-4"
        >
          <slot>
            <div class="w-full flex flex-col items-center gap-y-2">
              <div class="w-full flex flex-row justify-between items-center">
                <div class="flex flex-col items-start justify-between gap-y-2">
                  <span class="text-sm text-gray-800">ذهن آگاهی</span>
                  <span class="text-sm text-gray-800"
                    >100 هزار تومان تخفیف</span
                  >
                </div>
                <div
                  class="w-[7rem] h-[4rem] bg-[#D9D9D9] flex flex-row justify-center items-center rounded-lg"
                >
                  <Icon name="icon:no-picture" size="15" />
                </div>
              </div>
              <div
                class="w-full flex flex-row justify-between items-center border-t border-dashed border-primary py-2"
              >
                <span class="text-sm text-gray-500">20 امتیاز</span>
                <span class="text-sm text-gray-500">مشاهده جزئیات</span>
              </div>
            </div>
          </slot>
        </div>
        <div
          class="w-[calc(100%+6px)] h-32 custom-card-shadow rounded-2xl z-[1] bg-[#999999] absolute top-2 right-0"
        ></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Variables
const searchOptions = shallowRef([
  "همه",
  "فروشگاه",
  "مدیتیشن",
  "ذهن آگاهی",
  "لورم ایپسوم",
]);
</script>

<style scoped></style>
