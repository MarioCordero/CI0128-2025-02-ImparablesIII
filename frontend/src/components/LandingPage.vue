<template>
  <div class="min-h-screen bg-gradient-to-b from-blue-50 to-blue-200 flex flex-col">

    <!-- Hero Section -->
    <section class="flex-1 flex flex-col items-center justify-center text-center px-4">
      <h1 class="text-4xl md:text-6xl font-extrabold text-blue-800 mb-4">
        Organize Your Life with PlaniFy
      </h1>
      <p class="text-lg md:text-2xl text-blue-700 mb-8">
        The easiest way to plan, track, and achieve your goals.
      </p>
      <button class="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-8 py-3 rounded-lg shadow transition">
        Get Started
      </button>
    </section>

    <!-- Features Section -->
    <section class="bg-white py-12">
      <div class="max-w-4xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-8 px-4">
        <div class="flex flex-col items-center">
          <span class="text-blue-600 text-4xl mb-2">📅</span>
          <h2 class="font-bold text-xl mb-2">Easy Scheduling</h2>
          <p class="text-gray-600 text-center">Plan your tasks and events with a simple, intuitive interface.</p>
        </div>
        <div class="flex flex-col items-center">
          <span class="text-blue-600 text-4xl mb-2">✅</span>
          <h2 class="font-bold text-xl mb-2">Track Progress</h2>
          <p class="text-gray-600 text-center">Stay on top of your goals and see your achievements grow.</p>
        </div>
        <div class="flex flex-col items-center">
          <span class="text-blue-600 text-4xl mb-2">🔔</span>
          <h2 class="font-bold text-xl mb-2">Smart Reminders</h2>
          <p class="text-gray-600 text-center">Never miss a deadline with customizable notifications.</p>
        </div>
      </div>
    </section>

    <section class="bg-white py-12">
  <div class="max-w-4xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-8 px-4 border border-red-500">
    <div class="bg-blue-100">A</div>
    <div class="bg-blue-200">B</div>
    <div class="bg-blue-300">C</div>
  </div>
</section>

    <!-- Footer -->
    <footer class="text-center text-gray-500 py-6">
      © 2025 PlaniFy. All rights reserved.
    </footer>
  </div>
</template>

<script>
export default {
  name: "LandingPage",
};
</script>