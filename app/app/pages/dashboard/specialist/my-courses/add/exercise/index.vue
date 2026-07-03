<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});

const router = useRouter();
function goBack() {
  router.go(-1);
}

interface IQuestionType {
  title: string;
  id: number;
}

const selectedType = ref<number | null>(null);

function selectItem(selectedItem: number) {
  selectedType.value = selectedItem;
}

function quitCreateCourse() {
  router.push("/dashboard/specialist/");
}

function goToQuestionType() {
  router.push(
    `/dashboard/specialist/my-courses/add/exercise/${selectedType.value}`
  );
}

const questionTypes = ref<IQuestionType[]>([
  {
    title: "متن با پاسخ کوتاه",
    id: 1,
  },
  {
    title: "چندگزینه‌ای",
    id: 2,
  },
  {
    title: "متن با پاسخ بلند",
    id: 3,
  },
  {
    title: "چندگزینه‌ای تصویری",
    id: 4,
  },
  {
    title: "طیفی",
    id: 5,
  },
  {
    title: "درجه‌بندی",
    id: 6,
  },
  {
    title: "متن بدون پاسخ",
    id: 7,
  },
  {
    title: "عکس بدون پاسخ",
    id: 8,
  },
]);
</script>

<template>
  <div class="w-full h-full custom-pattern-bg-image">
    <UtilsWrappersPageWrapper
      style="--wrapper-header-height: 80px"
      inner-class="!px-10"
    >
      <template #header>
        <BaseSpecialistHeader @go-back="goBack">
          <template #title>
            <p class="text-white text-sm">افزودن تمرین</p>
          </template>
        </BaseSpecialistHeader>
      </template>
      <div class="w-full tabs-list flex items-center justify-start gap-x-9">
        <button type="button" class="text-[#00ABFB] text-sm font-medium">
          ایجاد
        </button>
        <button type="button" class="text-sm font-medium">پیش نمایش</button>
      </div>

      <hr class="my-4 border-[#D2D2D2]" />
      <div class="w-full flex flex-col gap-y-5">
        <p class="text-sm text-[#3D3D3D]">نوع تمرین را انتخاب کنید.</p>
        <div class="w-full grid grid-cols-2 gap-x-4 gap-y-2">
          <button
            v-for="type in questionTypes"
            :key="type.id"
            type="button"
            class="w-full bg-white border rounded-lg flex items-center p-4 text-[#3D3D3D] custom-button-box-shadow"
            :class="
              selectedType == type.id ? 'border-[#00ABFB]' : 'border-white'
            "
            @click="selectItem(type.id)"
          >
            <p class="text-xs">{{ type.title }}</p>
          </button>
        </div>
      </div>
      <div class="w-full grid grid-cols-2 gap-x-5 mt-24">
        <button
          type="button"
          class="w-full btn bg-white shadow-none border border-[#565E6D] !text-[#565E6D] bottom-0"
          @click="quitCreateCourse"
        >
          انصراف
        </button>
        <button
        :disabled="!selectedType"
          type="button"
          class="w-full btn bg-[#00ABFB] bottom-0"
          @click="goToQuestionType"
        >
          بعدی
        </button>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-button-box-shadow {
  box-shadow: 0px 0px 2px 0px rgba(0, 0, 0, 0.25);
}
</style>
