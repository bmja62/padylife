<script setup lang="ts">
import type {ValidationRule} from '@/models/IValidationRule'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";

const $api = inject<IApiProvider>('$api')

onMounted(() => {
  getAllBlogCategories()
})

// Interfaces
interface IEmits {
  (e: 'getSelectedCategory', value: any | null): void
}

interface IProps {
  label?: string
  disabled?: boolean
  color?: string
  rules?: ValidationRule[]
  validationOrder?: number,
  returnObject: boolean
  required: boolean
}

// Props
const props = withDefaults(defineProps<IProps>(), {
  label: 'انتخاب دسته بندی',
  disabled: false,
  color: 'success',
  returnObject: true,
  required: false
})

// Emits
const emits = defineEmits<IEmits>()

// Variables
const tempSelectedCategory = defineModel()

const blogCategoryList = ref<object[]>()

// Watchers
watch(
  () => tempSelectedCategory.value,
  value => {
    emits('getSelectedCategory', value)
  },
)

// functions
async function getAllBlogCategories() {
  try {
    const response = await $api?.blog.getAllBlogCategories({
      pageNumber:1,
      count:1000
    })
    blogCategoryList.value = response?.data.data.data as Array<ItempCategory>
  } catch (error) {
    console.error(error)
  } finally {
  }
}
</script>

<template>
  <CustomPicker
    v-model="tempSelectedCategory"
    rounded="lg"
    :validation-order="props.validationOrder"
    :disabled="props.disabled"
    :color="props.color"
    :label="props.label"
    :required="props.required"
    :rules="[(value) => !!value || 'این فیلد اجباری است']"
    :return-object="props.returnObject"
    :items="blogCategoryList"
    :item-title="(item: any) => item.title"
    :item-value="(item: any) => item.id"
    :search-callback="() => null"
  />
</template>
