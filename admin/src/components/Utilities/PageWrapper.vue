<script setup lang="ts">
interface IProps {
  hasFilters?: boolean
  isFlat?: boolean
}
interface IEmits {
  (e: 'submitFilters'): void
}

const props = withDefaults(defineProps<IProps>(), {
  hasFilters: false,
  isFlat: false,
})

const emit = defineEmits<IEmits>()

// Functions
function submitFilters() {
  emit('submitFilters')
}
</script>

<template>
  <VCard
    class="custom-card mb-6"
    :flat="isFlat"
  >
    <VCardTitle>
      <VRow
        justify="start"
        justify-md="space-between"
        class="pa-2"
      >
        <VCol
          cols="12"
          md="auto"
        >
          <h2 class="mb-2 w-auto text-wrap  text-md-start">
            <slot name="title" />
          </h2>
        </VCol>
        <VCol
          cols="auto"
          md="auto"
        >
          <slot name="append" />
        </VCol>
      </VRow>
    </VCardTitle>
    <VContainer
      v-if="props.hasFilters"
      fluid
    >
      <VRow no-gutters>
        <VCol cols="12">
          <VForm @submit.prevent>
            <VRow dense>
              <slot name="filters" />
              <VCol
                cols="12"
                md="1"
              >
                <VBtn
                  width="100%"
                  variant="outlined"
                  type="button"
                  color="primary"
                  @click="submitFilters"
                >
                  اعمال
                </VBtn>
              </VCol>
            </VRow>
          </VForm>
        </VCol>
      </VRow>
    </VContainer>
    <div
      v-if="$slots.default"
      class="pa-4"
    >
      <slot />
    </div>
    <div v-if="$slots.flatChild">
      <slot name="flatChild" />
    </div>
  </VCard>
</template>

<style>
.custom-card .v-card-title {
  line-height: normal !important;
}
</style>
