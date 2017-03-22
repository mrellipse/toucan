export interface IOptionsService<T> {
    ensure(): T;
    options: T
}

export class OptionsService<T> implements IOptionsService<T> {

    constructor(private key: string, private defaults?: T) {

    }

    public ensure(): T {

        let options = this.options;
        if (!this.options) {
            this.options = this.defaults;
        }
        return options || this.options;
    }

    public get options(): T {
        let options = localStorage.getItem(this.key);
        return options !== 'undefined' ? JSON.parse(options) : null;
    }

    public set options(value: T) {
        localStorage.setItem(this.key, JSON.stringify(value));
    }
}

export default OptionsService;