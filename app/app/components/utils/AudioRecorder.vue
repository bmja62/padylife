<template>
  <div class="w-full flex items-center"></div>
</template>

<script setup>
import {ref} from 'vue';

const emit = defineEmits(["getAudioBlob", "getMediaRecorderState"]);
const isPlaying = ref(false);
const audioChunks = ref([]);
const audioBlob = ref(null);
const mainStream = ref(null);
const alert = useAlerts()
let mediaRecorder = null;

const startRecording = async () => {
  try {
    mainStream.value = await navigator.mediaDevices.getUserMedia({audio: true});
    isPlaying.value = true
    handleRecording();
  } catch (error) {
    console.log(error);
    if (
        error.name === "NotFoundError" ||
        error.name === "DevicesNotFoundError"
    ) {
      alert.error(`هیچ میکروفونی روی دستگاه شما یافت نشد`);
    } else if (
        error.name === "NotAllowedError" ||
        error.name === "PermissionDeniedError"
    ) {
      alert.error('دسترسی میکروفون خود را فعال کنید');
    }
  }
};

const handleRecording = () => {
  mediaRecorder = new MediaRecorder(mainStream.value);
  mediaRecorder.start();
  mediaRecorder.addEventListener("dataavailable", (event) => {
    audioChunks.value.push(event.data);
  });
  emit("getMediaRecorderState", mediaRecorder.state);
};

const stopRecording = async () => {
  await mediaRecorder.addEventListener("stop", () => {
    audioBlob.value = new Blob(audioChunks.value, {
      type: 'audio/mp3',
    });
    // Create a proper File object from the Blob
    const audioFile = new File([audioBlob.value], 'recording.mp3', {
      type: 'audio/mp3',
      lastModified: Date.now()
    });

    emit("getAudioBlob", audioFile);
  });

  if (mediaRecorder.state !== "inactive") {
    mainStream.value.getTracks()[0].stop();
    mediaRecorder.stop();
  }
  audioChunks.value = []
  isPlaying.value = false;
  mediaRecorder.value = null
  emit("getMediaRecorderState", mediaRecorder.state);
};

defineExpose({
  startRecording,
  isPlaying,
  stopRecording
});
</script>

<style scoped></style>