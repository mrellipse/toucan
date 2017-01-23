import Vue = require('vue');

class GlobalEvents {
    public loading: string = 'global.loading';
    public login: string = 'global.login';
    public logout: string = 'global.logoff';
    public localeChange: string = "global.localeChange";
}

export class EventBus extends Vue {

    private globalEvents: GlobalEvents;

    public get global(): GlobalEvents {
        return this.globalEvents;
    }

    constructor() {
        super();
        this.globalEvents = new GlobalEvents();
    }
}

export default EventBus;