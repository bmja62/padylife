<script setup lang="ts">
import type {IExercise} from "~/services/ExerciseService";

const container = useTemplateRef("container");

interface IProps {
  circleItems: IExercise[];
}

const props: IProps = defineProps({
  circleItems: {
    type: Array as PropType<IExercise[]>
  }
})
const currentAngle = ref<number>(0);


onMounted(() => {
  nextTick(() => {
    // @ts-expect-error custom dirty plugin
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const propeller = new window.Propeller(container.value, {
      inertia: 0.7,
      angle: 0,
      stepTransitionEasing: "linear",
      onRotate: function () {
        currentAngle.value = this.angle;
      },
    });
  });
});

function calculatePolygonSize(circleItemCount: number): string {
  // Set the range of the bottom points (width of the triangle)
  const minWidth = 5; // Minimum width for larger item counts
  const maxWidth = 30; // Maximum width for smaller item counts

  // Calculate the width based on the item count
  const width = Math.max(
    minWidth,
    maxWidth - (circleItemCount - 1) * 3 // Adjust step based on preference
  );

  // Calculate left and right points
  const leftPoint = 50 - width;
  const rightPoint = 50 + width;

  // Return the clip-path polygon string
  return `polygon(50% 0%, ${leftPoint}% 100%, ${rightPoint}% 100%)`;
}

function calculateOuterRotation(idx: number) {
  return currentAngle.value - (360 / props.circleItems.length) * idx;
}

function calculateInnerRotation(idx: number) {
  console.log(currentAngle.value)

  return (360 / props.circleItems.length) * idx - currentAngle.value * 2;
}

const polygonClipPath = computed(() =>
    calculatePolygonSize(props.circleItems.length)
);
</script>

<template>
  <div class="w-full relative">
    <div
      ref="selectedPolygon"
      class="w-[335px] h-[335px] absolute top-0 -left-[130%] bg-transparent rounded-full border-b-4 border-black -rotate-90"
      :style="`clip-path: ${polygonClipPath};`"
    ></div>
    <div
      ref="container"
      class="w-[335px] h-[335px] absolute top-0 -left-[130%] border-2 rounded-full bg-transparent flex items-center justify-center"
    >
      <div
          v-for="(item, idx) in props.circleItems"
          :key="item.exerciseId"
        ref="circleItems"
        class="h-1/2 transform origin-bottom absolute top-0 flex flex-col justify-start items-center"
        :style="`rotate:${calculateOuterRotation(idx)}deg;`"
      >
        <div
          class="rounded-full p-4 transition-all !duration-[10ms]"
          :style="`rotate:${calculateInnerRotation(idx)}deg`"
        >
          <div class="flex items-center gap-2 pr-10">
            <span>{{ idx + 1 }}</span>
            <span>
          {{ item.title }}
          </span>
          </div>
        </div>
      </div>

      <div class="w-[134px] h-[134px] rounded-full bg-[#D9D9D9]"></div>
    </div>
  </div>
</template>
