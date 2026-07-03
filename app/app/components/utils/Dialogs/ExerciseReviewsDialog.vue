<script setup lang="ts">
import {CommentType} from "~/models/Enums/CommentType";
import type {IEntityComment} from "~/services/CommentService";
import type {IApiProvider} from "~/models/IApiProvider";
import type {IStepOption} from "~/services/StepService";

const isRendering = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
interface IProps {
  step: IStepOption
}

const props:IProps = defineProps({
  step: {
    type: Object as PropType<IStepOption>
  }
})
const entityCommentFilters = ref({
  entityId: props.step.id,
  entityType: CommentType.Step,
  pageNumber: 1,
  count: 100
})
const entityComments = ref<IEntityComment[]>([])

async function getEntityComments() {
  try {
    useSpinner().renderSpinner()

    const response = await $api.comment.getEntityComments(entityCommentFilters.value)
    entityComments.value = response.data.data
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

getEntityComments()
</script>

<template>
  <LazyUtilsDialogsBaseDialog v-model="isRendering" full-height dialog-id="ExerciseReviews">
    <template #title>
      <span v-if="props.exercise">تجربه های هم‌مسیر های شما </span>
    </template>
    <template #default>
      <div >
      <LazySharedChatMockForComments :entityId="step?.stepId"
                                     :entityType="CommentType.Step"></LazySharedChatMockForComments>
      </div>
      </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>