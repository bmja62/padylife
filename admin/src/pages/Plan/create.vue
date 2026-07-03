<script setup lang="ts">
import {useSpinner} from "@/composables/spinner";
import {IApiProvider} from "@/models/IApiProvider";
import {inject} from "vue";
import {useAlerts} from "@/composables/alert";
import {useRouter} from "vue-router";
import {VForm} from "vuetify/components/VForm";
import {ICreatePlan} from "@/services/PlanService";
import FroalaEditor from "@/components/Utilities/FroalaEditor.vue";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";

const spinner = useSpinner()
const planPayload = ref<ICreatePlan>({
  title: '',
  isSignUpPlan:false,
  imageUrl:'',
  planCategoryId: null,
  description: '',
  level: '',
  price: ''
})
const $api = inject<IApiProvider>('$api')
const alert = useAlerts()
const refVForm = ref(null)
const imageUrl = ref(null)
const isFree = ref(false)
const router = useRouter()


async function createPlan() {
  try {
    spinner.showSpinner()
    if (isFree.value) {
      planPayload.value.price = null
    } else {
      planPayload.value.price = planPayload.value.price.replaceAll(',', '')
    }
    imageUrl.value.getFiles()
    const data = await $api?.plan.createOrUpdatePlan(planPayload.value)
    if (data.data.isSuccess) {
      alert.success('پلن با موفقیت ساخته شد')
      router.push(`/Plan/${data.data.data.id}`)
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
  if (isValid.valid && planPayload.value.description) {
    createPlan()
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}
function setFile(medias) {
  if (medias.length) {
    planPayload.value.imageUrl = medias[0]
  }
}
const formattedPrice = computed({
  get: function () {
    return planPayload.value.price;
  },
  set: function (newValue) {
    if (newValue && newValue !== "") {
      // Remove all characters that are NOT numbers
      const cleanedValue = newValue.replace(/[^\d]/g, "");

      // Format the cleaned value with commas
      if (cleanedValue) {
        planPayload.value.price = Number(cleanedValue).toLocaleString("en-US");
      } else {
        planPayload.value.price = null;
      }
    } else {
      planPayload.value.price = null;
    }
  },
});
</script>

<template>
  <PageWrapper
  >
    <template #title>
      ایجاد یک پلن جدید
    </template>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <VRow>
        <VCol cols="12" md="6">
          <PlanCategoryPicker required v-model="planPayload.planCategoryId"></PlanCategoryPicker>
        </VCol>
        <VCol cols="12" md="6">
          <VTextField
            v-model="planPayload.title"
            type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']"
            required
            label="عنوان پلن"
          />
        </VCol>
        <VCol cols="12" md="6">
          <CustomSwitch
            v-model="isFree"
            label="پلن رایگان"
            id="customizer-menu-collapsed"
            :has-tooltip="false"
            class="ms-2"
          />
          <VTextField
            v-if="!isFree"
            v-model="formattedPrice"
            type="text"
            :rules="[!isFree ? (value) => !!value || 'این فیلد اجباری است' : '']"
            required
            label="قیمت پلن (تومان)"
          />
        </VCol>

        <VCol cols="12" md="12">
          <VTextField
            v-model="planPayload.level"
            type="text"
            :rules="[(value) => !!value || 'این فیلد اجباری است']"
            required
            label="سطح"
          />
        </VCol>
        <VCol md="12" cols="12" class="">
          <Uploader ref="imageUrl"
                    :key="planPayload.imageUrl"
                    @getFiles="setFile" label="تصویر پلن"
                    :file-type="UploaderTypes.Plan"></Uploader>
        </VCol>
        <VCol cols="12" md="12">
          <FroalaEditor
            v-model="planPayload.description"
            editor-placeholder="توضیحات پلن *"
          />
        </VCol>
        <VCol md="12" cols="12" class="d-flex justify-end">
          <VBtn
            type="submit"
            id="buy-now-btn"
            class="product-buy-now"
            color="green"
          >
            ثبت
          </VBtn>
        </VCol>
      </VRow>
    </VForm>

  </PageWrapper>
</template>

<style scoped lang="scss">

</style>
