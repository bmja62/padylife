<script setup lang="ts">
const totalCount = defineModel('totalCount')
const limit = defineModel('limit')

interface IProps {
  capacity: number
}

const props: IProps = defineProps({
  capacity: {
    type: Number as PropType<number>
  }
})
</script>

<template>
  <div v-if="totalCount >=limit">
    <div v-if="totalCount !==limit"
         @click="limit = totalCount "
         class="absolute -bottom-5 left-1/3 w-24 h-8 p-2 badge pill bg-primary text-white rounded-[16px]">
      <span>مشاهده بیشتر</span>
    </div>
    <div v-else @click="limit = props.capacity"
         class="absolute -bottom-5 left-1/3 w-24 h-8 p-2 badge pill bg-primary text-white rounded-[16px]">
      <span>مشاهده کمتر</span>
    </div>
  </div>
</template>

<style scoped>

</style>