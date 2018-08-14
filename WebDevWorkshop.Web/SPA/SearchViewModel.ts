import { SpotifyApi, ISearchResult, SearchType, IArtist, IAlbum, ITrack, IImage } from './SpotifyApi';
import { Observable } from 'rxjs';
import * as ko from 'knockout';

export class SearchViewModel {
    constructor(private api: SpotifyApi, input: Observable<string>, maxResults: number) {
        input.subscribe(query => this.onInput(query, maxResults))

        api.onSearchResult.subscribe(x => this.onSearchResult(x))
    }
    
    private onInput(query: string, maxResults: number) {
        this.isSearching(true);
        if (!query) {
            this.albums(null);
            this.artists(null);
            this.tracks(null);
            return;
        }
        this.api.search(query, maxResults);
    }
    private onSearchResult(result: ISearchResult) {
        this.isSearching(false);
        switch (result.type) {
            case SearchType.Album:
                this.albums((<IAlbum[]>result.items).map(x => ({name: x.name, image: this.getImage(x)})))
                break;
            case SearchType.Artist:
                this.artists((<IArtist[]>result.items).map(x => ({name: x.name, image: this.getImage(x)})))
                break;
            case SearchType.Track:
                this.tracks((<ITrack[]>result.items).map(x => ({name: x.name, artist: x.artists[0].name, image: this.getImage(x.album) })))
                break;
        }
    }
    private getImage(item: { images: IImage[] }) {
        if (!item.images || !item.images.length) return null;
        return item.images.reduce((prev, current) => { return (prev && prev.width < current.width) ? prev : current }, null);
    }

    albums = ko.observable();
    artists = ko.observable();
    tracks = ko.observable();
    isSearching = ko.observable(false);
    hasResults = ko.computed(() => this.albums() || this.artists() || this.tracks())
}