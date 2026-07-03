<script lang="ts" setup>
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {useAuthStore} from '@/stores/auth'
import {useAlerts} from "@/composables/alert";

// Interfaces

// LifeCycles


// Variables
const $api = inject<IApiProvider>('$api')
const auth = useAuthStore()
const spinner = useSpinner()
const route = useRoute()
const alert = useAlerts()
const currentBlogData = ref(null)

// Functions
async function getBlogById() {
  try {
    spinner.showSpinner()
    const response = await $api?.blog.getBlogById(route.params.blogId)
    currentBlogData.value = response.data.data
  } catch (error) {
    console.error(error)
  } finally {
    spinner.hideSpinner()
  }
}


onMounted(() => {
  getBlogById()
})
</script>

<template>
  <PageWrapper
  >
    <template #title>
      بروزرسانی خبر
    </template>
    <CreateOrUpdateBlogForm :currentBlogData="currentBlogData"></CreateOrUpdateBlogForm>
  </PageWrapper>
</template>
