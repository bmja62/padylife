<script setup lang="ts">
import type { PropType } from 'vue';
import type { ICourseProgress } from '~/services/ReportService';
interface IProps {
    course: ICourseProgress
}
const props: ICourseProgress = defineProps({
    course: {
        type: Object as PropType<ICourseProgress>
    }
})
</script>
<template>
    <div class="w-full flex flex-col col-span-2 items-center bg-white plans-custom-shadow rounded-xl p-4">
        <ChartsRadialChart :percent="props.course.progressPercentage" />
        <p class="text-[12px] text-center font-bold char-title" style="overflow-wrap: anywhere">
            {{ props.course.planTitle }}
        </p>
        <NuxtLink :to="`/dashboard/user/my-plans/companions/${props.course.planId}`"
            class="btn btn-sm !text-xs mt-2 w-full bg-primary text-white border-none">
            مشاهده همراهان
        </NuxtLink>
    </div>
</template>