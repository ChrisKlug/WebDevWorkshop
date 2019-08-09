export class SpotifyService {
    constructor(private apiUrl: string, private accessTokenProvider: IAccessTokenProvider) { }

    searchArtists(query: string) {
        return this.search(SearchType.artist, query, "artists");
    }
    searchAlbums(query: string) {
        return this.search(SearchType.album, query, "albums");
    }
    searchTracks(query: string) {
        return this.search(SearchType.track, query, "tracks");
    }

    private async search(type: SearchType, query: string, itemsProp: string): Promise<ISearchResult> {
        const accessToken = await this.accessTokenProvider.getAccessToken();
        const headers = new Headers();
        headers.set("Authorization", "Bearer " + accessToken);

        let response: Response;
        try {
            response = await fetch(`${this.apiUrl}v1/search?type=${SearchType[type]}&q=${encodeURIComponent(query)}`, { headers: headers });
        }
        catch (ex) {
            console.error("Error while retrieving results", ex);
            throw ex;
        }

        let data: any;
        try {
            data = await response.json();
        }
        catch (ex) {
            console.error("Error while converting JSON response", ex);
            throw ex;
        }

        return data ? {
            type: type,
            items: (<any[]>data[itemsProp].items).map(x => {
                return {
                    name: x.name + (x.artists ? " (" + (<any[]>x.artists).map(x => x.name).join(", ") + ")" : ""),
                    image: this.getImage(x.images)
                }
            })
        } : null;
    }

    private getImage(images: IImage[]) {
        if (!images || !images.length) {
            return null;
        }
        return images.reduce((prev, current) => !prev || prev.width> current.width ? current: prev, null)
    }
}

export enum SearchType {
    artist,
    track,
    album
}

export interface IImage {
    height: number;
    width: number;
    url: string;
}

export interface ISearchResult {
    type: SearchType;
    items: { name: string; image: IImage; }[];
}

interface IAccessTokenProvider {
    getAccessToken(): Promise<string>;
}