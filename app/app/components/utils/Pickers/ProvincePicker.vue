<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {IProvince} from "~/services/SettingService";

const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  },
  countryId: {
    type: Number as PropType<number>,
  }

})


const selectedOption = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const options = ref<IProvince[]>([])
const optionsFilters = ref({
  pageNumber: 1,
  count: 10,
  search: '',
  countryId: null
})

function searchCategories(event) {
  optionsFilters.value.search = event
  getOptions()
}

async function getOptions() {
  try {
    const response = await $api.setting.getProvinces(optionsFilters.value)

    if (response.data) {
      options.value = []
      options.value = response.data.data.map((item: IProvince) => {
        return {
          label: item.provinceNameFa,
          value: item.id
        }
      })
    }
  } catch (e) {
    console.log(e)
  }
}
watchEffect(() => {
  if (props.countryId) {
    console.log(props.countryId)
    optionsFilters.value.countryId = props.countryId
    getOptions()
  }
})
getOptions()
</script>

<template>
  <LazyUtilsPickersBasePicker v-if="options.length" @search="searchCategories" :required="props.required"
                              v-model="selectedOption"
                              placeholder="انتخاب استان"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>