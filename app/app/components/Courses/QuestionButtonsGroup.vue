<script setup lang="ts">
interface IEmits {
  (e: "next"): void;
}

const emits = defineEmits<IEmits>();

function next() {
  emits("next");
}
const router = useRouter()
function prev() {
  router.go(-1);

}
</script>

<template>
  <div class="w-full grid grid-cols-2 gap-x-5">
    <button
      type="button"
      class="w-full btn bg-white shadow-none border border-[#565E6D] !text-[#565E6D] bottom-0 !rounded-[28px]"
      @click="prev"
    >
      قبلی
    </button>
    <button
      type="button"
      class="w-full btn btn-primary bottom-0 !rounded-[28px]"
      @click="next"
    >
      بعدی
    </button>
  </div>
</template>
