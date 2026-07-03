<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {ICity} from "~/services/SettingService";

const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  },
  provinceId: {
    type: Number as PropType<number>,
  }
})
const selectedOption = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const options = ref<ICity[]>([])
const optionsFilters = ref({
  pageNumber: 1,
  count: 200,
  search: '',
  provinceId: null
})

function searchCategories(event) {
  optionsFilters.value.search = event
  getOptions()
}

async function getOptions() {
  try {
    const response = await $api.setting.getCities(optionsFilters.value)

    if (response.data) {
      options.value = []
      options.value = response.data.data.map((item: ICity) => {
        return {
          label: item.cityNameFa,
          value: item.id
        }
      })
    }
  } catch (e) {
    console.log(e)
  }
}

watchEffect(() => {
  if (props.provinceId) {
    optionsFilters.value.provinceId = props.provinceId
    getOptions()
  }
})
getOptions()
</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length" @search="searchCategories" :required="props.required"
                              v-model="selectedOption"
                              placeholder="انتخاب شهر"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>