import { createApp } from 'vue'
import App from './App.vue'
import Store from './store'
import './index.css'

const app = createApp(App)

app.use(Store)
app.mount('#app')
