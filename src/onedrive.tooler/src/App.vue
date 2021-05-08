<template>
  <template v-if="isAuthenticated" >
    <h1>Authenticated</h1>
    <textarea >
      {{account}}
    </textarea>
  </template>
  <template  v-else>
    <h1>Unauthenticated</h1>
  </template>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import Store from './store'
import { startAuthenticatedSession } from './services/authentication'


export default defineComponent({
  name: 'App',
  setup () {
    const isAuthenticated = Store.state.authentication.isAuthenticated;
    const authResults = Store.state.authentication.authenticationAccount;
    const account = JSON.stringify(authResults, undefined, 2);

    if (!isAuthenticated) {
      startAuthenticatedSession()
    }

    return {
      isAuthenticated,
      account,
      authResults
    }
  }
})
</script>
