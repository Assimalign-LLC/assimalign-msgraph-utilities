import { createStore } from 'vuex'
import { AuthenticationResult, AccountInfo } from '@azure/msal-browser'

interface IAuthentication {
    isAuthenticated: boolean;
    authenticationAccount?: AccountInfo;
    authenticationResults?: AuthenticationResult 
}



export interface IStore {
    authentication: IAuthentication;
}



const Store = createStore<IStore>({
    state: {
        authentication: {
            isAuthenticated: false,
            authenticationAccount: undefined,
            authenticationResults: undefined
        }
    },
    mutations: {
        setAuthenticated (state, payload) {
            state.authentication.isAuthenticated = payload;
        },
        setAuthenticatedResults (state, payload) {
            state.authentication.authenticationResults = payload
        },
        setAuthenticatedAccount (state, payload) {
            state.authentication.authenticationAccount = payload
        }
    },
    actions: {
        setAuthenticated(context, payload) { 
            context.commit('setAuthenticated', payload)
        },
        setAuthenticatedResults (context, payload) {
            context.commit('setAuthenticatedResults', payload)
        },
        setAuthenticatedAccount (context, payload) {
            context.commit('setAuthenticatedAccount', payload)
        }
    }
})


export default Store