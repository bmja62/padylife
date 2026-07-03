<script setup lang="ts">
import type {IEntityComment} from "~/services/CommentService";

interface IProps {
  comment: IEntityComment
}

const props: IProps = defineProps({
  comment: {
    type: Object as PropType<IEntityComment>
  }
})
const emits = defineEmits<{
  (e: 'refetch'): void
}>()
const {$api} = useNuxtApp()

async function reactToComment(type: string) {
  try {
    useSpinner().renderSpinner()
    const response = await $api.comment.reactToComment(props.comment.id, type)
    if (response.isSuccess) {
      emits('refetch')
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <div :class="{'chat-start w-52':props.comment.isMe,'chat-end -w-52':!props.comment.isMe}"
       class="chat  flex !flex-col">
    <small class="text-xs text-gray-500">{{
        props.comment.userInfo.fullName ? props.comment.userInfo.fullName : '-'
      }}</small>
    <div :class="{'bg-[#01ced1] text-white':props.comment.isMe,'bg-[#ffff] text-[#212121]':!props.comment.isMe}"
         class="chat-bubble">{{ props.comment.text }}
    </div>
    <div class="w-full flex items-center gap-1 mt-1"
         :class="{'justify-start':props.comment.isMe,'justify-end':!props.comment.isMe}">
      <Icon v-if="!props.comment.isLikedByMe" name="icon:heart-outline-new"
            class="stroke-red-500 w-4 h-4" @click="reactToComment('Like')"/>
      <Icon v-else name="icon:heart-fill-new" class="!fill-red-500 w-4 h-4" @click="reactToComment('DisLike')"/>
      <small class="text-xs text-gray-500">{{
          props.comment.likeCount
        }}</small>
    </div>
  </div>
</template>

<style scoped>

</style>