import { SecurityContext } from './SecurityContext';
import { ISpaConfig } from './ISpaConfig';
import { SpotifyService } from './SpotifyService';
import { fromEvent } from 'rxjs';
import { map, tap, switchMap } from 'rxjs/operators';
import { SearchResultViewModel } from './SearchResultViewModel';
import { ModalService } from './ModalService';
import * as searchResultTemplate from './SearchResultViewModel.html';

declare var spaConfig: ISpaConfig;

const securityContext = new SecurityContext({
    authority: spaConfig.oidcAuthority,
    clientId: "WDWSPA",
    redirectUri: "/auth/silentsignincallback",
    scopes: ["WDWApi"],
    onSignInFailure: () => document.location.href = "/auth/signin"
});

const spotify = new SpotifyService(spaConfig.apiEndpoint, securityContext);

const modalService = new ModalService();

const input = document.querySelector<HTMLInputElement>("input[type=text");
const btn = document.querySelector<HTMLButtonElement>("#btnSearch");

const input$ = fromEvent(input, "keyup").pipe(map(x => (<HTMLInputElement>x.target).value));
const search$ = fromEvent(btn, "click").pipe(tap(x => x.preventDefault()));

input$.pipe(
    tap(x => btn.disabled = x.length <= 2),
    switchMap(x => search$.pipe(map(z => x)))
).subscribe(query => {
    const vm = new SearchResultViewModel(query, spotify);
    modalService.showModal(searchResultTemplate, vm);
})
