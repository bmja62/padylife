<script setup lang="ts">

interface IProps {
  message: object;
}

const props = defineProps<IProps>();
</script>

<template>
  <div

      ref="message"
      class="chat"
      :class="props.message.isMe ? 'chat-start' : 'chat-end'"
  >
    <div
        class="chat-bubble"
        :class="[
              props.message.isMe
                ? 'bg-[#01ced1] text-white'
                : 'bg-[#ffff] text-[#212121]',
              props.message.hasTail ? '' : 'before:hidden !rounded-b-box']"
    >
      <div style="overflow-wrap: anywhere"  v-html="props.message.text"></div>
    </div>
  </div>

</template>

<style scoped>

</style>