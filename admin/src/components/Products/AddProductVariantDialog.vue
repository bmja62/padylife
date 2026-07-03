`
<script setup lang="ts">
import {isAxiosError} from 'axios'
import {inject} from "vue";
import type {IApiProvider} from "@/models/IApiProvider";
import {VForm} from "vuetify/components/VForm";
import {ICreateVariantPayload} from "@/services/ProductService";
import {useAlerts} from "@/composables/alert";
import {useSpinner} from "@/composables/spinner";
import {IAttribute} from "@/services/ProductAttributes";

interface IProps {
  dialogState: boolean
}

const props = defineProps<IProps>()
const emit = defineEmits<{
  (e: 'update:dialogState', value: boolean): void
  (e: 'refetch'): void
}>()
const $api = inject<IApiProvider>('$api')
const route = useRoute()
const refVForm = ref(null)
const selectedAttributes = ref([])
const mainImage = ref({
  file: null,
  pictureUrl: ''
})
const galleryImages = ref([])
const variantPayload = ref<ICreateVariantPayload>({
  sku: "",
  price: null,
  attributeValues: []
})

// Functions
function updateDialogState(val: boolean) {
  emit('update:dialogState', val)
}


function setMainImage(event) {
  mainImage.value.file = event.target.files[0]
  mainImage.value.pictureUrl = URL.createObjectURL(mainImage.value.file)

}

function setGalleryImages(event) {
  for (let i = 0; i < event.target.files.length; i++) {
    galleryImages.value.push({
      file: event.target.files[i],
      pictureUrl: URL.createObjectURL(event.target.files[i])
    })
  }
}

async function validateData() {
  if (!selectedAttributes.value.length)
    return useAlerts().error('لطفا حداقل یک ویژگی انتخاب کنید')
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    await Promise.all([
      await generateAttributesPayload(),
      await addVariantToProduct()
    ])
  } else {
    useAlerts().error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function generateAttributesPayload() {
  selectedAttributes.value.forEach((attr: IAttribute) => {
    variantPayload.value.attributeValues.push({
      attributeId: attr.id,
      value: attr.value!
    })
  })
}

async function addVariantToProduct() {
  try {
    useSpinner().showSpinner()
    variantPayload.value.price = variantPayload.value.price.replaceAll(',', '')
    variantPayload.value.sku = `PLP-${route.params.id}-${variantPayload.value.sku}`
    const response = await $api?.product.addProductVariant(variantPayload.value,route.params.id)
    if (response.data.isSuccess) {
      variantPayload.value = {
        sku: "",
        price: null,
        attributeValues: []
      }
      selectedAttributes.value = []
      useAlerts().success('قیمت با موفقیت ایجاد شد!')
      emit('update:dialogState', false)
      emit('refetch')
    } else {
      useAlerts().error(response.data.message)
    }
  } catch (error: unknown) {
    if (isAxiosError(error))
      useAlerts().error(error?.response?.data?.message || 'مشکلی پیش آمد، لطفا دوباره امتحان کنید')
    else
      console.error(error)
  } finally {
    useSpinner().hideSpinner()
  }
}

const formattedPrice = computed({
  get: function () {
    return variantPayload.value.price;
  },
  set: function (newValue) {
    if (newValue && newValue !== "") {
      // Remove all characters that are NOT numbers
      const cleanedValue = newValue.replace(/[^\d]/g, "");

      // Format the cleaned value with commas
      if (cleanedValue) {
        variantPayload.value.price = Number(cleanedValue).toLocaleString("en-US");
      } else {
        variantPayload.value.price = null;
      }
    } else {
      variantPayload.value.price = null;
    }
  },
});
</script>

<template>
  <CustomCreateDialog
    width="800"
    :dialog-state="props.dialogState"
    title="ایجاد قیمت جدید"
    @update:dialog-state="updateDialogState"
    @create="validateData"
  >
    <VCol cols="12" md="12">
      <ProductAttributePicker static dropdown-label="انتخاب ویژگی" multiple chips
                              v-model="selectedAttributes"></ProductAttributePicker>
    </VCol>
    <VForm
      class="w-100"
      ref="refVForm"
      @submit.prevent="validateData"
    >
      <VCol v-for="(item,idx) in selectedAttributes" :key="item.attributeId" cols="12">
        <VRow>
          <VCol md="1" cols="12" class="cursor-pointer" @click="selectedAttributes.splice(idx,1)">
            <VBtn
              density="compact"
              color="red"
              elevation="0"
              icon="mdi-close"
            />
          </VCol>
          <VCol md="5" cols="12" class="col-md-2">
            <VTextField
              v-model="item.name"
              disabled
              color="success"
              density="compact"
              hide-details="auto"
              label="مقدار جزئیات"
              type="text"
              variant="outlined"
            />
          </VCol>
          <VCol md="6" cols="12" class="col-md-3">
            <VTextField
              v-model="item.value"
              color="success"
              density="compact"
              hide-details="auto"
              label="مقدار جزئیات"
              :rules="[(value) => !!value || 'این فیلد اجباری است']"
              required
              type="text"
              variant="outlined"
            />

          </VCol>
        </VRow>
      </VCol>
      <VCol md="12">
        <VTextField
          v-model="formattedPrice"
          color="success"
          density="compact"
          hide-details="auto"
          label="قیمت (تومان)"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          type="text"
          variant="outlined"
        />
      </VCol>
      <VCol md="12">
        <VTextField
          v-model="variantPayload.sku"
          color="success"
          density="compact"
          hide-details="auto"
          label="شناسه SKU"
          :rules="[(value) => !!value || 'این فیلد اجباری است']"
          required
          dir="auto"
          type="text"
          variant="outlined"
        >
          <template #append-inner>
            <span style="min-width: max-content ">-PLP-{{ Intl.NumberFormat('en-US').format(route.params.id) }}</span>
          </template>
        </VTextField>
      </VCol>
      <VCol cols="12" md="12">
        <VFileInput
          id="mainImage"
          label="تصویر اصلی "
          accept="image/*"
          @input="setMainImage($event)"
        ></VFileInput>
        <div v-if="mainImage.pictureUrl" class=" mt-2  gap-2">
          <VImg :src="mainImage.pictureUrl"
                style="object-fit: cover;width: 10rem;height: 10rem"></VImg>
        </div>
      </VCol>
      <VCol cols="12" md="12">
        <VFileInput
          id="galleryImages"
          label="افزودن تصاویر "
          accept="image/*"
          multiple
          @input="setGalleryImages($event)"
        ></VFileInput>
        <div v-if="galleryImages.length" class="d-flex align-content-center justify-start mt-2">
          <div v-for="(item,idx) in galleryImages" class="position-relative"
               style="width: 10rem;height: 10rem">
            <div style="cursor: pointer;z-index: 999!important" @click="galleryImages.splice(idx,1)">
              <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>
            </div>
            <VImg :src="item.pictureUrl"
                  style="object-fit: cover;width: 7rem;height: 7rem"></VImg>
          </div>

        </div>
      </VCol>
    </VForm>
  </CustomCreateDialog>
</template>
