<script setup lang="ts">

</script>

<template>
  <div class="relative">
  <ul class="hearts  flying">
    <li class="heart "></li>
    <li class="heart "></li>
    <li class="heart "></li>
    <li class="heart "></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
    <li class="heart"></li>
  </ul>
  </div>
</template>

<style scoped>

.hearts {
  transform: translateX(-50%);
}

.hearts .heart {
  position: absolute;
  list-style: none;
  width: 32px;
  height: 32px;
  margin: 4px;
}

.hearts .heart:before,
.hearts .heart:after {
  position: absolute;
  content: "";
  left: 16px;
  top: 0;
  width: 16px;
  background: red;
  height: 24px;
  -moz-border-radius: 16px 16px 0 0;
  -o-border-radius: 16px 16px 0 0;
  border-radius: 16px 16px 0 0;
  -webkit-transform: rotate(-45deg);
  -moz-transform: rotate(-45deg);
  -o-transform: rotate(-45deg);
  transform: rotate(-45deg);
  -webkit-transform-origin: 0 100%;
  -moz-transform-origin: 0 100%;
  -o-transform-origin: 0 100%;
  transform-origin: 0 100%;
}

.hearts .heart:after {
  left: 0;
  -webkit-transform: rotate(45deg);
  -moz-transform: rotate(45deg);
  -o-transform: rotate(45deg);
  transform: rotate(45deg);
  -webkit-transform-origin: 100% 100%;
  -moz-transform-origin: 100% 100%;
  -o-transform-origin: 100% 100%;
  transform-origin: 100% 100%;
}

.hearts.flying > .heart:nth-child(even) {
  -webkit-animation: 3.8s flyingEven linear infinite;
  -moz-animation: 2.8s flyingEven linear infinite;
  animation: 2.8s flyingEven linear infinite;
}

.hearts.flying > .heart:nth-child(odd) {
  -webkit-animation: 3.8s flyingOdd linear infinite;
  -moz-animation: 2.8s flyingOdd linear infinite;
  animation: 2.8s flyingOdd linear infinite;
}

.hearts.flying > .heart:nth-child(1) {
  animation-delay: 0.2s;
}

.hearts.flying > .heart:nth-child(2) {
  animation-delay: 0.3s;
}

.hearts.flying > .heart:nth-child(3) {
  animation-delay: 0.6s;
}

.hearts.flying > .heart:nth-child(4) {
  animation-delay: 1.5s;
}

.hearts.flying > .heart:nth-child(5) {
  animation-delay: 2.2s;
}

.hearts.flying > .heart:nth-child(6) {
  animation-delay: 2.9s;
}

.hearts.flying > .heart:nth-child(7) {
  animation-delay: 3.2s;
}

.hearts.flying > .heart:nth-child(8) {
  animation-delay: 3.5s;
}

.hearts.flying > .heart:nth-child(9) {
  animation-delay: 3.8s;
}

.hearts.flying > .heart:nth-child(10) {
  animation-delay: 4.1s;
}

.hearts.flying > .heart:nth-child(11) {
  animation-delay: 4.5s;
}

.hearts.flying > .heart:nth-child(12) {
  animation-delay: 8s;
}

@-moz-keyframes flyingEven {
  0% {
    transform: translate(0, 0) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(35px, 0px) scale(1.5);
    opacity: 0;
    filter: blur(5px);
  }
}

@-webkit-keyframes flyingEven {
  0% {
    transform: translate(0px, 0px) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(0px, -500px) scale(1);
    opacity: 0;
    filter: blur(5px);
  }
}

@keyframes flyingEven {
  0% {
    transform: translate(5px, 10px) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(0px, -480px) scale(1.5);
    opacity: 0;
    filter: blur(5px);
  }
}

@-moz-keyframes flyingOdd {
  0% {
    transform: translate(5px, 15px) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(10px, -480px) scale(1.5);
    opacity: 0;
    filter: blur(5px);
  }
}

@-webkit-keyframes flyingOdd {
  0% {
    transform: translate(0, 0) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(-35px, -480px) scale(1.5);
    opacity: 0;
    filter: blur(5px);
  }
}

@keyframes flyingOdd {
  0% {
    transform: translate(0, 0) scale(1);
    opacity: 1;
    filter: blur(0px);
  }
  100% {
    transform: translate(-60px, -480px) scale(1.5);
    opacity: 0;
    filter: blur(5px);
  }
}
</style>