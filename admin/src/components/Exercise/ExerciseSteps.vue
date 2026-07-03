<script setup lang="ts">
import StepCountComponent from "@/components/Steps/StepCountComponent.vue";

const props = defineProps({
  stepsCount: Number as PropType<number>
})
const stepComponents = ref([])
const stepIds = ref([])

const emits = defineEmits<{
  (e: 'changeSlug', slugName: string): void
  (e: 'setStepIds', stepIds: []): void
}>()

function changeSlug() {
  emits('changeSlug', 'CreateExerciseForm')
}

function setSelectedStepIds(stepId: number) {
  stepIds.value.push(stepId)
  if (stepIds.value.length === stepComponents.value.length) {
    emits('setStepIds', stepIds.value)
  }
}

async function createExercise() {
  console.log(stepComponents.value)
  stepComponents.value.forEach((component) => {
    component.exposeStepId()
  })
}

</script>

<template>
  <VRow>
    <StepCountComponent @setSelectedId="setSelectedStepIds" v-for="stepCount in stepsCount" ref="stepComponents"
                        :stepCount="stepCount"></StepCountComponent>
    <VCol md="12" cols="12" class="d-flex justify-end gap-2">
      <VBtn
        @click="changeSlug"
        type="button"
        id="buy-now-btn"
        class="product-buy-now"
        color="secondary"
      >
        بازگشت
      </VBtn>
      <VBtn
        @click="createExercise"
        type="button"
        id="buy-now-btn"
        class="product-buy-now"
        color="green"
      >
        ثبت نهایی تمرین
      </VBtn>
    </VCol>
  </VRow>

</template>

<style scoped lang="scss">

</style>
