<script setup lang="ts">
import {DynamicSettings, type IDynamicSetting} from "~/services/SiteDynamicSetting";

interface IProps {
  slogan: string
  lvh: string
}

const props: IProps = defineProps({
  slogan: {
    type: String as PropType<string>,
    default: ''
  },
  lvh:{
    type: String as PropType<string>,
    default: 'h-[50lvh]'
  }
})
const slothAnchor = useTemplateRef("SlothAnchor");
const babakSetting = ref<IDynamicSetting>(null)
const spinner = useSpinner()
const observerInstance = ref<IntersectionObserver>();
const {$api} = useNuxtApp()

function sayHelloToBabak(entry: IntersectionObserverEntry[]) {
  if (entry && entry.length && entry[0]?.isIntersecting) {
    isRenderingBabak.value = true;
  } else {
    isRenderingBabak.value = false;
  }
}

async function getSiteDynamicSetting() {
  try {
    spinner.renderSpinner()
    const response = await $api.dynamicSetting.getSiteDynamicSettingByKeyAndType({
      type: DynamicSettings.babak,
      key: DynamicSettings.babak
    })
    babakSetting.value = response.data
    babakSetting.value.jsonValue =JSON.parse(babakSetting.value.jsonValue)
  } catch (error) {
    console.error(error.message);
  } finally {
    spinner.hideSpinner()
  }
}
const isRenderingBabak = ref<boolean>(false);

onMounted(() => {
  nextTick(() => {
    if (slothAnchor.value) {
      observerInstance.value = new IntersectionObserver(sayHelloToBabak);
      observerInstance.value.observe(slothAnchor.value as HTMLDivElement);
    }
    getSiteDynamicSetting()
  });
});

</script>

<template>
  <div class=" relative" :class="[props.lvh]">
    <div ref="SlothAnchor" class="absolute bottom-0 right-0"></div>
    <transition name="babak">
      <div
          v-show="isRenderingBabak"
          class="fixed bottom-36 -left-3 flex items-center justify-center z-50"
      >
        <div class="chat chat-end">
          <div class="chat-bubble chat-bubble-primary text-white text-center w-full">
            {{ babakSetting?.jsonValue ? babakSetting.jsonValue.title : props.slogan }}
          </div>
        </div>
        <img
            alt="babak the sloth"
            class="w-28 h-28 object-contain z-50"
            src="/core/babak.png"
        />
      </div>
    </transition>
  </div>
</template>

<style scoped>
@keyframes babak-move {
  0% {
    left: -100%;
  }
  100% {
    left: -12px;
  }
}

.babak-enter-active {
  animation: babak-move 1s ease-out;
}

.babak-leave-active {
  animation: babak-move reverse 1s ease-out;
}
</style>