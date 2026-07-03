<script setup lang="ts">
interface IProps {
  dialogId: string;
  persistent?: boolean;
  okOnly?: boolean;
  closeOnly?: boolean;
  okDisabled?: boolean;
  hasBackdrop?: boolean;
  fullHeight?: boolean;
  backdropClass?: string;
}

const props = withDefaults(defineProps<IProps>(), {
  dialogId: "customDialog",
  persistent: false,
  okOnly: false,
  closeOnly: false,
  okDisabled: false,
  fullHeight: false,
  backdropClass: "[&::backdrop]:bg-transparent",
});

const dialogState = defineModel<boolean>();

const dialogRef = useTemplateRef<HTMLDialogElement>(props.dialogId);
function openDialog() {
  dialogRef.value && dialogRef.value.showModal();
}
function closeDialog() {
  dialogRef.value && dialogRef.value.close();
  nextTick(() => {
    setTimeout(() => {
      dialogState.value = false;
    }, 90);
  });
}

watchEffect(() => {
  if (dialogState.value == true) {
    openDialog();
  } else if (dialogState.value == false) {
    closeDialog();
  }
});

function closeOnBackground() {
  if (!props.persistent) {
    closeDialog();
  } else {
    maketheDialogShake();
  }
}

function maketheDialogShake() {
  if (dialogRef.value) {
    dialogRef.value.classList.add("shake-animation");
    setTimeout(() => {
      dialogRef.value && dialogRef.value.classList.remove("shake-animation");
    }, 350);
  }
}

defineExpose({
  openDialog,
  closeDialog,
});
</script>

<template>
  <template v-if="dialogState">
    <dialog

      :ref="props.dialogId"
      class="modal modal-bottom md:modal-middle "
      :class="backdropClass ? [backdropClass] : '[&::backdrop]:bg-[#171A1F]/60'"
      @keydown.esc.prevent="closeOnBackground"
    >
      <div class="modal-box bg-white custom-scrollbar py-0 overflow-hidden" :class="{'h-[calc(100svh-100px)]':fullHeight}">
        <div class="w-full pt-6 pb-2 sticky top-0">
          <h3 class="text-2xl font-bold text-center">
            <slot name="title" />
          </h3>
        </div>
        <div
            :class="{'min-h-[calc(100svh-300px)] max-h-[calc(100svh-150px)]':props.fullHeight,'max-h-[calc(100svh-150px)]':!props.fullHeight}"
            class="py-4 pl-1 custom-scrollbar overflow-y-auto "
        >
          <slot />
        </div>
      </div>
      <div class="modal-backdrop bg-black/40">
        <button type="button" class="cursor-default" @click="closeOnBackground">
          بستن
        </button>
      </div>
    </dialog>
  </template>
</template>
<style scoped>
.shake-animation {
  animation: shake 0.1s ease-in-out;
}
</style>
