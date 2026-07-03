<script setup lang="ts">
import type {CommentType} from "~/models/Enums/CommentType";
import type {IApiProvider} from "~/models/IApiProvider";

const isRendering = defineModel()

interface IProps {
  commentType: CommentType
  label: string
  entityId: number
}

const {$api} = useNuxtApp<IApiProvider>()
const canRate = ref(true)
const props: IProps = defineProps({

  commentType: {
    type: String as PropType<CommentType>,
    required: true
  },
  entityId: {
    type: Number as PropType<number>,
    required: true
  },
  label: {
    type: String as PropType<string>,
    required: true
  }
})
const commentPayload = ref({
  rate: null,
  description: ''
})

async function createComment() {
  if (commentPayload.value.description) {
    try {
      useSpinner().renderSpinner()
      const response = await $api.comment.createComment({
        entityId: props.entityId,
        entityType: props.commentType,
        text: commentPayload.value.description,
        parentCommentId: null
      })
      if (response.isSuccess) {
        useAlerts().success('نظرت با موفقیت ثبت شدش و پس از تایید به نمایش درمیاد')

      } else {
        useAlerts().error(response.message)
      }
    } catch (e) {
      console.error(e)
    } finally {
      useSpinner().hideSpinner()
      if (canRate.value) {
        createRating()

      }else{
        commentPayload.value = {
          rate: null,
          description: ''
        }
      }
      isRendering.value = false
    }
  } else {
    useAlerts().error('لطفا متن نظر خودت رو وارد کن')
  }
}


async function createRating() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.rate.createRating({
      entityId: props.entityId,
      ratingValue: commentPayload.value.rate,
      entityType: props.commentType,
    })
    if (response.isSuccess) {
      commentPayload.value = {
        rate: null,
        description: ''
      }
    } else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

async function getMyRatingStats() {
  try {
    useSpinner().renderSpinner()
    const response = await $api.rate.getMyRatingStats({
      entityId: props.entityId,
      entityType: props.commentType,
    })

    if (response.isSuccess) {
      if (response.data.isRated) {
        canRate.value = false
      }
    }else {
      useAlerts().error(response.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }

}

getMyRatingStats()
</script>

<template>
  <LazyUtilsDialogsBaseDialog dialog-id="submitRateAndCommentDialog" v-model="isRendering">
    <template #title>
      <span>ثبت نظر</span>
    </template>
    <template #default>
      <div class="w-full flex flex-col gap-3">
        <UtilsInputsBaseInput
            v-model="commentPayload.description"
            name="description"
            :placeholder="`نظرت در مورد این ${props.label} چی بود ؟`"
        ></UtilsInputsBaseInput>

        <ClientOnly>
          <div v-if="canRate" class="flex items-center justify-between">
            <span>به این {{ props.label }} امتیاز بده</span>
            <StarRating
                v-model:rating="commentPayload.rate" class="xl:order-1 order-2" :show-rating="false" :star-size="20"
                :increment="1"></StarRating>
          </div>
        </ClientOnly>
        <button type="button" class="btn btn-primary w-full !rounded-full" @click="createComment">
          ثبت
        </button>
      </div>
    </template>
  </LazyUtilsDialogsBaseDialog>
</template>

<style scoped>

</style>