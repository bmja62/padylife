<script setup lang="ts">
import type {IChoice, IMultiChoiceOption} from "~/services/StepService";

const emits = defineEmits<{
  (e: 'setSelectedAnswers', payload: number[] | number): void
}>()
interface IProps {
  stepOption: IMultiChoiceOption | null
}

const props = withDefaults(defineProps<IProps>(), {
  stepOption: null,
});
const selectedAnswer = ref<IChoice>(null);
const selectedAnswers = ref<IChoice[]>([]);

function setSelectedAnswer(selectedItem: IChoice) {
  if (props.stepOption?.allowMultipleSelection) {
    const idx = selectedAnswers.value.findIndex(e => e.id === selectedItem.id)
    if (idx > -1) {
      selectedAnswers.value.splice(idx, 1);
    } else {
      selectedAnswers.value.push(selectedItem)
    }
  } else {
    selectedAnswer.value = selectedItem;
  }
  generatePayload()
}

function generatePayload() {
  if (props.stepOption?.allowMultipleSelection) {
    emits('setSelectedAnswers', selectedAnswers.value.map((answer) => {
      return answer.id
    }))


  } else {
    emits('setSelectedAnswers', selectedAnswer.value.id)
  }
}
function checkPresenceOfSelectedOption(item: IChoice) {
  if (props.stepOption?.allowMultipleSelection) {
    const idx = selectedAnswers.value.findIndex(e => e.id === item.id)
    if (idx > -1) {
      return true
    }
  } else {
    return selectedAnswer.value.id = item.id;
  }
  return false
}
</script>

<template>
  <div v-if="stepOption" class="w-full h-auto my-4">
    <h3 class="text-lg text-black my-4">{{ stepOption.title }}</h3>
    <div class="flex flex-col gap-y-4">
      <div
          v-for="item in stepOption?.choices"
          :key="item.id"
        class="w-full rounded-full p-4 border"
        :class="
        checkPresenceOfSelectedOption(item)
            ? 'bg-[#ECEFFF] border-[#00ABFB] text-[#212121]'
            : 'custom-text-shadow bg-white border-white'
        "
          @click="setSelectedAnswer(item)"
      >
        <p>{{ item.text }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.custom-text-shadow {
  box-shadow: 0px 8px 16px 0px rgba(75, 52, 37, 0.05);
}
.custom-selected-text-shadow {
  box-shadow: 0px 0px 0px 4px rgba(160, 214, 32, 0.25);
}
</style>
