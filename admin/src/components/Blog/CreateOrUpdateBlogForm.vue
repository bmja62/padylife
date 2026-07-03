<template>
  <VForm
    class="w-100"
    ref="refVForm"
  >
  <VRow>
    <ProgressSpinner
      :progress-amount="progressAmount"
      v-model="showOverlay"
    ></ProgressSpinner>

    <VCol cols="12" md="6">
      <VTextField
        v-model="blogPayload.title"
        required
        :rules="[(value) => !!value || 'این فیلد اجباری است']"
        label="نام خبر *"
      />
    </VCol>
    <VCol cols="12" md="6">
      <BlogCategoryPicker :return-object="false" required v-model="blogPayload.blogCategoryId"></BlogCategoryPicker>
    </VCol>
    <VCol md="4" cols="12">
      <VTextField
        v-model="blogPayload.spendTimeForRead"
        required
        :rules="[(value) => !!value || 'این فیلد اجباری است']"
        label="زمان مطالعه (دقیقه) *"
      />
    </VCol>
    <VCol md="12" cols="12">
      <VTextarea
        v-model="blogPayload.shortDescription"
        color="success"
        hide-details
        label="توضیحات کوتاه"
        max="500"
        variant="outlined"
      />
    </VCol>

    <VCol md="12" cols="12">
      <h2>متن خبر *</h2>
    </VCol>
    <VCol md="12" cols="12">
      <FroalaEditor
        v-model="blogPayload.content"
        editor-placeholder="متن خبر *"
      />
    </VCol>
    <VDivider :thickness="20" color="purple"></VDivider>
    <VCol md="12" cols="12">
      <h2>فایل های چند رسانه ای خبر</h2>
    </VCol>
    <VCol md="6" cols="12" class="">
      <Uploader ref="mainImageFile"
                :key="blogPayload.mainImageFile"
                :default-medias="blogPayload.mainImageFile"
                @getFiles="setFile" label="تصویر اصلی خبر"
                :file-type="UploaderTypes.Blog"></Uploader>
    </VCol>
    <VDivider :thickness="20" color="purple"></VDivider>

    <VCol md="12" cols="12">
      <h2>تنظیمات سئوی خبر</h2>
    </VCol>
    <VCol md="6" cols="12">
      <BlogStatusPicker required v-model="blogPayload.status"></BlogStatusPicker>
    </VCol>
    <VCol md="12" cols="12">
      <UpdateSeo
        ref="seoComponent"
        :title="blogPayload.title"
        :currentData="blogPayload"
        :showBtn="false"
        @setSeoData="updateSeoData"
      ></UpdateSeo>
    </VCol>

    <VCol md="12" cols="12" class="d-flex justify-end">
      <VBtn
        v-if="inCreatePage"
        id="buy-now-btn"
        class="product-buy-now"
        color="green"
        @click="validateData"
      >
        ثبت خبر
      </VBtn>
      <VBtn
        v-else
        id="buy-now-btn"
        class="product-buy-now"
        color="green"
        @click="validateData"
      >
        بروزرسانی خبر
      </VBtn>
    </VCol>
  </VRow>
  </VForm>

</template>

<script lang="ts" setup>
import {IApiProvider} from "@/models/IApiProvider";
import {useSpinner} from "@/composables/spinner";
import {useAlerts} from "@/composables/alert";
import {ICreateOrUpdateBlogDTO} from "@/services/BlogService";
import {UploaderTypes} from "@/models/Enums/UploaderTypes";
import {VForm} from "vuetify/components/VForm";

const router = useRouter();
const route = useRoute();
const alert = useAlerts();
const spinner = useSpinner();
const $api = inject<IApiProvider>("$api");
const showOverlay = ref(false);
const seoComponent = ref(null);
const refVForm = ref(null);
const blogPayload = ref<ICreateOrUpdateBlogDTO>({
  title: "",
  blogCategoryId: null,
  spendTimeForRead: "",
  type: 'Blog',
  seoURL: "",
  seoTitle: "",
  seoDescription: "",
  tableOfContent: "",
  content: "",
  shortDescription: "",
  metaKeywords: "",
  metaAuthor: "",
  ogTitle: '', //
  ogMainPicUrl: '', //
  canonicalLink: '',
  ogurl: '', //
  status: "PreRelease",
  mainImageFile: null,
});
const mainImageFile = ref(null)
const progressAmount = ref(0);
const props = defineProps({
  currentBlogData: {
    type: Object as PropType<object>,
  },
});
onMounted(()=>{
})
watch(
  () => props.currentBlogData,
  async (val) => {
    if (val) {
      setDefaultData();
    }
  },
  {immediate: true}
);
const inCreatePage = computed(() => {
  return route.path.toLowerCase().includes('create')
})

function setFile(medias) {
  if (medias.length) {
    blogPayload.value.mainImageFile = medias[0]
  }
}

function updateSeoData(data) {
  Object.keys(data).forEach((key) => {
    if (blogPayload.value[key]) {
      blogPayload.value[key] = data[key];
    }
  });
}

function setDefaultData() {
  Object.keys(blogPayload.value).forEach((key) => {
    if(key === 'mainImageFile'){
      blogPayload.value.mainImageFile = props.currentBlogData.mainImageUrl;
    }else{

    blogPayload.value[key] = props.currentBlogData[key];
    }
  });
}

async function validateData() {
  const isValid: Object = await refVForm?.value?.validate();
  if (isValid.valid) {
    if (inCreatePage.value) {
      createBlog()
    } else {
      updateBlog()
    }
  } else {
    alert.error('لطفا همه فیلد های اجباری را تکمیل نمایید')
  }
}

async function setOgProperties() {
  blogPayload.value.ogMainPicUrl = blogPayload.value.mainImageFile
  blogPayload.value.ogTitle = blogPayload.value.title
  blogPayload.value.ogurl = blogPayload.value.seoURL
}
async function createBlog() {
  await createTableOfContent();
  await setOgProperties()
  seoComponent.value.setSeoData();
    try {
      mainImageFile.value.getFiles()
      showOverlay.value = true;
      const response = await $api?.blog.createBlog(blogPayload.value, (progress) => {
        progressAmount.value = Math.round(
          (progress.loaded * 100) / progress.total
        );
      });
      if (response.data.isSuccess) {
        alert.success("خبر با موفقیت ساخته شد");
        router.push(`/Blog/Edit/${response.data.data.id}`);
      } else {
        alert.error(response.data.message);
      }
    } catch (error) {
      console.error(error);
    } finally {
      showOverlay.value = false;
    }

}

async function updateBlog() {
  await createTableOfContent();
  seoComponent.value.setSeoData();
    try {
      showOverlay.value = true;
      mainImageFile.value.getFiles()
      blogPayload.value["id"] = props.currentBlogData.id;
      const response = await $api?.blog.updateBlog(blogPayload.value, (progress) => {
        progressAmount.value = Math.round(
          (progress.loaded * 100) / progress.total
        );
      });
      if (response.data.isSuccess) {
        alert.success("خبر با موفقیت بروزرسانی شد");
        router.push("/Blog/List");
      }
    } catch (error) {
      console.error(error);
    } finally {
      showOverlay.value = false;
    }
}

async function createTableOfContent() {
  const parser = new DOMParser();
  blogPayload.value.tableOfContent = "";
  let html = parser.parseFromString(blogPayload.value.content, "text/html");
  const findH2s = html.documentElement.getElementsByTagName("h2");
  const findH3s = html.documentElement.getElementsByTagName("h3");
  let froala = html.documentElement.querySelector("[data-f-id=pbf]");
  if (froala) {
    froala.remove();
  }
  Array.prototype.forEach.call(findH2s, (item) => {
    if (item.innerText !== "") {
      blogPayload.value.tableOfContent += `${item.innerText} ,`;
    }
  });
  Array.prototype.forEach.call(findH3s, (item) => {
    if (item.innerText !== "") {
      blogPayload.value.tableOfContent += `${item.innerText} ,`;
    }
  });
}
</script>

<style scoped></style>
