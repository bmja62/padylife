<script setup lang="ts">
import type {IStepOption} from "~/services/StepService";

interface IProps {
  isRenderingQuestion?: boolean;
  stepOption: IStepOption | null
}

const props = withDefaults(defineProps<IProps>(), {
  isRenderingQuestion: true,
  stepOption: null,
});
</script>

<template>
  <div class="w-full  relative">
    <div
        class="w-full h-38 custom-card-shadow rounded-2xl bg-white z-[2] top-0 right-0 p-4"
    >
      <div class="w-full">

        <span class="w-6 h-auto inline-block align-middle">
          <Icon size="24" name="icon:user-new" color="#50429A" />
        </span>
          <p style="overflow-wrap: anywhere" class="text-[#736B66] text-sm inline text-justify">
            {{ stepOption.description }}
        </p>

      </div>
    </div>

  </div>
</template>
