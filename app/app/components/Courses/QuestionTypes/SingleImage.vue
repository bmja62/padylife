<script setup lang="ts">
import type {IImageOption} from "~/services/StepService";

interface IProps {
  stepOption: IImageOption | null;
}
const isRenderingPreviewDialog = ref(false)
const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});

</script>

<template>
  <div class="w-full my-6">
    <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>
    <div
        class="w-full h-[200px] rounded-[32px] bg-white border border-[#00ABFB] flex items-center justify-center"
    >
      <img @click="isRenderingPreviewDialog = true" :src="stepOption?.imageUrl" :alt="stepOption?.altText" class="w-full h-full object-contain rounded-[32px]"/>
    </div>
    <div class=divider></div>
      <p style="overflow-wrap: anywhere">
        {{stepOption.caption}}
      </p>
  <LazyUtilsDialogsPreviewImageDialog :media="stepOption?.imageUrl" v-model="isRenderingPreviewDialog"></LazyUtilsDialogsPreviewImageDialog>
  </div>
</template>

<!-- Keep the existing styles as they are correct -->
<style scoped>

</style>