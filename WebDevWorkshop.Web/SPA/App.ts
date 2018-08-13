import { SecurityContext } from './SecurityContext';
import { ISpaConfig } from './ISpaConfig';
import { SpotifyApi } from './SpotifyApi';
import { InputObserver } from './InputObserver';
import { SearchViewModel } from './SearchViewModel';
import 'knockout';

declare var spaConfig: ISpaConfig;

const securityContext = new SecurityContext({
    authority: spaConfig.oidcAuthority,
    client_id: "WDWSPA",
    silent_redirect_uri: new URL('/auth/silentsignincallback', window.location.href).toString(),
    scope: "openid WDWApi",
    response_type: "id_token token",
    onSignInFailure: () => document.location.href = "/auth/signin"
});
const spotifyApi = new SpotifyApi(spaConfig.apiBaseUrl, securityContext);

const txt = <HTMLInputElement>document.getElementById("txtSearch");
const inputObserver = new InputObserver(txt);

const vm = new SearchViewModel(spotifyApi, inputObserver.onInput, 5);
ko.applyBindings(vm);

document.getElementById("loading").classList.add("hidden");
document.getElementById("app").classList.remove("hidden");

