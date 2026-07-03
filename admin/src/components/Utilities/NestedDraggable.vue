<template>
  <div class="">
    <draggable v-model="props.items" item-key="id" @end="identifyEvent">
      <template #item="{ element }">
        <div class="draggable-item my-1">
          <span>{{ element.name }}</span>
          <NestedDraggable @refetch="refetchAgain" v-if="element.childs.length" :items="element.childs"
          ></NestedDraggable>

        </div>

      </template>
    </draggable>
  </div>
</template>

<script setup lang="ts">
import draggable from 'vuedraggable';
import {useAlerts} from "@/composables/alert";
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const $api = inject<IApiProvider>('$api')

const props = defineProps({
  items: {
    type: Array as PropType<[]>
  }
})
const emits = defineEmits<{
  (e:'refetch'):void
}>()

const events = {
  added: 'عملیات غیرمجاز',
  removed: 'عملیات غیرمجاز',
}
const alert = useAlerts()
function refetchAgain(){
  emits('refetch')
}
function identifyEvent(event) {
  // Object.keys(event).forEach((key) => {
  //   if (events[key]) {
  //     alert.error(events[key])
  //   } else {
  //
  //   }
  // })
  createPayload(event)
}

function createPayload(event) {
  let currentObject = props.items[event.oldIndex];
  let objectToMove = props.items[event.newIndex];
  setPriority(currentObject, objectToMove)
}

async function setPriority(currentObject, objectToMove) {
  try {
    const res = await $api?.products.setProductCategoryPriority(currentObject.productCategoryId, objectToMove.productCategoryId)
    if (res.data.isSuccess) {
      alert.success('عملیات با موفقیت انجام شد')
      emits('refetch')
    }
  } catch (e) {
    console.log(e)
  }
}

</script>

<style scoped>
.draggable-container {
  width: 300px;
  padding: 10px;
  background: #f8f9fa;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.draggable-item {
  padding: 10px;
  margin: 5px 0;
  background: #ffffff;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: grab;
}

.draggable-item:active {
  cursor: grabbing;
}
</style>
