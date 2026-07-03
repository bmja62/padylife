<script setup lang="ts">
const showOverlay = defineModel()
const props = defineProps({
  progressAmount: {
    type: Number as PropType<number>
  }
})
</script>

<template>
  <v-overlay class="align-center justify-center" v-model="showOverlay">
    <v-progress-circular
      :model-value="progressAmount"
      :size="128"
      :width="8"
      color="blue"
    >
      <span class="text-white">{{ progressAmount }}%</span>
    </v-progress-circular>
  </v-overlay>
</template>

<style scoped lang="scss">

</style>
