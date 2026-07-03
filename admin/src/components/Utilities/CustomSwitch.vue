<script setup lang="ts">
interface IProps {
  hasIcon?: boolean
  iconName?: string | undefined
  hasTooltip?: boolean
  tooltipText?: string | null
  switchHint?: string | null
  label?: string | null
  color?: string
  value?: unknown
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  hasIcon: true,
  iconName: 'tabler:info-circle',
  hasTooltip: true,
  switchHint: null,
  label: null,
  color: 'primary',
})

// Models
const model = defineModel()
</script>

<template>
  <div class="w-auto">
    <VLabel
      v-if="props.hasIcon || props.hasTooltip"
      for="switch"
    >
      <VIcon
        v-if="props.hasIcon"
        class="ml-1"
        size="small"
        :icon="iconName"
        color="gray"
      />
      {{ props.label }}
      <VTooltip
        v-if="props.hasTooltip"
        activator="parent"
        location="top"
      >
        {{ props.tooltipText }}
      </VTooltip>
    </VLabel>
    <VSwitch
      id="switch"
      v-model="model"
      :inset="false"
      :value="props.value"
      :color="props.color"
      :label="props.switchHint ? props.switchHint : undefined"
    />
  </div>
</template>
