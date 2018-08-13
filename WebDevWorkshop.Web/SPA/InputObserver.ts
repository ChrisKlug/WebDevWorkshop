import { Observable, fromEvent } from 'rxjs';
import { debounceTime, map, filter, distinctUntilChanged } from 'rxjs/operators';

const defaultConfig = {
    debounce: 500,
    minLength: 3
}

export class InputObserver {
    private inputObservable: Observable<string>;

    constructor(element: HTMLElement, config?: IConfig) {
        let cfg: IConfig = defaultConfig;
        Object.assign(cfg, config);

        this.inputObservable = fromEvent<Event>(element, "input")
            .pipe(debounceTime(cfg.debounce), map(x => (<HTMLInputElement>x.target).value), filter(x => x.length > cfg.minLength || x.length == 0), distinctUntilChanged());
    }

    get onInput() {
        return this.inputObservable;
    }
}

export interface IConfig {
    debounce?: number;
    minLength?: number;
}