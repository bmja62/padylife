<script lang="ts" setup>
import {inject} from 'vue'
import {useSpinner} from '@/composables/spinner'
import type {IApiProvider} from '@/models/IApiProvider'
import {useAuthStore} from '@/stores/auth'

// Interfaces

// LifeCycles


// Variables
const $api = inject<IApiProvider>('$api')
const auth = useAuthStore()
const spinner = useSpinner()


// Functions

</script>

<template>
  <PageWrapper
  >
    <template #title>
      نوشتن خبر جدید
    </template>
    <CreateOrUpdateBlogForm></CreateOrUpdateBlogForm>
  </PageWrapper>
</template>
