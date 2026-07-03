<script setup lang="ts">

import type {IExercise, IExerciseDetail} from "~/services/ExerciseService";

interface IProps {
  circleItems: IExercise[]
  currentExercise: IExerciseDetail
}

const props:IProps = defineProps({
  circleItems: {
    type: Array as PropType<IExercise[]>
  },
  currentExercise: {
    type: Object as PropType<IExerciseDetail>
  }
})

const emits = defineEmits<{
  (e: 'setSelectedItem', exercise: IExercise): void
}>()
const rotateDeg = ref(0);
const halfCircle = useTemplateRef('halfCircle')
const {direction} = useSwipe(halfCircle)
const {pressed,sourceType} = useMousePressed({touch: false, target: halfCircle,drag:true,capture:true,onPressed:onMousePressed})
watch(() => direction.value, async (val) => {
  // if (val === 'up') {
  //   rotateDeg.value -= 81
  // } else
    if (val === 'down') {
    rotateDeg.value += 81
  }
})
function onMousePressed(event){
  if(event.target === halfCircle.value){
     rotateDeg.value += 81
  }
}

function calculateInnerRotation(idx: number) {
  return (360 / props.circleItems.length) * idx + rotateDeg.value;
}

function setSelectedExercise(item: IExercise) {
  emits('setSelectedItem', item)
}

</script>

<template>

  <div class="w-max h-max transform  relative translate-x-[-40px] translate-y-[-10px]">
    <!--    <div-->
    <!--        ref="selectedPolygon"-->
    <!--        class="w-[335px] h-[335px] absolute top-0  bg-transparent rounded-full border-b-4 border-[#00ABFB] rotate-[-103deg]"-->
    <!--        :style="`clip-path:polygon(50% 0%, 26% 100%,50% 100%);`"-->
    <!--    ></div>-->
    <div ref="halfCircle" :style="`rotate: ${rotateDeg}deg`"
         class="w-[335px] transition-all h-[335px] border border-[#51E8F166] rounded-full relative flex flex-row justify-center items-center z-0">
      <div v-for="(item,idx) in props.circleItems"
           @click.stop
           class="h-1/2 transform !z-[999] origin-bottom absolute top-0 flex flex-col justify-start items-center  translate-y-[40px]"
           :style="`rotate:${(360/props.circleItems.length)*idx}deg;`">
        <div :class="{'text-[#00ABFB]':props.currentExercise?.id === item.id}"
             @click="setSelectedExercise(item)"
             :style="`transform: rotate(-${calculateInnerRotation(idx)}deg)`">
          <div class="flex items-center gap-1">
            <strong class="!text-[10px]">
              {{ idx + 1 }}
            </strong>
            <strong class="text-[10px]">{{ item.title }}</strong>
            <Icon name="icon:check" size="9" color="white"/>
          </div>
        </div>
      </div>
      <div class="w-[134px] h-[134px] rounded-full bg-primary"></div>
    </div>

  </div>


</template>

<style scoped>



</style>