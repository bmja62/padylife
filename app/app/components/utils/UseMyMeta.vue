<template>

</template>

<script lang="ts" setup>
import type {PropType} from "vue";
import type {MetaFlat} from "zhead";

export interface IPageData {
  seoTitle: string,
  seoDescription: string,
}

const route = useRoute()
let props = defineProps({
  pageData: {
    type: Object as PropType<IPageData>
  },
  seoMeta: {
    type: Object as PropType<MetaFlat>
  }
})
let seoTitle = ref(null)
let seoDescription = ref(null)
let computedSeoTitle = computed(() => {
  return seoTitle.value
})
let computedSeoDescription = computed(() => {
  return seoDescription.value
})


if (process.client) {
  onMounted(() => {
    seoTitle.value = props?.pageData?.seoTitle
    seoDescription.value = props?.pageData?.seoDescription
  })
  useHead({
    title: computedSeoTitle,
    meta: [
      {
        name: "description",
        content: computedSeoDescription,
      },
    ],
    // this is not working
    // link: [
    //   {
    //     rel: 'canonical',
    //     href: `https://azmaleather.ir${route.path}`
    //   }
    // ]

  })
  // this is not working
  useSeoMeta(props.seoMeta)
}
useServerHead({
  link: [
    {
      rel: 'canonical',
      href: `https://azmaleather.ir${route.path}`
    }
  ]
})
useServerSeoMeta(props.seoMeta)
</script>

<style scoped>

</style>
