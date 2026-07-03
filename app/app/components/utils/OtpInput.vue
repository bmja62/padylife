<template>
  <div class="flex items-center justify-between gap-2  " dir="ltr">
    <div class="bg-white rounded w-full gap-2 flex justify-between">
      <input
          v-for="i in optCount"
          :key="i"
          ref="otpInputRef"
          class="w-10 h-10 text-center rounded-xl  border border-gray-300 bg-transparent focus:outline-none"
          maxlength="1"
          type="tel"
          @mouseenter="counter=i"
          @keydown.tab="tabClicked(i)"
          @input="nextElement"
          @keydown.enter="exposeNumbersManually"
          @keydown.backspace="prevElement"
          @focus="handleFocus"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
const counter = ref(0);
const result = ref([]);
const otpInputRef = ref([]);

const route = useRoute();

const props = defineProps({
  optCount: {
    type: Number,
    default: 5,
  },
});

const emits = defineEmits<{
  (e: 'getNumber', joinedNumber: string): void;
  (e: 'applyFilters'): void;
}>();

defineExpose({
  makeJoinedNumbers,
});

onMounted(async () => {
});

function tabClicked(i) {
  counter.value = i
  if (otpInputRef.value[counter.value - 1]) {
    otpInputRef.value[i - 1].value = null
  }
}


function handleFocus(event: FocusEvent) {
  const input = event.target as HTMLInputElement;

  // Prevent selection and place the cursor at the end of the input's value
  input.select()
}

function makeJoinedNumbers() {
  result.value = [];
  for (let i = 0; i < counter.value; i++) {
    result.value.push(otpInputRef.value[i].value);
  }
  const newResult = result.value.join('');
  emits('getNumber', newResult);
}

function nextElement() {
  if (otpInputRef.value[counter.value]) {
    otpInputRef.value[counter.value].select();
    counter.value++;
  }
}

function exposeNumbersManually() {
  emits('applyFilters')
}

function prevElement() {
  counter.value--;
  if (otpInputRef.value[counter.value]) {
    otpInputRef.value[counter.value].select();
    emits('getNumber', []);
    result.value = [];
  }
}
</script>


<style scoped>
.single-input {
  width: 1rem;
  border-bottom: 1px solid black;
  margin: 0 0.3rem;
  background-color: transparent;
}

.edit-number-input {
  border: 1px solid rgb(0, 174, 255);
}

.otp-input::-webkit-input-placeholder {
  text-align: right;
}

.otp-input:-moz-placeholder {
  text-align: right;
}

.submit-button:disabled {
  cursor: initial;
}

.submit-button {
  transition: all 0.1s;
  width: 40%;
}

.single-input,
.single-input::-webkit-outer-spin-button,
.single-input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  -moz-appearance: textfield;
}
</style>
