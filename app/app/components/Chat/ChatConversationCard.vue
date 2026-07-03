<script setup lang="ts">
import type {IChatInfo} from "~~/types/chat";

interface IProps {
  conversation: IChatInfo
}

const props: IProps = defineProps({
  conversation: {
    type: Object as PropType<IChatInfo>
  }
})
const selectedRoom:IChatInfo = defineModel()
</script>

<template>
  <div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<!-- ******************************************** -->
 <!-- <div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div>
<div class="w-full p-3 flex bg-white rounded-2xl shadow mt-2 items-center justify-between transition-all cursor-pointer">
    <div class="w-auto ml-3">
      <div
          class="w-[56px] h-[56px] rounded-2xl"
      >

        <img :src="props.conversation.userInfo.profileImage"
             v-if="props.conversation.userInfo.profileImage" alt="profile-image"
             class="w-[56px] h-[56px] rounded-xl object-cover">
        <img v-else src="/common/no-image.png" class="w-[56px] h-[56px] rounded-xl object-cover" alt="no-image">
      </div>
    </div>
    <div
        class="flex flex-col 2xl:w-10/12 xl:w-9/12 md:w-8/12 sm:w-9/12 w-9/12 gap-1"
    >
      <div class="flex items-center w-full justify-between">
        <div class="flex flex-col gap-2">
          <small class="text-[#0F1828] font-semibold">
            {{ conversation?.userInfo?.userFullName }}
          </small>
          <span class="line-clamp-1 text-[#ADB5BD] text-sm">
            {{ conversation?.lastMessage }}
          </span>
        </div>
        <div class="flex flex-col items-center gap-2">
          <small v-if="conversation?.lastMessageTime" class="text-[12px] text-[#ADB5BD]">
            {{
              new Date(conversation?.lastMessageTime).toLocaleDateString(
                  "fa-IR",
                  {
                    day: 'numeric',
                    month: 'long',
                  }
              )
            }}
          </small>
          <div
              v-if="props.conversation?.unreadMessages"
              class="w-6 h-6 flex items-center justify-center text-white text-sm rounded-full bg-red-600"
          >
            {{ props.conversation.unReadMessageCount }}
          </div>
        </div>
      </div>
    </div>
  </div> -->

</template>


<style scoped>

</style>