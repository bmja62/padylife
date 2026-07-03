<script setup lang="ts">
const selectedAnswer = ref<number | null>(null);

function selectAnswer(selectedItem: number) {
  selectedAnswer.value = selectedItem;
}
</script>

<template>
  <div class="w-full grid grid-cols-1 gap-y-5">
    <div
      class="w-full h-32 p-4 border-4 rounded-[20px] shadow-[0px_8px_16px_0px_rgba(75,52,37,0.05)]"
      :class="
        selectedAnswer == 1
          ? 'bg-[#ECEFFF]  border-[#00ABFB] [&_p]:!text-[#212121]'
          : 'bg-white border-white'
      "
      @click="selectAnswer(1)"
    >
      <p class="text-lg text-right text-black">بله</p>
      <p class="text-sm text-[#736B66] mt-1 text-justify">
        لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده
        از طراحان گرافیک است چاپگرها و متون بلکه روزنامه و مجله
      </p>
    </div>
    <div
      class="w-full h-32 p-4 border-4 rounded-[20px] shadow-[0px_8px_16px_0px_rgba(75,52,37,0.05)]"
      :class="
        selectedAnswer == 2
          ? 'bg-[#ECEFFF]  border-[#00ABFB] [&_p]:!text-[#212121]'
          : 'bg-white border-white'
      "
      @click="selectAnswer(2)"
    >
      <p class="text-lg text-right text-black">خیر</p>
      <p class="text-sm text-[#736B66] mt-1 text-justify">
        لورم ایپسوم متن ساختگی با تولید سادگی نامفهوم از صنعت چاپ و با استفاده
        از طراحان گرافیک است چاپگرها و متون بلکه روزنامه و مجله
      </p>
    </div>
  </div>
</template>
