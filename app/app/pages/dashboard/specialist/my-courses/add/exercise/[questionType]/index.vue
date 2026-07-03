<script setup lang="ts">
definePageMeta({
  layout: "dashboard",
  auth:true
});

const router = useRouter();
function goBack() {
  router.go(-1);
}

const questionTypes = ref<Record<number, string>>({
  1: "متن با پاسخ کوتاه",
  2: "چندگزینه‌ای",
  3: "متن با پاسخ بلند",
  4: "چندگزینه‌ای تصویری",
  5: "طیفی",
  6: "درجه‌بندی",
  7: "متن بدون پاسخ",
  8: "عکس بدون پاسخ",
});

const route = useRoute();

const doesHaveDescription = ref<boolean>(false);

function quitCreateCourse() {
  router.push("/dashboard/specialist/");
}

function saveCourse() {
  router.push("/dashboard/specialist/my-courses/");
}
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
            <p class="text-white text-sm">
              {{ questionTypes[+route.params.questionType!] }}
            </p>
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
        <p class="text-sm text-[#3D3D3D]">سوال</p>
        <textarea
          class="textarea textarea-bordered rounded-[4px] w-full focus:outline-none"
          placeholder="متن سوال"
        ></textarea>

        <div class="w-full flex items-center justify-between">
          <label for="description" class="text-[#3D3D3D] text-sm"
            >توضیحات</label
          >
          <input
            id="description"
            v-model="doesHaveDescription"
            type="checkbox"
            class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
          />
        </div>
        <textarea
          v-show="doesHaveDescription"
          class="textarea textarea-bordered rounded-[4px] w-full"
          placeholder="توضیحات"
        ></textarea>

        <div class="w-full grid grid-cols-2 gap-x-4 gap-y-3">
          <p class="text-sm col-span-2">محدودیت تعداد حروف پاسخ</p>
          <input
            type="number"
            inputmode="numeric"
            placeholder="حداقل"
            class="input w-full rounded-lg placeholder:text-sm focus:outline-none custom-input-shadow"
          />
          <input
            type="number"
            inputmode="numeric"
            placeholder="حداکثر"
            class="input w-full rounded-lg placeholder:text-sm focus:outline-none custom-input-shadow"
          />
        </div>
        <div class="w-full flex items-center justify-between mt-6">
          <label for="video" class="text-[#3D3D3D] text-sm">عکس یا ویدیو</label>
          <input
            id="video"
            type="checkbox"
            class="toggle border-none !bg-white checked:bg-white checked:[--tglbg:#00ABFB] [--tglbg:#D9D9D9]"
          />
        </div>
        <div class="w-full grid grid-cols-2">
          <label for="points" class="w-full flex items-center gap-x-2 text-sm">
            امتیاز مرحله

            <Icon name="icon:info-circle" size="19" />
          </label>
          <input
            id="points"
            type="number"
            inputmode="numeric"
            placeholder="امتیاز را وارد کنید"
            class="w-full input rounded-lg placeholder:text-sm focus:outline-none custom-input-shadow"
          />
        </div>
      </div>
      <div class="w-full grid grid-cols-2 gap-x-5 mt-20">
        <button
          type="button"
          class="w-full btn bg-white shadow-none border border-[#565E6D] !text-[#565E6D] bottom-0"
          @click="quitCreateCourse"
        >
          انصراف
        </button>
        <button
          type="button"
          class="w-full btn bg-[#00ABFB] bottom-0"
          @click="saveCourse"
        >
          ذخیره
        </button>
      </div>
    </UtilsWrappersPageWrapper>
  </div>
</template>

<style scoped>
.custom-input-shadow {
  box-shadow: 0px 1px 2px 0px rgba(0, 0, 0, 0.25);
}
</style>
