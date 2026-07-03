<script setup lang="ts">

import {useSpinner} from "@/composables/spinner";
import {IProduct, IProductImageGallery, IProductImageMain} from "@/services/ProductService";
import {UploaderTypesMapped} from "@/models/Enums/UploaderTypes";
import {inject} from "vue";
import {IApiProvider} from "@/models/IApiProvider";
import {useAlerts} from "@/composables/alert";

const $api = inject<IApiProvider>('$api')
const emits = defineEmits<{
  (e: 'refetch'): void
}>()
const productInfo: IProduct = defineModel()
const route = useRoute()
const mainImagePayload = ref<IProductImageMain>({
  objectId: route.params.id,
  type: UploaderTypesMapped.ProductMain,
  mainImage: null
})
const galleryImagePayload = ref<IProductImageGallery>({
  objectId: route.params.id,
  type: UploaderTypesMapped.ProductGallery,
  galleryImage: null
})

function setMainImage(event) {
  mainImagePayload.value.mainImage = event.target.files[0]
  uploadMainImage()
}

async function uploadMainImage() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.changeMainImage(mainImagePayload.value)
    if (data.data.isSuccess) {
      useAlerts().success('تصویر اصلی محصول با موفقیت تغییر یافت')
      emits('refetch')
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function setGalleryImages(event) {
  for (let i = 0; i < event.target.files.length; i++) {
    galleryImagePayload.value.galleryImage = event.target.files[i]
    await uploadGalleryImages()
  }
}

async function uploadGalleryImages() {
  try {
    useSpinner().showSpinner()
    const data = await $api?.product.addImageToGallery(galleryImagePayload.value)
    if (data.data.isSuccess) {
      useAlerts().success('تصاویر محصول با موفقیت افزوده شدند')
      galleryImagePayload.value.galleryImage = null
      emits('refetch')
    }
  } catch (e) {
    console.error(e)
  } finally {
    useSpinner().hideSpinner()
  }
}

async function deleteMedia(item) {
  try {
    useSpinner().showSpinner()
    const response = await $api?.uploader.delete(item.url.split('/').filter(e => e)[6])
    if (response.data.isSuccess) {
      useAlerts().success('تصویر با موفقیت حذف شد')
      emits('refetch')
    } else {
      useAlerts().error(response.data.errorMessage)
    }
  } catch (error) {
    console.error(error)
  } finally {
    useSpinner().hideSpinner()
  }
}
</script>

<template>
  <VRow>
    <VCol cols="12">
      <h2 class="border-b pb-2">
        رسانه
      </h2>
    </VCol>
    <VCol cols="12" md="12">
      <VFileInput
        id="mainImage"
        label="تصویر اصلی محصول"
        accept="image/*"
        :rules="[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']"
        @input="setMainImage($event)"
      ></VFileInput>
      <div v-if="productInfo.productImages.main" class=" mt-2 relative  gap-2">
        <div style="cursor: pointer;z-index: 999!important" @click="deleteMedia(productInfo.productImages.main)">
          <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>
        </div>
        <VImg :src="productInfo.productImages.main.url"
              style="object-fit: cover;width: 10rem;height: 10rem"></VImg>
      </div>
    </VCol>
    <VCol cols="12" md="12">
      <VFileInput
        id="galleryImages"
        label="افزودن تصاویر محصول"
        accept="image/*"
        multiple
        :rules="[(value: File[]) => value.length > 0  || 'انتخاب عکس اجباری است']"
        @input="setGalleryImages($event)"
      ></VFileInput>
      <div v-if="productInfo.productImages.gallery.length" class="d-flex align-content-center justify-start mt-2">
        <div v-for="(item,idx) in productInfo.productImages.gallery" class="position-relative"
             style="width: 10rem;height: 10rem">
          <div style="cursor: pointer;z-index: 999!important" @click="deleteMedia(item,idx)">
            <VIcon class="position-absolute " color="error" icon="tabler-x" style="z-index: 999!important"></VIcon>
          </div>
          <VImg :src="item.url"
                style="object-fit: cover;width: 7rem;height: 7rem"></VImg>
        </div>

      </div>
    </VCol>
  </VRow>
</template>

<style scoped lang="scss">

</style>
