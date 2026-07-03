<script setup lang="ts">
import {TransitionPresets, useTransition} from "@vueuse/core";

const {transitionBaseNumber, message, isErrorAlert,isNotificationAlert, transitionDuration} =
  storeToRefs(useAlertStore());

const { closeAlert } = useAlertStore();

const utils = useUtils();
const currentTransition = ref<(n: number) => number>(
  utils.easeOutElasticAnimation
);

const customFnNumber = useTransition(transitionBaseNumber, {
  duration: transitionDuration,
  transition: currentTransition,
  onStarted() {
    if (transitionBaseNumber.value == 100) {
      // @ts-expect-error Changing with cubicBezierPoints
      currentTransition.value = TransitionPresets.easeInQuint;
    }
  },
  onFinished() {
    if (transitionBaseNumber.value == 100) {
      currentTransition.value = utils.easeOutElasticAnimation;
      closeAlert();
    }
  },
});
</script>

<template>
  <div class="w-full bg-white">
    <div
        v-if="!isNotificationAlert"
      role="alert"
      class="alert w-5/6 md:w-1/3 fixed top-10 -right-full z-[9999] -translate-x-[10%] md:-translate-x-10"
      :style="`right: ${customFnNumber}%`"
      :class="isErrorAlert ? 'alert-error' : 'alert-success'"
    >
      <Icon
        :name="isErrorAlert ? 'icon:xmark-circle' : 'icon:tick-circle'"
        class="text-white"
        size="30"
      />
      <span class="text-white">
        {{ message || "مشکلی پیش آمد، لطفا دوباره امتحان کنید" }}
      </span>
    </div>
    <div
        v-else
        role="alert"
        class="alert w-5/6 md:w-1/3 bg-white fixed top-10 -right-full z-50 -translate-x-[10%] md:-translate-x-10"
        :style="`right: ${customFnNumber}%`"
    >
      <Icon
          :name="'icon:bell-new' "
          class="text-black"
          size="30"
      />
      <strong>
        {{ message}}
      </strong>
        <nuxt-link to="/dashboard/notifications" type="button" class="btn bg-primary text-white border-none w-full">مشاهده</nuxt-link>
    </div>
  </div>
</template>
