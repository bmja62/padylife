<script setup lang="ts">
const isRendering = defineModel()
interface IProps {
  media:string
}
const props:IProps = defineProps({
  media:{
    type:String as PropType<string>
  }
})
</script>

<template>
  <LazyUtilsDialogsBaseDialog  dialog-id="previewImage" v-model="isRendering">
    <template #title>
      <span>مشاهده</span>
    </template>
    <template #default>
      <img :src="props.media"  alt="">
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>