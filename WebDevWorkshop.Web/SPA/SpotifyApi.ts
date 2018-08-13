import { merge, from, Subject, Subscription } from 'rxjs';
import { map, delayWhen } from 'rxjs/operators';

export class SpotifyApi {
    private subject = new Subject<ISearchResult>();
    private searchSubscription: Subscription;

    constructor(private apiUrl: string, private accessTokenProvider: IAccessTokenProvider) {}

    async search(query: string, maxResult: number) {
        if (this.searchSubscription) {
            this.searchSubscription.unsubscribe();
            delete this.searchSubscription;
        }

        let accessToken = await this.accessTokenProvider.getAccessToken();

        let artistSearch = this.doSearch(query, SearchType.Artist, accessToken);
        let albumSearch = this.doSearch(query, SearchType.Album, accessToken);
        let trackSearch = this.doSearch(query, SearchType.Track, accessToken);

        this.searchSubscription = merge(artistSearch, albumSearch, trackSearch)
            .pipe(map(x => <Promise<IApiSearchResult>>x.json()), delayWhen(x => from(x)))
            .subscribe(async promise => {
                let result = await promise;
                if (result.albums) {
                    this.subject.next({type: SearchType.Album, items: result.albums.items.slice(0, maxResult)});
                }
                else if (result.artists) {
                    this.subject.next({type: SearchType.Artist, items: result.artists.items.slice(0, maxResult)});
                }
                else if (result.tracks) {
                    this.subject.next({type: SearchType.Track, items: result.tracks.items.slice(0, maxResult)});
                }
            });
    }

    private doSearch(query: string, type: SearchType, accessToken: string) {
        return fetch(`${this.apiUrl}v1/search?q=${query}&type=${SearchType[type].toLowerCase()}`, { headers: { "Authorization": "Bearer " + accessToken } })
    }

    get onSearchResult() {
        return this.subject.asObservable();
    }
}

interface IApiSearchResult {
    artists?: { items: IArtist[] };
    albums?: { items: IAlbum[] };
    tracks?: { items: ITrack[] };
}

export interface ISearchResult {
    type: SearchType;
    items: IAlbum[] | IArtist[] | ITrack[]
}

export interface IArtist {
    name: string;
    href: string;
    images: IImage[];
}

export interface IAlbum {
    name: string;
    artists: { name: string }[];
    images: IImage[];
    href: string;
}

export interface ITrack {
    name: string;
    album: IAlbum;
    artists: { name: string }[];
    href: string;
}

export interface IImage {
    height: number;
    width: number;
    url: string;
}

export enum SearchType {
    Artist,
    Album,
    Track
}

interface IAccessTokenProvider {
    getAccessToken(): Promise<string>;
}