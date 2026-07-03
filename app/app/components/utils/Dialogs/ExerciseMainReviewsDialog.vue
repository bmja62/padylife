<script setup lang="ts">
import {CommentType} from "~/models/Enums/CommentType";
import type {IEntityComment} from "~/services/CommentService";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const route = useRoute()
const entityCommentFilters = ref({
  entityId: route.params.exerciseId,
  entityType: CommentType.Excersie,
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

onMounted(() => {
  getEntityComments()
})
</script>

<template>
  <LazyUtilsDialogsBaseDialog v-model="isRendering" full-height dialog-id="ExerciseReviews">
    <template #title>
      <span>نظرات هم‌مسیر های شما </span>
    </template>
    <template #default>
      <LazySharedChatMockForComments :entityId="route.params.exerciseId"
                                     :entityType="CommentType.Excersie"></LazySharedChatMockForComments>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>