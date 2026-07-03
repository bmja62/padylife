<template>
  <VRow>
    <VCol cols="12" md="6">
      <VTextField
        v-model.trim="allCharactersToDash"
        color="success"
        label="seo url *"
      />
    </VCol>
    <VCol cols="12" md="6">
      <VTextField
        v-model.trim="props.currentData.seoTitle"
        color="success"
        label="seo title *"
      />
      <span :class="{'text-lightPink':props?.currentData?.seoTitle?.length===60}"><small>{{
          props?.currentData?.seoTitle?.length
        }}</small><small
        class="text-secondary">/</small><small class="text-secondary">60</small></span>
    </VCol>
    <VCol v-if="props.currentData.canonicalLink!== undefined" cols="12" md="6">
      <VTextField
        v-model.trim="props.currentData.canonicalLink"
        color="success"
        label="canonical link"
      />
    </VCol>
    <VCol cols="12" md="12">
      <VTextarea
        v-model.trim="props.currentData.seoDescription"
        color="success"
        label="seo description *"
        maxlength="160"
      />
      <span v-if="props.currentData.seoDescription "
            :class="{'text-lightPink':props?.currentData?.seoDescription.length===160}"><small>{{
          props?.currentData?.seoDescription.length
        }}</small><small
        class="text-secondary">/</small><small class="text-secondary">160</small></span>
    </VCol>
    <VCol v-if="props.currentData.metaKeywords !==undefined" cols="12" md="6">
      <VTextField
        v-model.trim="props.currentData.metaKeywords"
        color="success"
        label="meta keywords"
      />
    </VCol>
    <VCol v-if="props.currentData.metaAuthor !==undefined" cols="12" md="6">
      <VTextField
        v-model.trim="props.currentData.metaAuthor"
        color="success"
        label="meta author"
      />
    </VCol>
    <VCol v-if="props.showBtn" cols="12" md="12">
      <VBtn
        id="buy-now-btn"
        class="product-buy-now"
        color="green"
        @click="setSeoData"
      >
        ثبت
      </VBtn>
    </VCol>
  </VRow>
</template>

<script lang="ts" setup>
import type {PropType} from "vue";

const props = defineProps({
  currentData: {
    type: Object as PropType<any>
  },
  title: {
    type: String as PropType<string>
  },
  showBtn: {
    type: Boolean,
    default: true
  },
})

// watch(() => props.title, async (val) => {
//   if (!props.currentData.seoURL) {
//     props.currentData.seoURL = characterToDashConverter(val)
//   }
// }, {immediate: true})
defineExpose({
  setSeoData
})
const emits = defineEmits<{
  (e: 'setSeoData', seoData: object): void
}>()
const allCharactersToDash = computed({
  get: function () {
    return props?.currentData?.seoURL;
  },
  set: function (newValue) {
    // This setter is getting number, replace all commas with empty str
    // Then start to separate numbers with ',' from beginning to prevent
    // from data corruption
    if (newValue) {
      props.currentData.seoURL = newValue
      // Remove all characters that are NOT number
      props.currentData.seoURL = props.currentData.seoURL.replace(/[+*!@#$%^&*()_;:~`»«,×='"|<>/?{}\/\.]/g, "");
      props.currentData.seoURL = props.currentData.seoURL.replaceAll(' ', '-')
    } else if (!newValue || props.currentData.seoURL === "") {
      props.currentData.seoURL = null;
    }
  },
})

function characterToDashConverter(str) {
  let resultStr = ''
  str.replace(/[+*!@#$%^&*()_;:~`»«,×='"|<>/?{}\/\.]/g, "");
  resultStr = str.replaceAll(' ', '-')
  return resultStr

}

function setSeoData() {
  emits('setSeoData', {
    seoURL: props.currentData.seoURL,
    seoTitle: props.currentData.seoTitle,
    seoDescription: props.currentData.seoDescription,
    metaKeywords: props.currentData.metaKeywords,
    canonicalLink: props.currentData.canonicalLink,
    metaAuthor: props.currentData.metaAuthor,
    scriptContent: props.currentData.scriptContent,
  })
}
</script>

<style scoped>

</style>
