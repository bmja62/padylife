<script setup lang="ts">
import {CommentType} from "~/models/Enums/CommentType";
import type {IEntityComment} from "~/services/CommentService";
import type {IApiProvider} from "~/models/IApiProvider";

const {$api} = useNuxtApp<IApiProvider>()

interface IProps {
  entityId: number
  entityType: string
}

const props: IProps = defineProps({
  entityId: {
    type: Number as PropType<number>
  },
  entityType: {
    type: String as PropType<CommentType>
  }
})
const entityCommentFilters = ref({
  entityId: props.entityId,
  entityType: props.entityType,
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
  <div

      class="w-full h-[calc(100vh-200px)] p-4 bg-[#f7f8fe] overflow-y-auto rounded-[32px] flex flex-col gap-3"
  >
    <template v-if="entityComments.length">

      <LazySharedCommentBubble
          v-for="comment in entityComments" :key="comment.id"
          :comment="comment" @refetch="getEntityComments"></LazySharedCommentBubble>
    </template>
    <div v-else class="flex flex-col items-center justify-center gap-2">
      <Icon name="icon:comments-v1" class="w-20 h-20 "></Icon>
      <strong class="text-center">هنوز هیچ تجربه‌ای ثبت نشده! میتونی اولین تجربه رو ثبت کنی</strong>
    </div>
  </div>
</template>

<style scoped>

</style>