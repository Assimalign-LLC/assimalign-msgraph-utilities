import { createStore } from 'vuex'

interface IAuthentication {
    isAuthenticated: boolean;
}



interface IStore {
    authentication: IAuthentication;
}


const Store = createStore<IStore>({
    state: {
        authentication: {
            isAuthenticated: false
        }
    },
    mutations: {
        setAuthenitcated (state, payload) {
            state.authentication.isAuthenticated = payload;
        }
    }
})


export default Store