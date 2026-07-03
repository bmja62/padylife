<script setup lang="ts">
interface IProps {
  dialogState: boolean
  title: string

}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
  (e: 'update'): void
}

// Props
const props = defineProps<IProps>()

// Emits
const emit = defineEmits<IEmit>()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}
</script>

<template>
  <VDialog
    width="500"
    :model-value="props.dialogState"
    @update:model-value="updateDialogState"
  >
    <VCard max-height="80dvh">
      <VToolbar
        border
        color="warning"
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
      <VCardActions class="d-flex align-items-center justify-end">
        <VBtn
          variant="outlined"
          color="error"
          @click="emit('update:dialogState', false)"
        >
          بستن
        </VBtn>
        <slot name="actions" />
      </VCardActions>
    </VCard>
  </VDialog>
</template>
