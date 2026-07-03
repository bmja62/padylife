<script lang="ts" setup>
interface IProps {
  dialogState: boolean
  title: string
  persistent?: boolean,
  width?:string
}

interface IEmit {
  (e: 'update:dialogState', value: boolean): void

  (e: 'refetch'): void

  (e: 'create'): void
}

// Props
const props = withDefaults(defineProps<IProps>(),{
  width:'500'
})

// Emits
const emit = defineEmits<IEmit>()

// Functions
function sendCreateRequest() {
  emit('create')
}

function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}
</script>

<template>
  <VDialog
    :model-value="props.dialogState"
    :persistent="props.persistent"
    :width="props.width"
    @update:model-value="updateDialogState"
  >
    <VCard>
      <VToolbar
        :title="props.title"
        border
        color="success"
        density="compact"
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
          <slot/>
        </VRow>
      </VCardText>
      <VCardActions class="d-flex align-items-center justify-end pe-6">
        <VBtn
          color="error"
          variant="outlined"
          @click="emit('update:dialogState', false)"
        >
          بستن
        </VBtn>
        <VBtn
          color="success"
          variant="flat"
          @click="sendCreateRequest"
        >
          ثبت
        </VBtn>
      </VCardActions>
    </VCard>
  </VDialog>
</template>
