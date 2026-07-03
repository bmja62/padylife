<script setup lang="ts">
import type { IApiProvider } from "~/models/IApiProvider";
import type { IGetOrdersParams, IOrder } from "~/services/OrderService";
const router = useRouter()
definePageMeta({
    layout: "dashboard",
    auth: false
});
useHead({
    title: 'قوانین و مقررات'
})
function goBack() {
    router.go(-1);
}
</script>

<template>
    <div class="w-full h-full custom-pattern-bg-image">
        <header class="w-full flex items-center justify-between px-5 py-6">
            <div class="grid grid-cols-12 gap-3 ">
                <div class="col-span-2">
                    <button type="button"
                        class="w-8 h-8 flex cursor-pointer flex-row justify-center items-center p-1 rounded-full bg-white"
                        @click="goBack">
                        <Icon name="icon:chevron-right" size="15" />
                    </button>
                </div>
                <strong class="text-white self-center col-span-10">
                    قوانین و مقررات
                </strong>
            </div>

        </header>
        <div
            class="w-full h-full py-8  px-4 bg-white rounded-t-[32px] flex flex-col justify-between items-start  gap-y-4">
            <article class="w-full text-left">
                <header class="mb-6">
                    <div class="flex items-center gap-4">
                        <div
                            class="w-12 h-12 flex items-center justify-center rounded-lg bg-indigo-100 text-indigo-600 font-bold">
                            P</div>
                        <div>
                            <h1 class="text-2xl md:text-2xl text-right font-semibold">قوانین و مقررات پادی لایف</h1>
                        </div>
                    </div>
                </header>
                <section class="space-y-4 flex flex-col items-start">
                    <h2 class="text-lg font-medium">۱. مقدمه</h2>
                    <p class="text-sm leading-relaxed text-right">استفادهٔ شما از «پادی لایف» به‌معنای پذیرش قوانین و
                        رویه‌های زیر
                        است. لطفاً قبل از استفاده از اپلیکیشن، این قوانين را با دقت بخوانید.</p>


                    <h2 class="text-lg font-medium">۲. تعریف‌ها</h2>
                    <p class="text-sm leading-relaxed text-right">«کاربر» به هر شخصی گفته می‌شود که از خدمات اپلیکیشن
                        استفاده
                        می‌کند. «خدمات» شامل تمام قابلیت‌های مرتبط با توسعهٔ فردی است که در اپ ارائه می‌شود.
                    </p>


                    <h2 class="text-lg font-medium">۳. حساب کاربری و امنیت</h2>
                    <p class="text-sm leading-relaxed text-right">کاربران مسئول حفظ محرمانگی اطلاعات حساب خود هستند.
                        هرگونه فعالیت
                        تحت حساب شما به‌حساب شما نسبت داده می‌شود؛ در صورت سوءاستفاده سریعاً ما را مطلع
                        کنید.</p>


                    <h2 class="text-lg font-medium">۴. استفاده مجاز</h2>
                    <p class="text-sm leading-relaxed text-right">استفاده از خدمات باید مطابق قانون، با احترام به
                        دیگران و بدون
                        اهداف تجاری یا خرابکارانه باشد. هر محتوای غیرقانونی، توهین‌آمیز یا مغایر با اخلاق
                        حذف خواهد شد.
                    </p>


                    <h2 class="text-lg font-medium">۵. مالکیت معنوی</h2>
                    <p class="text-sm leading-relaxed text-right">تمامی حقوق مالکیت معنوی مرتبط با اپ و محتوای اختصاصی
                        آن متعلق به
                        پادی لایف (یا مجوزدهندگان) است. بازنشر بدون مجوز ممنوع است.</p>


                    <h2 class="text-lg font-medium">۶. محرمانگی و داده‌ها</h2>
                    <p class="text-sm leading-relaxed text-right">ما متعهد به حفاظت از حریم خصوصی شما هستیم. اطلاعات
                        پایه‌ای برای
                        ارائهٔ خدمات ذخیره می‌شود. برای جزئیات بیشتر به سیاست حریم خصوصی مراجعه کنید.</p>


                    <h2 class="text-lg font-medium">۷. محدودیت مسئولیت</h2>
                    <p class="text-sm leading-relaxed text-right">اپ تلاش می‌کند محتوای باکیفیتی ارائه دهد اما هیچ
                        تضمینی دربارهٔ
                        نتایج شخصی یا حرفه‌ای وجود ندارد. از اعتماد کامل به توصیه‌های اپ بدون بررسی مستقل
                        خودداری کنید.
                    </p>


                    <h2 class="text-lg font-medium">۸. تغییرات در قوانین</h2>
                    <p class="text-sm leading-relaxed text-right">پادی لایف می‌تواند این قوانین را در هر زمان تغییر
                        دهد. تغییرات
                        مهم از طریق اعلان داخل اپ یا ایمیل اعلام خواهد شد. ادامهٔ استفادهٔ شما به‌معنای
                        پذیرش تغییرات
                        است.</p>


                    <h2 class="text-lg font-medium">۹. تماس</h2>
                    <p class="text-sm leading-relaxed text-right">برای پرسش‌ها یا گزارش مشکلات، با تیم پشتیبانی تماس
                        بگیرید: <a href="mailto:support@padylife.app"
                            class="text-indigo-600 underline">support@padylife.app</a>.</p>
                </section>


                <footer class="mt-8 text-sm text-gray-500">
                    <p>متشکریم که پادی لایف را انتخاب کردید — همراه راه رشد شما.</p>
                </footer>
            </article>
        </div>
    </div>
</template>

<style scoped></style>
