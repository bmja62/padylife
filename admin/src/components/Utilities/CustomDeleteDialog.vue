<script setup lang="ts">
interface IProps {
  dialogState: boolean
  title: string
  actionText?: string
  cancelText?: string

}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
  (e: 'delete'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  actionText: 'حذف',
  cancelText: 'بستن',
})

// Emits
const emit = defineEmits<IEmit>()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}
function sendDeleteRequest() {
  emit('delete')
}
</script>

<template>
  <VDialog
    width="500"
    :model-value="props.dialogState"
    @update:model-value="updateDialogState"
  >
    <VCard>
      <VToolbar
        border
        color="error"
        density="compact"
        :title="props.title"
      >
        <VBtn
          density="compact"
          icon
          @click="emit('update:dialogState', false)"
        >
          <VIcon color="white">
            mdi-close
          </VIcon>
        </VBtn>
      </VToolbar>
      <VCardText>
        <VRow>
          <slot />
        </VRow>
      </VCardText>
      <VCardActions class="d-flex align-items-center justify-end pe-6">
        <VBtn
          variant="outlined"
          color="error"
          @click="emit('update:dialogState', false)"
        >
          {{ props.cancelText }}
        </VBtn>
        <VBtn
          variant="flat"
          color="error"
          @click="sendDeleteRequest"
        >
          {{ props.actionText }}
        </VBtn>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
