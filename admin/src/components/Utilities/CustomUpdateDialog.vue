<script setup lang="ts">
interface IProps {
  dialogState: boolean
  title: string
  actionText?: string
  persistent?:boolean,


}
interface IEmit {
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
  (e: 'update'): void
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  actionText: 'ویرایش',
  width:'500'
})

// Emits
const emit = defineEmits<IEmit>()

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}

async function sendUpdateRequest() {
  emit('update')
}
</script>

<template>
  <VDialog
    :persistent="props.persistent"
    width="500"
    :model-value="props.dialogState"
    @update:model-value="updateDialogState"
  >
    <VCard>
      <VToolbar
        border
        color="update"
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
          بستن
        </VBtn>
        <slot name="actions">
          <VBtn
            variant="flat"
            color="update"
            @click="sendUpdateRequest"
          >
            {{ props.actionText }}
          </VBtn>
        </slot>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
