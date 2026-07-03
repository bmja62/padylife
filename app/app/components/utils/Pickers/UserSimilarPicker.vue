<script setup lang="ts">
import type {IApiProvider} from "~/models/IApiProvider";
import type {ISimilarUser} from "~/services/UsersService";

const props = defineProps({
  required: {
    type: Boolean as PropType<boolean>,
    default: false
  },
})
const selectedOption = defineModel()
const {$api} = useNuxtApp<IApiProvider>()
const options = ref<ISimilarUser[]>([])
const authStore = useAuthStore()
function searchCategories(event) {
  optionsFilters.value.search = event
  getOptions()
}

defineExpose({
  selectRandom
})

function getRandomIdx() {
  return Math.ceil(Math.random() * options.value.length)
}

async function selectRandom() {
  const randomIdx = getRandomIdx()
  if (options.value[randomIdx])
    return selectedOption.value = options.value[randomIdx].value
  getRandomIdx()

}
async function getOptions() {
  try {
    const response = await $api.users.getSimilarUsers(authStore.getUser.id)

    if (response.data) {
      options.value = []
      options.value = response.data.map((item: ISimilarUser) => {
        return {
          label: `${item.fullName ? item.fullName : item.userName} ${item.badgeTitle ? `(${item.badgeTitle} ${item.badgeIcon})` : ''} - ${item.similarityPercent} درصد تشابه پاسخ ها`,
          value: item.userId
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
                              placeholder="انتخاب همراه"
                              :options="options"></LazyUtilsPickersBasePicker>
</template>

<style scoped>

</style>