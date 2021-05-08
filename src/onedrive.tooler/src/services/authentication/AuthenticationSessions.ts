import Store, { IStore } from '../../store'
import { 
    PublicClientApplication,
    PopupRequest,
    AuthenticationResult, 
    AccountInfo, 
    AuthError} from '@azure/msal-browser'




export const startAuthenticatedSession = async () => {
    const isAuthenticated = Store.state.authentication.isAuthenticated

    if (isAuthenticated) {
        return
    }

    const client = new PublicClientApplication({
        auth:{
            clientId: 'eb00e8f6-a9d1-4b7d-bc5d-c9a52e5cecf0',
            authority: 'https://login.microsoftonline.com/29967363-a86a-4ea6-8f76-29aa44ec6f27',
            redirectUri: 'http://localhost:3000'
        },
        cache: {
            cacheLocation: 'localStorage',
            storeAuthStateInCookie: true
        }
    });

    let results: AuthenticationResult | undefined
    let loginRequest: PopupRequest = {
        redirectUri: 'http://localhost:3000',
        authority: 'https://login.microsoftonline.com/29967363-a86a-4ea6-8f76-29aa44ec6f27',
        scopes: [
            'user.read'
        ],
        prompt: 'none'
    }

    const accounts = client.getAllAccounts();

    if (accounts !== undefined && accounts.length > 1) {
        loginRequest.prompt = 'select_account'
    }

    try {
        results = await client.acquireTokenPopup(loginRequest);
    }
    catch(error) {

        if((error as AuthError)?.errorCode === 'interaction_required') {
            loginRequest.prompt = 'login'
            results = await client.acquireTokenPopup(loginRequest); 
        }
    }


    if(results) {
        Store.dispatch('setAuthenticated', true)
        Store.dispatch('setAuthenticatedResults', results)
        Store.dispatch('setAuthenticatedAccount', results.account);
    }
}


export const endAuthenticatedSession = () =>{

}