<template>
  <template v-if="get()" >
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
import { Store } from 'vuex'
import { startAuthenticatedSession } from './services/authentication'
import { useStore } from 'vuex';


export default defineComponent({
  name: 'App',
  setup () {

    
    const authResults = Store.state.authentication.authenticationAccount;
    const account = JSON.stringify(authResults, undefined, 2);



    return {
      account,
      authResults
    }
  },

  methods: {
    get(){
      return this.authentication.isAuthenticated
    }
  }
})
</script>
