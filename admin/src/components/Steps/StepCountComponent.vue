<script setup lang="ts">
import {VForm} from "vuetify/components/VForm";
import {useAlerts} from "@/composables/alert";

const props = defineProps({
  stepCount: {
    type: Number as PropType<number>
  }
})
const alert = useAlerts()
defineExpose({
  exposeStepId
})
const emits = defineEmits<{
  (e: 'setSelectedId', stepId: number): void
}>()

async function exposeStepId() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
  emits('setSelectedId', selectedStepId.value)
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

const refVForm = ref(null)
const selectedStepId = ref(null)
</script>

<template>
  <VCol cols="12" md="12">
    <div class="py-2">
      <strong>مرحله {{ props.stepCount }}</strong>
    </div>
    <VForm
      ref="refVForm"
      @submit.prevent
    >
      <StepsPicker  required :key="stepCount" v-model="selectedStepId"></StepsPicker>
    </VForm>
  </VCol>
</template>

<style scoped lang="scss">

</style>
