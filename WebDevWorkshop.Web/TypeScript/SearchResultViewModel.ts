import { SpotifyService, ISearchResult, SearchType, IImage } from "./SpotifyService";
import { merge, from } from "rxjs";
import { take } from "rxjs/operators";

export class SearchResultViewModel {
    constructor(query: string, private spotifyService: SpotifyService) {
        merge<ISearchResult>(
            from(this.spotifyService.searchArtists(query)),
            from(this.spotifyService.searchAlbums(query)),
            from(this.spotifyService.searchTracks(query))
        ).pipe(take(3))
        .subscribe(result => {
            switch (result.type) {
                case SearchType.artist:
                    this.artists(result.items)
                    break;
                case SearchType.album:
                    this.albums(result.items)
                    break;
                case SearchType.track:
                    this.tracks(result.items)
                    break;
            }
        })
    }

    artists = ko.observable<ISearchItem[]>(null);
    tracks = ko.observable<ISearchItem[]>(null);
    albums = ko.observable<ISearchItem[]>(null);
}

interface ISearchItem {
    name: string;
    image: IImage;
}