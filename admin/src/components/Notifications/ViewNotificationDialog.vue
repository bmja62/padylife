<script setup lang="ts">
import type {IApiProvider} from '@/models/IApiProvider'
import {useSpinner} from '@/composables/spinner'
import {useAlerts} from '@/composables/alert'
import {INotification} from "@/services/NotificationsService";

// Interfaces
interface IProps {
  dialogState: boolean
  selectedItem: INotification
}

// Props
const props = withDefaults(defineProps<IProps>(), {})

// Emits
const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()

// Variables
const $api = inject<IApiProvider>('$api')
const spinner = useSpinner()
const alert = useAlerts()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}


</script>

<template>
  <CustomUpdateDialog
    no-accept
    :dialog-state="props.dialogState"
    title="مشاهده متن اعلان"

    @update:dialog-state="updateDialogState"
  >
  <template #actions>
    <span></span>
  </template>
    <VRow>
      <VCol cols="12">
        <p v-html="props.selectedItem.description"></p>
      </VCol>

    </VRow>

  </CustomUpdateDialog>
</template>
