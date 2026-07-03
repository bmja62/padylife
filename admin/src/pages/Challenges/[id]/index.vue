<script setup lang="ts">
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";
import {VForm} from "vuetify/components/VForm";
import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";
import {IChallenge} from "@/services/ChallengeService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

const spinner = useSpinner()
const challengePayload = ref<IChallenge>(null)
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const refVForm = ref(null)
const imageUrl = ref(null)
const router = useRouter()
const route = useRoute()


async function updateChallenge() {
  try {
    spinner.showSpinner()
    imageUrl.value.getFiles()
    const data = await $api?.challenge.updateChallenge(challengePayload.value)
    if (data.data.isSuccess) {
      alert.success('چالش با موفقیت ویرایش شد')
      router.push(`/Challenges/list`)
    } else {
      alert.error(data.data.message)
    }
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid && challengePayload.value.description) {
    updateChallenge()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

function setFile(medias) {
  if (medias.length) {
    challengePayload.value.imageUrl = medias[0]
  }
}

async function getChallengeById() {
  try {
    spinner.showSpinner()
    const data = await $api?.challenge.getChallengeById(route.params.id)
    challengePayload.value = data.data.data
  } catch (e) {
    console.error(e)
  } finally {
    spinner.hideSpinner()
  }
}

onMounted(() => {
  getChallengeById()
})
</script>

<template>
  <PageWrapper
    v-if="challengePayload"
  >
    <template #title>
      ویرایش چالش
    </template>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <VRow>
        <VCol cols="12" md="6">
          <VTextField
            v-model="challengePayload.title"
            type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']"
            required
            label="عنوان چالش"
          />
        </VCol>
        <VCol cols="12" md="6">
          <ChallengeTypePicker required v-model="challengePayload.type"></ChallengeTypePicker>
        </VCol>
        <VCol cols="12" md="12">
          <FroalaEditor
            v-model="challengePayload.description"
            editor-placeholder="توضیحات چالش "
          />
        </VCol>
        <VCol cols="12">
          <Uploader accept="image/*"  ref="imageUrl"
                    :default-medias="challengePayload.imageUrl"
                    @getFiles="setFile" label="عکس اصلی"
                    :file-type="UploaderTypes.Challenge"></Uploader>
        </VCol>
        <VCol md="12" cols="12" class="d-flex justify-end">
          <VBtn
            type="submit"
            id="buy-now-btn"
            class="product-buy-now"
            color="green"
          >
            ویرایش
          </VBtn>
        </VCol>
      </VRow>
    </VForm>

  </PageWrapper>
</template>

<style scoped lang="scss">

</style>
