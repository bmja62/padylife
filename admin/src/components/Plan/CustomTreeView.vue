<script setup lang="ts">
import {InternalPlanQuestionOptionActions} from "@/models/Enums/InternalPlanQuestionOptionActions";
import {IPlanQuestionOption} from "@/services/PlanService";

const strategy = shallowRef('leaf')
const selected = ref([])
const props = defineProps({
  items: {
    type: Array as PropType<object>
  }
})
const emits = defineEmits<{
  (e: 'getSelectedProperty', item: IPlanQuestionOption, action: InternalPlanQuestionOptionActions): void
}>()

function getSelectedProperty(item: IPlanQuestionOption, action: InternalPlanQuestionOptionActions) {
  console.log(item)
  emits('getSelectedProperty', item, action)
}

</script>

<template>
  <div>

    <v-treeview
      :items="props.items"
      :select-strategy="strategy"
      item-value="id"
      return-object
    >
      <template v-slot:prepend="{ item, isOpen }">
        <v-icon v-if="item.linkedPlanQuestionId ||item.isMain || item.linkedPlanQuestionId===undefined"
                :icon="'mdi-comment-question-outline'"></v-icon>
        <v-icon v-if="item?.linkedExercises?.length" :icon="'mdi-lead-pencil'"></v-icon>
      </template>
      <template v-slot:title="{ item }">
        <div class="d-flex w-100 align-content-end justify-space-between">
          <div class="d-flex align-content-center justify-space-center flex-wrap">
            <div v-html="item.text"></div>
          </div>
          <div
               class="d-flex align-content-center gap-2">
            <template  v-if="!item.isMain &&(item.linkedPlanQuestionId!==undefined &&item.linkedExercises!==undefined ) ">
            <VBtn
              @click="getSelectedProperty(item,1)"
              v-if="(!item.linkedPlanQuestionId && !item?.linkedExercises?.length) ||(item.linkedPlanQuestionId && item?.linkedExercises?.length) "
              color="primary"
            >
              اتصال به سوال
            </VBtn>
            <VBtn
              @click="getSelectedProperty(item,2)"
              v-if="!item.linkedExercises &&!item.linkedPlanQuestionId"
              color="success"
            >
              اتصال به تمرین
            </VBtn>
              <template v-if="item.linkedExercises">
                <VBtn
                  v-for="exercise in item.linkedExercises"
                  :to="`/Exercises/${exercise.id}`"
                  color="warning"
                >
                  ویرایش {{ exercise.title }}
                </VBtn>
              </template>
            </template>
<!--            <VBtn-->
<!--              v-if="item.linkedPlanQuestionId"-->
<!--              color="error"-->
<!--            >-->
<!--              حذف سوال-->
<!--            </VBtn>-->
          </div>
        </div>
      </template>
    </v-treeview>

  </div>
</template>

<style scoped lang="scss">

</style>
