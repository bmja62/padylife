<script setup>
const appUrl = import.meta.env.VITE_APP_URL || "https://app.padylife.ir";
</script>

<template>
  <div class="container">
    <header>
      <div class="header-container">
        <div class="header-right">
          <img src="/src/images/logo.png" alt="logo" />
        </div>
        <a :href="appUrl" class="header-btn">Login / Register</a>
      </div>
    </header>

    <main class="pady-intro-container">
      <div class="intro-txt">
        <div class="intro-header">
          <h1>PadyLife Platform</h1>
          <h2>Smart companion for a healthier life</h2>
        </div>
        <div class="intro-body">
          <p>
            PadyLife helps users build healthy routines with guided plans,
            coaching, and practical tools for daily wellness.
          </p>
        </div>
      </div>
      <img src="/src/images/Pady.png" alt="pady-app" />
    </main>
  </div>
</template>
