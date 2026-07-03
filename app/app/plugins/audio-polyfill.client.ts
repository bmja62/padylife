import AudioRecorder from "audio-recorder-polyfill";

export default defineNuxtPlugin(() => {
    window.MediaRecorder = AudioRecorder;
})